﻿using System;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace SimpleEventStore.AzureDocumentDb
{
    public class AzureDocumentDbStorageEngineBuilder
    {
        private readonly string databaseName;
        private readonly DocumentClient client;
        private readonly CollectionOptionsV2 _collectionOptionsV2 = new CollectionOptionsV2();
        private readonly DatabaseOptions databaseOptions = new DatabaseOptions();
        private readonly LoggingOptions loggingOptions = new LoggingOptions();
        private ISerializationTypeMap typeMap = new DefaultSerializationTypeMap();
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();

        public AzureDocumentDbStorageEngineBuilder(DocumentClient client, string databaseName)
        {
            Guard.IsNotNull(nameof(client), client);
            Guard.IsNotNullOrEmpty(nameof(databaseName), databaseName);

            this.client = client;
            this.databaseName = databaseName;
        }

        public AzureDocumentDbStorageEngineBuilder UseCollection(Action<CollectionOptionsV2> action)
        {
            Guard.IsNotNull(nameof(action), action);

            action(_collectionOptionsV2);
            return this;
        }

        public AzureDocumentDbStorageEngineBuilder UseLogging(Action<LoggingOptions> action)
        {
            Guard.IsNotNull(nameof(action), action);

            action(loggingOptions);
            return this;
        }

        public AzureDocumentDbStorageEngineBuilder UseTypeMap(ISerializationTypeMap typeMap)
        {
            Guard.IsNotNull(nameof(typeMap), typeMap);
            this.typeMap = typeMap;

            return this;
        }

        public AzureDocumentDbStorageEngineBuilder UseJsonSerializerSettings(JsonSerializerSettings settings)
        {
            Guard.IsNotNull(nameof(settings), settings);
            this.jsonSerializerSettings = settings;
            return this;
        }

        public AzureDocumentDbStorageEngineBuilder UseDatabase(Action<DatabaseOptions> action)
        {
            Guard.IsNotNull(nameof(action), action);

            action(databaseOptions);
            return this;
        }

        public IStorageEngine Build()
        {
            var engine = new AzureDocumentDbStorageEngine(this.client, this.databaseName, this._collectionOptionsV2,this.databaseOptions, this.loggingOptions, this.typeMap, JsonSerializer.Create(this.jsonSerializerSettings));
            return engine;
        }
    }
}