using System;
using System.Runtime.Serialization;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    [DataContract]
    public abstract class Command : Message
    {
        protected Command(Guid id, int version) : base(id, version) { }
    }
}
