using System;
using CQRS.Sample1.Domain;

namespace CQRS.Sample1.EventStore
{
    public interface IRepository<T>
        where T : AggregateRoot
    {
        T GetById(Guid id);
        void Save(T instance);
    }
}
