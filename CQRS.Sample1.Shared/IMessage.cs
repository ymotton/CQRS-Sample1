using System;

namespace CQRS.Sample1.Shared
{
    public interface IMessage
    {
        Guid Id { get; }
    }
}
