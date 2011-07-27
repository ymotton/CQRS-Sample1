using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process
{
    public static class ReadOnlyStore
    {
        private static IRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = IoCManager.Get<IRepository>();
                }
                return _repository;
            }
        }
        private static IRepository _repository;

        public static object Get(Type instanceType)
        {
            return Repository.Get<object>(instanceType.FullName);
        }

        public static T Get<T>()
        {
            return Repository.Get<T>(typeof(T).FullName);
        }

        public static void Put(object instance)
        {
            Repository.Put(instance);
        }
    }
}
