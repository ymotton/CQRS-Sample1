using System;

namespace CQRS.Sample1.Shared
{
    public interface IHaveIdentifier
    {
        Guid Id { get; }
    }
}
