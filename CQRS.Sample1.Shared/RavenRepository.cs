using System;
using Raven.Client;
using Raven.Client.Document;

namespace CQRS.Sample1.Shared
{
    public class RavenRepository : IRepository, IDisposable
    {
        private IDocumentStore _store;

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

        public void Put<T>(T instance)
        {
            using (var session = _store.OpenSession())
            {
                session.Store(instance);
                session.SaveChanges();
            }
        }
        
        public T Get<T>(string id)
        {
            using (var session = _store.OpenSession())
            {
                return session.Load<T>(id);
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
