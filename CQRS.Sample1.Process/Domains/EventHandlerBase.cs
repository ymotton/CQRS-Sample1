using System;
using System.Threading;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process.Domains
{
    public abstract class EventHandlerBase<TModel> : IEventHandler<TModel>
        where TModel : class, IModel
    {
        #region Fields

        private readonly IReadOnlyStore _readOnlyStore;
        private readonly TimeSpan _commitTimeout;
        private DateTime _commitStartTime;

        #endregion

        protected EventHandlerBase(IReadOnlyStore readOnlyStore, TimeSpan? commitTimeout = null)
        {
            _readOnlyStore = readOnlyStore;

            if (!commitTimeout.HasValue)
            {
                commitTimeout = TimeSpan.FromSeconds(10);
            }
            _commitTimeout = commitTimeout.Value; 
            
            Type modelType = typeof(TModel);
            Model = (TModel)_readOnlyStore.Get(modelType);
            if (Model == null)
            {
                Model = (TModel)Activator.CreateInstance(modelType, null);

                Rebuild();

                _readOnlyStore.Put(Model);
            }            
        }

        private void Rebuild()
        {
            var eventStore = IoCManager.Get<IEventStore>();

            foreach (Event @event in eventStore.GetAllEvents())
            {
                this.AsDynamic().Handle(@event);
            }
        }

        protected void ResetCommitTimer()
        {
            _commitStartTime = DateTime.Now + _commitTimeout;
        }
        protected void DelayedCommit()
        {
            if (DateTime.Now < _commitStartTime)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    ResetCommitTimer();

                    while (DateTime.Now < _commitStartTime)
                    {
                        Thread.Sleep(1);
                    }

                    _readOnlyStore.Put(Model);
                });
        }

        #region IEventHandler{TModel} Members

        object IEventHandler.Model { get { return Model; } }
        public TModel Model { get; private set; }

        #endregion

    }
}
