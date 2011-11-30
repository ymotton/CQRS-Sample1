using System;

namespace CQRS.Sample1.Process
{
    public interface IReadOnlyStore
    {
        object Get(Type instanceType);
        T Get<T>();
        void Put(object instance);
    }
}
