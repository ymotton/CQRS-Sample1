using System;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    public abstract class Message : IMessage
    {
        protected Message() : this(Guid.NewGuid()) { }
        protected Message(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}