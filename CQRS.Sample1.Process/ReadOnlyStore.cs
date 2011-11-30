using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process
{
    public class ReadOnlyStore : IReadOnlyStore
    {
        #region Fields
        
        private readonly IRepository _repository;

        #endregion

        #region Ctor

        public ReadOnlyStore(IRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region IReadOnlyStore Members

        public object Get(Type instanceType)
        {
            return _repository.Get<object>(instanceType.FullName);
        }

        public T Get<T>()
        {
            return _repository.Get<T>(typeof(T).FullName);
        }

        public void Put(object instance)
        {
            _repository.Put(instance);
        }

        #endregion
    }
}
