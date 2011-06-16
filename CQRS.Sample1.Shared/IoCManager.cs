using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS.Sample1.Shared
{
    public static class IoCManager
    {
        private static IDictionary<Type, IList<Type>> _types = new Dictionary<Type, IList<Type>>();
        private static IDictionary<Type, IList<object>> _instances = new Dictionary<Type, IList<object>>();

        public static void InjectType<T>(Type type)
        {
            IList<Type> implementations;
            if (!_types.TryGetValue(typeof(T), out implementations))
            {
                implementations = new List<Type>();
                _types.Add(typeof(T), implementations);
            }
            implementations.Add(type);
        }

        public static void InjectInstance<T>(T instance)
        {
            IList<object> instances;
            if (!_instances.TryGetValue(typeof(T), out instances))
            {
                instances = new List<object>();
                _instances.Add(typeof(T), instances);
            }
            instances.Add(instance);
        }

        public static T Get<T>()
        {
            IList<Type> types = null;
            IList<object> instances = null;

            if (!_types.TryGetValue(typeof(T), out types) && !_instances.TryGetValue(typeof(T), out instances))
            {
                throw new InvalidOperationException("No implementation for this type cannot be found.");
            }

            if (((types != null ? types.Count : 0) + (instances != null ? instances.Count : 0)) > 1)
            {
                throw new InvalidOperationException("Multiple implementations of this type were found, but only one was requested.");
            }

            return types != null && types.Count > 0
                ? (T) Activator.CreateInstance(types.Single())
                : (T) instances.Single();
        }

        public static IEnumerable<T> GetAll<T>()
        {
            IList<Type> types = null;
            IList<object> instances = null;

            if (!_types.TryGetValue(typeof(T), out types) && !_instances.TryGetValue(typeof(T), out instances))
            {
                throw new InvalidOperationException("No implementation for this type cannot be found.");
            }

            return types != null
                       ? instances != null
                             ? types.Select(Activator.CreateInstance).Union(instances).OfType<T>()
                             : types.Select(Activator.CreateInstance).OfType<T>()
                       : instances.OfType<T>();
        }
    }
}
