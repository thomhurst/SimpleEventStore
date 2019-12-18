﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SimpleEventStore.AzureDocumentDb;
using SimpleEventStore.Tests.Events;

namespace SimpleEventStore.AzureCosmosV3.Tests
{
    internal static class StorageEngineFactory
    {
        public const string DefaultDatabaseName = "EventStoreTests";

        internal static Task<IStorageEngine> Create(string collectionName, string databaseName = null, Action<AzureCosmosV3StorageEngineBuilder> builderOverrides = null, JsonSerializerSettings settings = null)
        {
            settings = settings ?? new JsonSerializerSettings();
            
            databaseName = databaseName ?? DefaultDatabaseName;

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var consistencyLevel = config["ConsistencyLevel"];

            if (!Enum.TryParse(consistencyLevel, true, out ConsistencyLevel consistencyLevelEnum))
            {
                throw new Exception($"The ConsistencyLevel value {consistencyLevel} is not supported");
            }

            var client = CosmosClientFactory.Create(settings);

            var builder = new AzureCosmosV3StorageEngineBuilder(client, databaseName)
                .UseDatabase(o =>
                {
                    o.DatabaseRequestUnits = TestConstants.RequestUnits;
                })
                .UseCollection(o =>
                {
                    o.CollectionName = collectionName;
                    o.ConsistencyLevel = consistencyLevelEnum;
                    o.CollectionRequestUnits = null;
                })
                .UseTypeMap(new ConfigurableSerializationTypeMap()
                    .RegisterTypes(
                        typeof(OrderCreated).GetTypeInfo().Assembly,
                        t => t.Namespace != null && t.Namespace.EndsWith("Events"),
                        t => t.Name))
                .UseJsonSerializerSettings(settings);

            builderOverrides?.Invoke(builder);

            return builder.Build().Initialise();
        }
    }
}
