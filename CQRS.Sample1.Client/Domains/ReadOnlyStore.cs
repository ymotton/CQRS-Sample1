using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using CQRS.Sample1.Client.Domains.Products;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.Domains
{
    public static class ReadOnlyStore
    {
        private static IDictionary<string, object> _keyValueStore;

        static ReadOnlyStore()
        {
            _keyValueStore = new Dictionary<string, object>();

            var products = new List<ProductDetailDto>
                               {
                                   new ProductDetailDto(Guid.NewGuid(), "Article One", 20, null),
                                   new ProductDetailDto(Guid.NewGuid(), "Article Two", 100, null),
                                   new ProductDetailDto(Guid.NewGuid(), "Article Three", 10, null),
                                   new ProductDetailDto(Guid.NewGuid(), "Article Four", 40, null)
                               };
            SetWithId(products.ToArray());

            Set(
                new BindableCollection<ProductDto>(
                    products.Select(p => new ProductDto(p.Id, p.Name))
                )
            );
        }

        public static bool TryGetWithId<T>(Guid id, out T value)
            where T : IHaveIdentifier
        {
            object objectValue;
            if (_keyValueStore.TryGetValue(id.ToString(), out objectValue))
            {
                value = (T)objectValue;
                return true;
            }
            value = default(T);
            return false;
        }
        public static T GetWithId<T>(Guid id)
            where T : IHaveIdentifier
        {
            object objectValue;
            if (_keyValueStore.TryGetValue(id.ToString(), out objectValue))
            {
                return (T)objectValue;
            }
            throw new KeyNotFoundException("The key was not found");
        }
        public static void SetWithId<T>(T value)
            where T : IHaveIdentifier
        {
            _keyValueStore[value.Id.ToString()] = value;
        }
        public static void SetWithId<T>(params T[] values)
            where T : IHaveIdentifier
        {
            foreach (var value in values)
            {
                _keyValueStore[value.Id.ToString()] = value;
            }
        }

        public static bool TryGet<T>(out T value)
        {
            object objectValue;
            if (_keyValueStore.TryGetValue(typeof(T).FullName, out objectValue))
            {
                value = (T)objectValue;
                return true;
            }
            value = default(T);
            return false;
        }
        public static T Get<T>()
        {
            object objectValue;
            if (_keyValueStore.TryGetValue(typeof(T).FullName, out objectValue))
            {
                return (T)objectValue;
            }
            throw new KeyNotFoundException("The key was not found");
        }
        public static void Set<T>(T value)
        {
            _keyValueStore[typeof(T).FullName] = value;
        }
    }
}
