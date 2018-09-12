using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace SimpleEventStore.AzureDocumentDb.Tests
{
    internal static class DocumentClientFactory
    {
        internal static DocumentClient Create(string databaseName)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var documentDbUri = config["COSMOS_URI"];
            var authKey = config["COSMOS_AUTHKEY"];

            return new DocumentClient(new Uri(documentDbUri), authKey);
        }
    }
}