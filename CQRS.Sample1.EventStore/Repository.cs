using System;
using System.Collections.Generic;
using CQRS.Sample1.Domain;
using CQRS.Sample1.Shared;

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
            if (history == null)
            {
                return null;
            }
            return (T)Activator.CreateInstance(typeof(T), id, history);
        }

        public void Save(T instance)
        {
            _eventStore.SaveEvents(instance.Id, instance.GetChanges(), instance.Version);
        }
    }
}
