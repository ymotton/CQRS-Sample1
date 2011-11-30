using System;
using System.Collections.Generic;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.EventStore
{
    public class RavenEventStore : EventStoreBase
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        public RavenEventStore(IRepository repository, IServiceBus serviceBus) : base(serviceBus)
        {
            _repository = repository;
        }

        protected override IEnumerable<EventDescriptionCollection> GetAllEventDescriptionCollections()
        {
            return _repository.Get<EventDescriptionCollection>();
            //var eventDescriptionCollections = new List<EventDescriptionCollection>();
            //_repository.PagedGet<EventDescriptionCollection>(eventDescriptionCollections.AddRange);
            //return eventDescriptionCollections;
        }

        protected override EventDescriptionCollection GetEventDescriptionCollection(Guid id)
        {
            return _repository.Get<EventDescriptionCollection>(id.ToString());
        }

        protected override void PersistEventDescriptionCollection(EventDescriptionCollection collection)
        {
            _repository.Put(collection);
        }
    }
}
