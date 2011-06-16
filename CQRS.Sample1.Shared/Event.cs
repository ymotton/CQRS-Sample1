using System;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    public class Event : Message
    {
        public int Version { get; set; }

        public Event(Guid id) : base(id) { }
    }
}
