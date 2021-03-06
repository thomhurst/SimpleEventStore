﻿using System;
using Microsoft.Azure.Cosmos;
using NUnit.Framework;

namespace SimpleEventStore.CosmosDb.Tests
{
    [TestFixture]
    public class AzureCosmosDbStorageEngineBuilderTests
    {
        [Test]
        public void when_creating_an_instance_the_document_client_must_be_supplied()
        {
            Assert.Throws<ArgumentNullException>(() => new AzureCosmosDbStorageEngineBuilder(null, "Test"));
        }

        [Test]
        public void when_creating_an_instance_the_database_name_must_be_supplied()
        {
            Assert.Throws<ArgumentException>(() => new AzureCosmosDbStorageEngineBuilder(CreateClient(), null));
        }

        [Test]
        public void when_setting_collection_settings_a_callback_must_be_supplied()
        {
            var builder = new AzureCosmosDbStorageEngineBuilder(CreateClient(), "Test");
            Assert.Throws<ArgumentNullException>(() => builder.UseCollection(null));
        }

        [Test]
        public void when_setting_subscription_settings_a_callback_must_be_supplied()
        {
            var builder = new AzureCosmosDbStorageEngineBuilder(CreateClient(), "Test");
            Assert.Throws<ArgumentNullException>(() => builder.UseCollection(null));
        }

        [Test]
        public void when_setting_logging_settings_a_callback_must_be_supplied()
        {
            var builder = new AzureCosmosDbStorageEngineBuilder(CreateClient(), "Test");
            Assert.Throws<ArgumentNullException>(() => builder.UseLogging(null));
        }

        [Test]
        public void when_setting_the_type_map_it_must_be_supplied()
        {
            var builder = new AzureCosmosDbStorageEngineBuilder(CreateClient(), "Test");
            Assert.Throws<ArgumentNullException>(() => builder.UseTypeMap(null));
        }

        [Test]
        public void when_setting_the_jsonserializationsettings_it_must_be_supplied()
        {
            var builder = new AzureCosmosDbStorageEngineBuilder(CreateClient(), "Test");
            Assert.Throws<ArgumentNullException>(() => builder.UseJsonSerializerSettings(null));
        }

        private static CosmosClient CreateClient()
        {
            return new CosmosClient("https://localhost:8081/", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
        }
    }
}
