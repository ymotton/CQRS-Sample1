using System;
using System.Collections.Generic;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.EventStore
{
    public class FakeEventStore : EventStoreBase
    {
        private readonly IDictionary<string, EventDescriptionCollection> _eventStore;

        public FakeEventStore(IServiceBus serviceBus) : base(serviceBus)
        {
            _eventStore = new Dictionary<string, EventDescriptionCollection>();
        }

        protected override EventDescriptionCollection GetEventDescriptionCollection(Guid id)
        {
            EventDescriptionCollection collection;

            if (!_eventStore.TryGetValue("EventDescriptionCollection/" + id, out collection))
            {
                return null;
            }

            return collection;
        }

        private object _syncLock = new object();
        protected override void PersistEventDescriptionCollection(EventDescriptionCollection collection)
        {
            lock (_syncLock)
            {
                if (!_eventStore.ContainsKey(collection.Id))
                {
                    _eventStore.Add(collection.Id, collection);
                }
            }
        }
    }
}
