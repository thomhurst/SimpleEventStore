using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;

namespace SimpleEventStore.AzureDocumentDb.ChangeFeedProcessor
{
    public class EventStoreChangeFeedHost : IChangeFeedObserver
    {
        private readonly Func<IReadOnlyCollection<StorageEvent>, string, Task> onNextEvent;
        private readonly ISerializationTypeMap typeMap;

        public EventStoreChangeFeedHost(Func<IReadOnlyCollection<StorageEvent>, string, Task> onNextEvent, ISerializationTypeMap typeMap)
        {
            this.onNextEvent = onNextEvent;
            this.typeMap = typeMap;
        }
        
        public Task OpenAsync(ChangeFeedObserverContext context)
        {
            // TODO: Add logging
            return Task.FromResult(0);
        }

        public Task CloseAsync(ChangeFeedObserverContext context, ChangeFeedObserverCloseReason reason)
        {
            // TODO: Add logging
            return Task.FromResult(0);
        }

        // TODO: Make the callback return a Task
        // TODO: Add error handling & logging
        // TODO: Make skipping dead letters configurable
        public async Task ProcessChangesAsync(ChangeFeedObserverContext context, IReadOnlyList<Document> docs)
        {
            var events = new List<StorageEvent>();
            
            foreach (var d in docs)
            {
                events.Add(DeserializeEventFromDocument(d));
            }
            
            await onNextEvent(events, context.PartitionKeyRangeId);
        }
        
        private StorageEvent DeserializeEventFromDocument(Document doc)
        {
            var docDbStorageEvent = DocumentDbStorageEvent.FromDocument(doc);
            return docDbStorageEvent.ToStorageEvent(typeMap);
        }
    }
}