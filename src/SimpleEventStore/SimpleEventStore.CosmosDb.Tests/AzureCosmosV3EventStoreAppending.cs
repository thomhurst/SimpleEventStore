using System.Threading.Tasks;
using NUnit.Framework;
using SimpleEventStore.Tests;

namespace SimpleEventStore.CosmosDb.Tests
{
    [TestFixture]
    public class AzureCosmosV3EventStoreAppending : EventStoreAppending
    {
        protected override Task<IStorageEngine> CreateStorageEngine()
        {
            return CosmosV3StorageEngineFactory.Create("AppendingTests");
        }
    }
}
