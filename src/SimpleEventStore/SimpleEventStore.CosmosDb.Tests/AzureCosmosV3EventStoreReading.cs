using System.Threading.Tasks;
using NUnit.Framework;
using SimpleEventStore.Tests;

namespace SimpleEventStore.CosmosDb.Tests
{
    [TestFixture]
    public class AzureCosmosV3EventStoreReading : EventStoreReading
    {
        protected override Task<IStorageEngine> CreateStorageEngine()
        {
            return CosmosV3StorageEngineFactory.Create("ReadingTests");
        }
    }
}
