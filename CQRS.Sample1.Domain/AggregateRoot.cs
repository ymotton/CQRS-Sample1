using System;
using System.Collections.Generic;
using CQRS.Sample1.Shared;
using System.Reflection;

namespace CQRS.Sample1.Domain
{
    public abstract class AggregateRoot
    {
        #region Fields

        private readonly List<Event> _changes = new List<Event>();
        private IDictionary<Type, Tuple<MethodInfo, object>> _handlers = new Dictionary<Type, Tuple<MethodInfo, object>>();

        #endregion

        #region Properties

        public Guid Id { get; private set; }

        public int Version { get; protected set; }

        #endregion

        #region Ctors

        protected AggregateRoot(Guid id)
        {
            Id = id;
            Version = -1;

            Initialize();
        }
        protected AggregateRoot(Guid id, IEnumerable<Event> history, int version)
        {
            Id = id;
            Version = version;
            Restore(history);

            Initialize();
        }

        #endregion

        #region Public Members

        public IEnumerable<Event> GetChanges()
        {
            return _changes;
        }

        public void ResetChanges()
        {
            _changes.Clear();
        }

        #endregion

        private void Restore(IEnumerable<Event> history)
        {
            foreach (Event @event in history)
            {
                Apply(@event, true);
            }
        }

        protected abstract void Initialize();
        protected void RegisterHandler<T>(Action<T> handler) where T : Event
        {
            _handlers[typeof(T)] = new Tuple<MethodInfo, object>(handler.Method, handler);
        }

        private void Handle(Event @event)
        {
            Type eventType = @event.GetType();

            Tuple<MethodInfo, object> handlerTuple;
            if (!_handlers.TryGetValue(eventType, out handlerTuple))
            {
                throw new KeyNotFoundException(string.Format("No Handler found for type '{0}'", eventType));
            }

            handlerTuple.Item1.Invoke(this, new[] { @event });
        }
        protected void Apply(Event @event, bool isHistorical = false)
        {
            Handle(@event);

            // If it's a historical event, update the aggregate to its version)
            if (!isHistorical)
            {
                _changes.Add(@event);
            }
        }
    }
}
