using System;
using System.Runtime.Serialization;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    [DataContract]
    public class Event : Message
    {
        public Event(Guid id, int version) : base(id, version) { }
    }
}
