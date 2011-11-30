using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;

namespace CQRS.Sample1.Shared
{
    public class RavenRepository : IRepository, IDisposable
    {
        private IDocumentStore _store;
        private IDocumentSession _cachedSession;
        private Task _cachedTask;
        private object _syncLock = new object();

        #region Ctor

        public RavenRepository()
        {
            //_store = new EmbeddableDocumentStore
            //             {
            //                 DataDirectory = @"Data",
            //                 UseEmbeddedHttpServer = true
            //             }.Initialize();
            _store = new DocumentStore
                            {
                                Url = "http://localhost:8080/"
                            }.Initialize();
        }

        #endregion
        
        #region IRepository

        public Task Put<T>(T instance)
        {
            lock (_syncLock)
            {
                if (_cachedSession == null)
                {
                    _cachedSession = _store.OpenSession();

                    _cachedTask = new TaskFactory().StartNew(
                        () =>
                            {
                                Thread.Sleep(2000);

                                lock (_syncLock)
                                {
                                    _cachedSession.SaveChanges();
                                    _cachedSession.Dispose();
                                    _cachedSession = null;
                                }
                            }
                        );
                }

                _cachedSession.Store(instance);

                return _cachedTask;
            }
        }
        
        public T Get<T>(string id)
        {
            using (var session = _store.OpenSession())
            {
                // ACID: Store command, reads committed
                return session.Load<T>(id);
            }
        }

        public IEnumerable<T> Get<T>()
        {
            using (var session = _store.OpenSession())
            {
                // BASE: Reads indexed (might be stale)
                return session.Advanced.LuceneQuery<T>();
            }
        }

        public int PagedGet<T>(Action<IEnumerable<T>> pageHandler, int pageSize = 128)
        {
            using (var session = _store.OpenSession())
            {
                // BASE: Reads indexed (might be stale)
                int pages = 0;
                for (int i=0; i<session.Query<T>().Count(); i+=pageSize)
                {
                    pageHandler(session.Query<T>().Skip(i).Take(pageSize).ToList());
                    pages++;
                }

                return pages;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _store.Dispose();
        }

        #endregion
    }
}
