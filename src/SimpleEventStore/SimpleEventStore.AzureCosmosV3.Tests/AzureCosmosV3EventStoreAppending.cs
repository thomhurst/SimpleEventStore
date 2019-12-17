using System.Threading.Tasks;
using NUnit.Framework;
using SimpleEventStore.Tests;

namespace SimpleEventStore.AzureCosmosV3.Tests
{
    [TestFixture]
    public class AzureDocumentDbEventStoreAppending : EventStoreAppending
    {
        protected override Task<IStorageEngine> CreateStorageEngine()
        {
            return StorageEngineFactory.Create("AppendingTests");
        }
    }
}
