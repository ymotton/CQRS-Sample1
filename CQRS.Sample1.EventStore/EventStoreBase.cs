using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.EventStore
{
    public abstract class EventStoreBase : IEventStore
    {
        protected class EventDescriptor
        {
            #region Properties

            public Guid Id { get; set; }
            public int Version { get; set; }
            public string MessageType { get; set; }
            public string Message { get; set; }

            #endregion

            public static EventDescriptor Create(Guid id, Event @event)
            {
                return new EventDescriptor()
                {
                    Id = id,
                    Message = SerializationHelper.Serialize(@event),
                    MessageType = @event.GetType().AssemblyQualifiedName,
                    Version = @event.Version
                };
            }
            public Event GetEvent()
            {
                return (Event) SerializationHelper.Deserialize(Type.GetType(MessageType), Message);
            }
        }
        protected class EventDescriptionCollection
        {
            #region Properties

            public string Id { get; set; }
            public int LatestVersion { get; set; }

            private List<EventDescriptor> _items;
            public List<EventDescriptor> Items
            {
                get
                {
                    if (_items == null)
                    {
                        _items = new List<EventDescriptor>();
                    }
                    return _items;
                }
                set { _items = value; }
            }

            #endregion

            #region Ctor

            public EventDescriptionCollection(Guid id)
            {
                Id = "EventDescriptionCollection/" + id;
            }

            #endregion

            public void Add(Guid id, Event @event)
            {
                LatestVersion = @event.Version;
                Items.Add(EventDescriptor.Create(id, @event));
            }
        }

        private readonly IServiceBus _serviceBus;

        #region Ctor

        protected EventStoreBase(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        #endregion

        protected abstract EventDescriptionCollection GetEventDescriptionCollection(Guid id);

        public IEnumerable<Event> GetEventsForAggregate(Guid id)
        {
            var collection = GetEventDescriptionCollection(id);

            return collection != null
                       ? collection.Items.Select(ed => ed.GetEvent()).ToList()
                       : new List<Event>();
        }

        protected abstract void PersistEventDescriptionCollection(EventDescriptionCollection collection);

        public void SaveEvents(Guid id, IEnumerable<Event> events, int expectedVersion)
        {
            var collection = GetEventDescriptionCollection(id);

            if (collection == null)
            {
                collection = new EventDescriptionCollection(id);
            }
            else if (collection.Items.Any() && collection.LatestVersion != expectedVersion)
            {
                throw new ConcurrencyException();
            }

            foreach (var @event in events)
            {
                expectedVersion++;
                @event.Version = expectedVersion;
                collection.Add(id, @event);
            }

            PersistEventDescriptionCollection(collection);

            foreach (var @event in events)
            {
                _serviceBus.Publish(@event);
            }
        }
    }
}
