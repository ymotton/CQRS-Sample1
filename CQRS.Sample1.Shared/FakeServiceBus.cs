using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CQRS.Sample1.Shared
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
            var commandType = command.GetType();
            var handler = GetMatchingHandlersOrThrow(commandType).First();
            InvokeHandle(commandType, handler, command);
        }
        public void Publish<T>(T @event) where T : Event
        {
            var eventType = @event.GetType();

            foreach (var handler in GetMatchingHandlersOrThrow(eventType))
            {
                var capturedHandler = handler;

                ThreadPool.QueueUserWorkItem(
                    (state) =>
                    {
                        // Fake delay to see what's going on
                        // Thread.Sleep(5000);
                        InvokeHandle(eventType, capturedHandler, @event);
                    });
            }
        }
        private static void InvokeHandle(Type messageType, object handler, object message)
        {
            var method = typeof(IHandle<>).MakeGenericType(messageType).GetMethod("Handle");
            method.Invoke(handler, new[] { message });
        }
        private static IEnumerable<object> GetMatchingHandlersOrThrow(Type messageType)
        {
            IList<object> handlers;
            if (!_handlers.TryGetValue(messageType, out handlers) || handlers.Count == 0)
            {
                throw new InvalidOperationException("No Handlers were found for this type of message.");
            }

            return handlers;
        }
    }
}
