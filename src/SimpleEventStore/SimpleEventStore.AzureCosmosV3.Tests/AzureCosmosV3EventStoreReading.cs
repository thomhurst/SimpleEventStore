﻿using System.Threading.Tasks;
using NUnit.Framework;
using SimpleEventStore.Tests;

namespace SimpleEventStore.AzureCosmosV3.Tests
{
    [TestFixture]
    public class AzureDocumentDbEventStoreReading : EventStoreReading
    {
        protected override Task<IStorageEngine> CreateStorageEngine()
        {
            return CosmosV3StorageEngineFactory.Create("ReadingTests");
        }
    }
}