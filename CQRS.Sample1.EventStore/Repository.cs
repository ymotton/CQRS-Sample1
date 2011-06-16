using System;
using CQRS.Sample1.Domain;
using CQRS.Sample1.Shared;
using System.Collections.Generic;

namespace CQRS.Sample1.EventStore
{
    public class Repository<T> : IRepository<T>
        where T : AggregateRoot
    {
        private readonly IEventStore _eventStore;

        public Repository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public T GetById(Guid id)
        {
            IEnumerable<Event> history = _eventStore.GetEventsForAggregate(id);
            return (T)Activator.CreateInstance(typeof(T), history);
        }

        public void Save(T instance)
        {
            _eventStore.SaveEvents(instance.Id, instance.GetChanges(), instance.Version);
        }
    }
}
