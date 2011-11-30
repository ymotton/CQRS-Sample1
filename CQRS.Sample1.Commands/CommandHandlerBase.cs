using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Commands
{
    public class CommandHandlerBase
    {
        protected IEventStore EventStore
        {
            get { return _eventStore ?? (_eventStore = IoCManager.Get<IEventStore>()); }
        }
        private IEventStore _eventStore;
    }
}
