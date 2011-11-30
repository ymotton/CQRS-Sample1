using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRS.Sample1.Shared
{
    public interface IRepository
    {
        Task Put<T>(T instance);
        T Get<T>(string id);
        IEnumerable<T> Get<T>();
        int PagedGet<T>(Action<IEnumerable<T>> pageHandler, int pageSize = 128);
    }
}
