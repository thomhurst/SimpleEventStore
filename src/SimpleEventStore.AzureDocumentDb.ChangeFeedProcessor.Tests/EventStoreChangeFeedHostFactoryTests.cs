using System;
using System.Threading.Tasks;
using Xunit;

namespace SimpleEventStore.AzureDocumentDb.ChangeFeedProcessor.Tests
{
    public class EventStoreChangeFeedHostFactoryTests
    {
        [Fact]
        public void when_a_factory_is_created_the_on_next_event_callback_is_required()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStoreChangeFeedHostFactory(null, new DefaultSerializationTypeMap()));
        }

        [Fact]
        public void when_a_factory_is_created_a_serialization_type_map_is_required()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStoreChangeFeedHostFactory((events, checkpoint) => Task.FromResult(0), null));
        }

        [Fact]
        public void when_an_observer_is_created_it_is_an_event_store_change_feed_host()
        {
            var sut = new EventStoreChangeFeedHostFactory((events, checkpoint) => Task.FromResult(0), new DefaultSerializationTypeMap());
            var result = sut.CreateObserver();
            
            Assert.IsType<EventStoreChangeFeedHost>(result);
        }
    }
}