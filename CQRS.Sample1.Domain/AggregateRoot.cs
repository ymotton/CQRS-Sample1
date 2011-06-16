using System;
using System.Collections.Generic;
using System.Reflection;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Domain
{
    public abstract class AggregateRoot
    {
        #region Fields

        private readonly List<Event> _changes = new List<Event>();

        #endregion

        #region Properties

        public Guid Id { get; private set; }

        public int Version { get; private set; }

        #endregion

        #region Ctors

        protected AggregateRoot(Guid id)
        {
            Id = id;
            Version = -1;
        }
        protected AggregateRoot(IEnumerable<Event> history)
        {
            Restore(history);
        }

        #endregion
        
        private void Restore(IEnumerable<Event> history)
        {
            foreach (Event @event in history)
            {
                Apply(@event, true);
            }
        }

        protected void Apply(Event @event, bool isHistorical = false)
        {
            this.AsDynamic().Handle(@event);

            // If it's a historical event, update the aggregate to its version
            if (isHistorical)
            {
                Version = @event.Version;
            }
            // Else queue the changes, they will be dequeued by the repository
            else
            {
                _changes.Add(@event);
            }
        }

        public IEnumerable<Event> GetChanges()
        {
            return _changes;
        }

        public void ResetChanges()
        {
            _changes.Clear();
        }
    }
}
