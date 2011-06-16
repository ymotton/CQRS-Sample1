using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.Bus
{
    public class FakeServiceBus : IServiceBus
    {
        private static readonly IDictionary<Type, IList<object>> _handlers = new Dictionary<Type, IList<object>>();

        public void SubscribeCommandHandler<T>(IHandle<T> handler) where T : Command
        {
            var handlers = GetHandlerList<T>();
            if (handlers.Count > 0) throw new InvalidOperationException("Each Command can only have one Handler.");
            handlers.Add(handler);
        }
        public void SubscribeEventHandler<T>(IHandle<T> handler) where T : Event
        {
            GetHandlerList<T>().Add(handler);
        }
        private static IList<object> GetHandlerList<T>()
        {
            IList<object> handlers;
            if (!_handlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<object>();
                _handlers.Add(typeof(T), handlers);
            }

            return handlers;
        }

        public void Send<T>(T command) where T : Command
        {
            GetMatchingHandlersOrThrow<T>().First().Handle(command);
        }
        public void Publish<T>(T @event) where T : Event
        {
            foreach (var handler in GetMatchingHandlersOrThrow<T>())
            {
                var capturedHandler = handler;
                ThreadPool.QueueUserWorkItem(
                    (state) =>
                    {
                        // Fake delay to see what's going on
                        Thread.Sleep(5000);
                        capturedHandler.Handle(@event);
                    });
            }
        }
        private static IEnumerable<IHandle<T>> GetMatchingHandlersOrThrow<T>() where T : Message
        {
            IList<object> handlers;
            if (!_handlers.TryGetValue(typeof(T), out handlers) || handlers.Count == 0)
            {
                throw new InvalidOperationException("No Handlers were found for this type of message.");
            }

            return handlers.OfType<IHandle<T>>();
        }
    }
}
