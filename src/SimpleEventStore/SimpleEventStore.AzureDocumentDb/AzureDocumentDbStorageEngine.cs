﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace SimpleEventStore.AzureDocumentDb
{
    internal class AzureDocumentDbStorageEngine : IStorageEngine
    {
        private const string AppendStoredProcedureName = "appendToStream";
        private const string ConcurrencyConflictErrorKey = "Concurrency conflict.";

        private readonly DocumentClient client;
        private readonly string databaseName;
        private readonly CollectionOptions collectionOptions;
        private readonly Uri commitsLink;
        private readonly Uri storedProcLink;
        private readonly List<Subscription> subscriptions = new List<Subscription>();
        private readonly SubscriptionOptions subscriptionOptions;

        internal AzureDocumentDbStorageEngine(DocumentClient client, string databaseName, CollectionOptions collectionOptions, SubscriptionOptions subscriptionOptions)
        {
            this.client = client;
            this.databaseName = databaseName;
            this.collectionOptions = collectionOptions;
            this.commitsLink = UriFactory.CreateDocumentCollectionUri(databaseName, collectionOptions.CollectionName);
            this.storedProcLink = UriFactory.CreateStoredProcedureUri(databaseName, collectionOptions.CollectionName, AppendStoredProcedureName);
            this.subscriptionOptions = subscriptionOptions;
        }

        public async Task<IStorageEngine> Initialise()
        {
            await CreateDatabaseIfItDoesNotExist();
            await CreateCollectionIfItDoesNotExist();
            await CreateAppendStoredProcedureIfItDoesNotExist();

            return this;
        }

        public async Task AppendToStream(string streamId, IEnumerable<StorageEvent> events)
        {
            var docs = events.Select(d => DocumentDbStorageEvent.FromStorageEvent(d)).ToList();

            try
            {
                var result = await this.client.ExecuteStoredProcedureAsync<dynamic>(
                    storedProcLink, 
                    new RequestOptions { PartitionKey = new PartitionKey(streamId), ConsistencyLevel = this.collectionOptions.ConsistencyLevel },
                    docs);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Error.Message.Contains(ConcurrencyConflictErrorKey))
                {
                    throw new ConcurrencyException(ex.Error.Message, ex);
                }

                throw;
            }
        }

        public async Task<IReadOnlyCollection<StorageEvent>> ReadStreamForwards(string streamId, int startPosition, int numberOfEventsToRead)
        {
            int endPosition = numberOfEventsToRead == int.MaxValue ? int.MaxValue : startPosition + numberOfEventsToRead;

            var eventsQuery = this.client.CreateDocumentQuery<DocumentDbStorageEvent>(commitsLink)
                .Where(x => x.StreamId == streamId && x.EventNumber >= startPosition && x.EventNumber <= endPosition)
                .OrderBy(x => x.EventNumber)
                .AsDocumentQuery();

            var events = new List<StorageEvent>();

            while (eventsQuery.HasMoreResults)
            {
                foreach (var e in await eventsQuery.ExecuteNextAsync<DocumentDbStorageEvent>())
                {
                    events.Add(e.ToStorageEvent());
                }
            }

            return events.AsReadOnly();
        }

        public void SubscribeToAll(Action<IReadOnlyCollection<StorageEvent>, string> onNextEvent, string checkpoint)
        {
            EnsureSubscriptionsAreEnabled();
            Guard.IsNotNull(nameof(onNextEvent), onNextEvent);

            var subscription = new Subscription(this.client, this.commitsLink, onNextEvent, checkpoint, this.subscriptionOptions);
            subscriptions.Add(subscription);

            subscription.Start();
        }

        private void EnsureSubscriptionsAreEnabled()
        {
            if (subscriptionOptions == null)
            {
                throw new SubscriptionsNotConfiguredException("Ensure subscription options have been supplied prior to using subscription features.");
            }
        }

        private async Task CreateDatabaseIfItDoesNotExist()
        {
            var databaseExistsQuery = client.CreateDatabaseQuery()
                .Where(x => x.Id == databaseName)
                .Take(1)
                .AsDocumentQuery();

            if (!(await databaseExistsQuery.ExecuteNextAsync<Database>()).Any())
            {
                await client.CreateDatabaseAsync(new Database {Id = databaseName});
            }
        }

        private async Task CreateCollectionIfItDoesNotExist()
        {
            var databaseUri = UriFactory.CreateDatabaseUri(databaseName);

            var commitsCollectionQuery = client.CreateDocumentCollectionQuery(databaseUri)
                .Where(x => x.Id == collectionOptions.CollectionName)
                .Take(1)
                .AsDocumentQuery();

            if (!(await commitsCollectionQuery.ExecuteNextAsync<DocumentCollection>()).Any())
            {
                var collection = new DocumentCollection();
                collection.Id = collectionOptions.CollectionName;
                collection.PartitionKey.Paths.Add("/streamId");
                collection.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
                collection.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/body/*"});
                collection.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/metadata/*" });

                var requestOptions = new RequestOptions
                {
                    OfferThroughput = collectionOptions.CollectionRequestUnits
                };

                await client.CreateDocumentCollectionAsync(databaseUri, collection, requestOptions);
            }
        }

        private async Task CreateAppendStoredProcedureIfItDoesNotExist()
        {
            var query = client.CreateStoredProcedureQuery(commitsLink)
                .Where(x => x.Id == AppendStoredProcedureName)
                .AsDocumentQuery();

            if (!(await query.ExecuteNextAsync<StoredProcedure>()).Any())
            { 
                await client.CreateStoredProcedureAsync(commitsLink, new StoredProcedure
                {
                    Id = AppendStoredProcedureName,
                    Body = Resources.GetString("AppendToStream.js")
                });
            }
        }
    }
}
