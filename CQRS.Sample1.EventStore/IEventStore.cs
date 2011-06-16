using System;
using System.Collections.Generic;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.EventStore
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEventsForAggregate(Guid id);
        void SaveEvents(Guid id, IEnumerable<Event> events, int expectedVersion);
    }
}
