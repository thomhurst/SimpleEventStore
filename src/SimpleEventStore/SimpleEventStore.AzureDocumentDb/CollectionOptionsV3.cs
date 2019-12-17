using Microsoft.Azure.Cosmos;

namespace SimpleEventStore.AzureDocumentDb
{
    public class CollectionOptionsV3
    {
        public CollectionOptionsV3()
        {
            this.ConsistencyLevel = ConsistencyLevel.Session;
            this.CollectionRequestUnits = 400;
            this.CollectionName = "Commits";
        }

        public string CollectionName { get; set; }

        public ConsistencyLevel ConsistencyLevel { get; set; }

        public int? CollectionRequestUnits { get; set; }

        public int? DefaultTimeToLive { get; set; }
    }
}