using System;
using System.Collections.Generic;

namespace CQRS.Sample1.Shared
{
    public interface IEventStore
    {
        IEnumerable<Event> GetAllEvents();
        IEnumerable<Event> GetEventsForAggregate(Guid id);
        void SaveEvents(Guid id, IEnumerable<Event> events, int expectedVersion);
    }
}
