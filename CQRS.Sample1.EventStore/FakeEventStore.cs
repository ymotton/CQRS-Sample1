using System;
using System.Linq;
using System.Collections.Generic;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.EventStore
{
    public class FakeEventStore : IEventStore
    {
        private struct EventDescriptor
        {
            public readonly Guid Id;
            public readonly Event Message;
            public readonly int Version;

            public EventDescriptor(Guid id, Event message, int version)
            {
                Id = id;
                Message = message;
                Version = version;
            }
        }

        private readonly IServiceBus _serviceBus;
        private readonly IDictionary<Guid, IList<EventDescriptor>> _eventStore;

        public FakeEventStore(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
            _eventStore = new Dictionary<Guid, IList<EventDescriptor>>();
        }

        public IEnumerable<Event> GetEventsForAggregate(Guid id)
        {
            IList<EventDescriptor> eventDescriptors;
            if (!_eventStore.TryGetValue(id, out eventDescriptors))
            {
                throw new KeyNotFoundException("id");
            }

            return eventDescriptors.Select(ed => ed.Message);
        }

        public void SaveEvents(Guid id, IEnumerable<Event> events, int expectedVersion)
        {
            IList<EventDescriptor> eventDescriptors;
            if (!_eventStore.TryGetValue(id, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _eventStore[id] = eventDescriptors;
            }
            else if (eventDescriptors.Any() && eventDescriptors.Last().Version != expectedVersion)
            {
                throw new ConcurrencyException();
            }

            foreach (var @event in events)
            {
                expectedVersion++;
                eventDescriptors.Add(new EventDescriptor(id, @event, expectedVersion));
                _serviceBus.Publish(@event);
            }
        }
    }
}
