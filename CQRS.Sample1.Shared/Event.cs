using System;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    public class Event : Message
    {
        public Event(Guid id) : base(id) { }
    }
}
