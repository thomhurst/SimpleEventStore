using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace SimpleEventStore.AzureCosmosV3.Tests
{
    internal static class CosmosClientFactory
    {
        internal static CosmosClient Create()
        {
            return Create(new CosmosSerializationOptions());
        }

        internal static CosmosClient Create(CosmosSerializationOptions serializationOptions)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var documentDbUri = config["Uri"];
            var authKey = config["AuthKey"];

            return new CosmosClient(documentDbUri, authKey, new CosmosClientOptions
            {
                SerializerOptions = serializationOptions
            });
        }
    }
}