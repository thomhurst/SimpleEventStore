using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.ChangeFeedProcessor;

namespace SimpleEventStore.AzureDocumentDb.ChangeFeedProcessor
{
    public class EventStoreChangeFeedHostFactory : IChangeFeedObserverFactory
    {
        private readonly Func<IReadOnlyCollection<StorageEvent>, string, Task> onNextEvent;
        private readonly ISerializationTypeMap typeMap;

        public EventStoreChangeFeedHostFactory(Func<IReadOnlyCollection<StorageEvent>, string, Task> onNextEvent, ISerializationTypeMap typeMap)
        {
            Guard.IsNotNull(nameof(onNextEvent), onNextEvent);
            Guard.IsNotNull(nameof(typeMap), typeMap);
            
            this.onNextEvent = onNextEvent;
            this.typeMap = typeMap;
        }
        
        public IChangeFeedObserver CreateObserver()
        {
            return new EventStoreChangeFeedObserver(onNextEvent, typeMap);
        }
    }
}