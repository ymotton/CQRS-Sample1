namespace CQRS.Sample1.Shared
{
    public static class ServiceBus
    {
        public static void SubscribeCommandHandler<T>(IHandle<T> handler) where T : Command
        {
            IoCManager.Get<IServiceBus>().SubscribeCommandHandler(handler);
        }

        public static void SubscribeEventHandler<T>(IHandle<T> handler) where T : Event
        {
            IoCManager.Get<IServiceBus>().SubscribeEventHandler(handler);
        }

        public static void Send<T>(T command) where T : Command
        {
            IoCManager.Get<IServiceBus>().Send(command);
        }

        public static void Publish<T>(T @event) where T : Event
        {
            IoCManager.Get<IServiceBus>().Publish(@event);
        }
    }
}
