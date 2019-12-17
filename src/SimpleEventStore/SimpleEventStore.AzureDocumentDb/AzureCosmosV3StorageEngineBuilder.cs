using System;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace SimpleEventStore.AzureDocumentDb
{
    public class AzureCosmosV3StorageEngineBuilder
    {
        private readonly string _databaseName;
        private readonly CosmosClient _client;
        private readonly CollectionOptionsV3 _collectionOptionsV3 = new CollectionOptionsV3();
        private readonly DatabaseOptions _databaseOptions = new DatabaseOptions();
        private readonly LoggingOptions _loggingOptions = new LoggingOptions();
        private ISerializationTypeMap _typeMap = new DefaultSerializationTypeMap();
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings();

        public AzureCosmosV3StorageEngineBuilder(CosmosClient client, string databaseName)
        {
            Guard.IsNotNull(nameof(client), client);
            Guard.IsNotNullOrEmpty(nameof(databaseName), databaseName);

            _client = client;
            _databaseName = databaseName;
        }

        public AzureCosmosV3StorageEngineBuilder UseCollection(Action<CollectionOptionsV3> action)
        {
            Guard.IsNotNull(nameof(action), action);

            action(_collectionOptionsV3);
            return this;
        }

        public AzureCosmosV3StorageEngineBuilder UseLogging(Action<LoggingOptions> action)
        {
            Guard.IsNotNull(nameof(action), action);

            action(_loggingOptions);
            return this;
        }

        public AzureCosmosV3StorageEngineBuilder UseTypeMap(ISerializationTypeMap typeMap)
        {
            Guard.IsNotNull(nameof(typeMap), typeMap);
            _typeMap = typeMap;

            return this;
        }

        public AzureCosmosV3StorageEngineBuilder UseJsonSerializerSettings(JsonSerializerSettings settings)
        {
            Guard.IsNotNull(nameof(settings), settings);
            _jsonSerializerSettings = settings;
            return this;
        }

        public AzureCosmosV3StorageEngineBuilder UseDatabase(Action<DatabaseOptions> action)
        {
            Guard.IsNotNull(nameof(action), action);

            action(_databaseOptions);
            return this;
        }

        public IStorageEngine Build()
        {
            return new AzureCosmosV3StorageEngine(_client, 
                _databaseName, 
                _collectionOptionsV3,
                _databaseOptions,
                _loggingOptions,
                _typeMap,
                JsonSerializer.Create(_jsonSerializerSettings));
        }
    }
}
