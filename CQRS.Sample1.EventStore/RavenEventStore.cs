using System;
using CQRS.Sample1.Shared;
using System.Collections.Generic;

namespace CQRS.Sample1.EventStore
{
    public class RavenEventStore : EventStoreBase
    {
        public RavenEventStore(IServiceBus serviceBus) : base(serviceBus) { }

        protected override IEnumerable<EventDescriptionCollection> GetAllEventDescriptionCollections()
        {
            var repository = IoCManager.Get<IRepository>();

            return null; //repository.Get<EventDescriptionCollection>();
        }

        protected override EventDescriptionCollection GetEventDescriptionCollection(Guid id)
        {
            var repository = IoCManager.Get<IRepository>();

            return repository.Get<EventDescriptionCollection>(id.ToString());
        }

        protected override void PersistEventDescriptionCollection(EventDescriptionCollection collection)
        {
            var repository = IoCManager.Get<IRepository>();

            repository.Put(collection);
        }
    }
}
