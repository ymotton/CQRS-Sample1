using System;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    public abstract class Command : Message
    {
        protected Command() : this(Guid.NewGuid()) { }
        protected Command(Guid id) : base(id) { }
    }
}
