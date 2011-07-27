namespace CQRS.Sample1.Shared
{
    public static class ServiceBus
    {
        #region Properties

        private static IServiceBus Bus
        {
            get
            {
                if (_bus == null)
                {
                    _bus = IoCManager.Get<IServiceBus>();
                }

                return _bus;
            }
        }
        private static IServiceBus _bus;

        #endregion

        public static void SubscribeCommandHandler<T>(IHandle<T> handler) where T : Command
        {
            Bus.SubscribeCommandHandler(handler);
        }
        public static void SubscribeEventHandler<T>(IHandle<T> handler) where T : Event
        {
            Bus.SubscribeEventHandler(handler);
        }
        public static void Send<T>(T command) where T : Command
        {
            Bus.Send(command);
        }
        public static void Publish<T>(T @event) where T : Event
        {
            Bus.Publish(@event);
        }
    }
}
