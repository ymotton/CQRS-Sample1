namespace CQRS.Sample1.Shared
{
    public interface IServiceBus 
    {
        void SubscribeCommandHandler<T>(IHandle<T> handler) where T : Command;
        void SubscribeEventHandler<T>(IHandle<T> handler) where T : Event;
        void Send<T>(T command) where T : Command;
        void Publish<T>(T @event) where T : Event;
    }
}
