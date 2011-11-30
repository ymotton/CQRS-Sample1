using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading.Tasks;

namespace CQRS.Sample1.Shared
{
    public class MsmqServiceBus : IServiceBus
    {
        #region Properties

        private static MessageQueue EventQueue
        {
            get { return _eventQueue ?? (_eventQueue = GetMessageQueue(@".\Private$\EventQueue")); }
        }
        private static MessageQueue _eventQueue;

        private static MessageQueue CommandQueue
        {
            get { return _commandQueue ?? (_commandQueue = GetMessageQueue(@".\Private$\CommandQueue")); }
        }
        private static MessageQueue _commandQueue;

        private static readonly IDictionary<Type, IList<object>> _handlers = new Dictionary<Type, IList<object>>();

        #endregion

        #region Subscribe Handlers

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

        #endregion

        #region Send Command and Publish Events

        public void Send<T>(T command) where T : Command
        {
            var message =
                new System.Messaging.Message
                {
                    Formatter = new BinaryMessageFormatter(),
                    Body = command,
                    Label = command.GetType().AssemblyQualifiedName,
                    Recoverable = true
                };

            // Only return when we get the ack that the message has been sent to the queue
            CommandQueue.Send(message);
        }
        public void Publish<T>(T @event) where T : Event
        {
            var message =
                new System.Messaging.Message
                {
                    Formatter = new BinaryMessageFormatter(),
                    Body = @event,
                    Label = @event.GetType().AssemblyQualifiedName,
                    Recoverable = true
                };
            
            // Only return when we get the ack that the message has been sent to the queue
            EventQueue.Send(message);
        }

        #endregion

        #region Helpers

        private static readonly object _syncLock = new object();
        private static MessageQueue GetMessageQueue(string path)
        {
            lock (_syncLock)
            {
                if (!MessageQueue.Exists(path))
                {
                    return MessageQueue.Create(path);
                }
            }

            MessageQueue queue = new MessageQueue(path);

            queue.Formatter = new BinaryMessageFormatter();
            queue.ReceiveCompleted += MessageReceiveCompleted;
            queue.BeginReceive();

            return queue;
        }
        private static void MessageReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var queue = (MessageQueue)sender;

            System.Messaging.Message message = queue.EndReceive(e.AsyncResult);
            Type messageType = Type.GetType(message.Label);

            foreach (var handler in GetMatchingHandlersOrThrow(messageType))
            {
                InvokeHandle(messageType, handler, message.Body);
            }

            queue.BeginReceive();
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
        private static void InvokeHandle(Type messageType, object handler, object message)
        {
            var method = typeof(IHandle<>).MakeGenericType(messageType).GetMethod("Handle");
            method.Invoke(handler, new[] { message });
        }

        #endregion
    }
}
