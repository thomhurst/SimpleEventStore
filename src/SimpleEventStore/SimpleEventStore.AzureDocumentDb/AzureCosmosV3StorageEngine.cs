using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;

namespace SimpleEventStore.AzureDocumentDb
{
    internal class AzureCosmosV3StorageEngine : IStorageEngine
    {
        private readonly CosmosClient _client;
        private readonly string _databaseName;
        private readonly CollectionOptionsV3 _collectionOptionsV3;
        private readonly LoggingOptions _loggingOptions;
        private readonly ISerializationTypeMap _typeMap;
        private readonly JsonSerializer _jsonSerializer;
        private readonly DatabaseOptions _databaseOptions;
        private Database _database;
        private Container _collection;
        private (string Name, string Body) _storedProcedureInformation;

        internal AzureCosmosV3StorageEngine(CosmosClient client, string databaseName, CollectionOptionsV3 collectionOptionsV3, DatabaseOptions databaseOptions, LoggingOptions loggingOptions, ISerializationTypeMap typeMap, JsonSerializer serializer)
        {
            _client = client;
            _databaseName = databaseName;
            _databaseOptions = databaseOptions;
            _collectionOptionsV3 = collectionOptionsV3;
            _loggingOptions = loggingOptions;
            _typeMap = typeMap;
            _jsonSerializer = serializer;
        }

        public async Task<IStorageEngine> Initialise(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var databaseResponse = await CreateDatabaseIfItDoesNotExist();
            _database = databaseResponse.Database;

            cancellationToken.ThrowIfCancellationRequested();
            var containerResponse = (await CreateCollectionIfItDoesNotExist());
            _collection = containerResponse.Container;
            
            cancellationToken.ThrowIfCancellationRequested();
            await Task.WhenAll(
                InitialiseStoredProcedure(),
                SetDatabaseOfferThroughput(),
                SetCollectionOfferThroughput()
                );

            return this;
        }

        public async Task AppendToStream(string streamId, IEnumerable<StorageEvent> events, CancellationToken cancellationToken = default)
        {
            var docs = events.Select(d => DocumentDbStorageEvent.FromStorageEvent(d, _typeMap, _jsonSerializer)).ToArray<dynamic>();

            try
            {
                var result = await _collection.Scripts.ExecuteStoredProcedureAsync<dynamic>(
                    _storedProcedureInformation.Name,
                    new PartitionKey(streamId),
                    docs,
                    new StoredProcedureRequestOptions()
                    {
                        ConsistencyLevel = _collectionOptionsV3.ConsistencyLevel
                    },
                    cancellationToken);

                _loggingOptions.OnSuccess(ResponseInformation.FromWriteResponse(nameof(AppendToStream), result));
            }
            catch (CosmosException ex) when (ex.Headers["x-ms-substatus"] == "409" || ex.SubStatusCode == 409)
            {
                throw new ConcurrencyException(ex.ResponseBody, ex);
            }
        }

        public async Task<IReadOnlyCollection<StorageEvent>> ReadStreamForwards(string streamId, int startPosition, int numberOfEventsToRead, CancellationToken cancellationToken = default)
        {
            int endPosition = numberOfEventsToRead == int.MaxValue ? int.MaxValue : startPosition + numberOfEventsToRead;

            var eventsQuery = _collection.GetItemLinqQueryable<DocumentDbStorageEvent>()
                .Where(x => x.StreamId == streamId && x.EventNumber >= startPosition && x.EventNumber <= endPosition)
                .OrderBy(x => x.EventNumber)
                .ToFeedIterator();

            var events = new List<StorageEvent>();

            while (eventsQuery.HasMoreResults)
            {
                var response = await eventsQuery.ReadNextAsync(cancellationToken);
                _loggingOptions.OnSuccess(ResponseInformation.FromReadResponse(nameof(ReadStreamForwards), response));

                foreach (var e in response)
                {
                    events.Add(e.ToStorageEvent(_typeMap, _jsonSerializer));
                }
            }

            return events.AsReadOnly();
        }

        private Task<DatabaseResponse> CreateDatabaseIfItDoesNotExist()
        {
            return _client.CreateDatabaseIfNotExistsAsync(_databaseName, _databaseOptions.DatabaseRequestUnits);
        }

        private Task<ContainerResponse> CreateCollectionIfItDoesNotExist()
        {
            var collectionProperties = new ContainerProperties()
            {
                Id = _collectionOptionsV3.CollectionName,
                IndexingPolicy = new IndexingPolicy { 
                    IncludedPaths =
                    {
                        new IncludedPath { Path = "/*" },
                    },
                    ExcludedPaths =
                    {
                        new ExcludedPath { Path = "/body/*" },
                        new ExcludedPath { Path = "/metadata/*" }
                    }
                },
                DefaultTimeToLive = _collectionOptionsV3.DefaultTimeToLive,
                PartitionKeyPath = "/streamId"
            };

            return _database.CreateContainerIfNotExistsAsync(collectionProperties,
                _collectionOptionsV3.CollectionRequestUnits);
        }

        private async Task InitialiseStoredProcedure()
        {
            _storedProcedureInformation = AppendSprocProvider.GetAppendSprocData();

            try
            {
                await _collection.Scripts.ReadStoredProcedureAsync(_storedProcedureInformation.Name);
            }
            catch (CosmosException cosmosException) when(cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                await _collection.Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties(_storedProcedureInformation.Name, _storedProcedureInformation.Body));
            }
        }
        private async Task SetCollectionOfferThroughput()
        {
            if (_collectionOptionsV3.CollectionRequestUnits != null)
            {
                await _collection.ReplaceThroughputAsync((int) _collectionOptionsV3.CollectionRequestUnits);
            }
        }

        private async Task SetDatabaseOfferThroughput()
        {
            if (_databaseOptions.DatabaseRequestUnits != null)
            {
                await _database.ReplaceThroughputAsync((int) _databaseOptions.DatabaseRequestUnits);
            }
        }
    }
}
