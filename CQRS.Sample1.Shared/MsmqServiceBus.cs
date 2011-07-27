using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;

namespace CQRS.Sample1.Shared
{
    public class MsmqServiceBus : IServiceBus
    {
        #region Properties

        private static MessageQueue EventQueue
        {
            get
            {
                if (_eventQueue == null)
                {
                    _eventQueue = GetOrCreateMessageQueue(@".\Private$\EventQueue");
                    _eventQueue.Formatter = new BinaryMessageFormatter();
                    _eventQueue.ReceiveCompleted +=
                        (o, e) =>
                        {
                            try
                            {
                                System.Messaging.Message message = _eventQueue.EndReceive(e.AsyncResult);

                                Type eventType = Type.GetType(message.Label);
                                Event @event = (Event)message.Body;

                                foreach (var handler in GetMatchingHandlersOrThrow(eventType))
                                {
                                    InvokeHandle(eventType, handler, @event);
                                }
                            }
                            catch (MessageQueueException)
                            {
                                throw;
                            }

                            _eventQueue.BeginReceive();
                        };
                    _eventQueue.BeginReceive();
                }

                return _eventQueue;
            }
        }
        private static MessageQueue _eventQueue;

        private static MessageQueue CommandQueue
        {
            get
            {
                if (_commandQueue == null)
                {
                    _commandQueue = GetOrCreateMessageQueue(@".\Private$\CommandQueue");
                    _commandQueue.Formatter = new BinaryMessageFormatter();
                    _commandQueue.ReceiveCompleted +=
                        (o, e) =>
                        { 
                            try
                            {
                                System.Messaging.Message message = _commandQueue.EndReceive(e.AsyncResult);

                                Type commandType = Type.GetType(message.Label);
                                Command command = (Command)message.Body;

                                var handler = GetMatchingHandlersOrThrow(commandType).First();
                                InvokeHandle(commandType, handler, command);
                            }
                            catch (MessageQueueException)
                            {
                                throw;
                            }

                            _commandQueue.BeginReceive();
                        };
                }

                _commandQueue.BeginReceive();

                return _commandQueue;
            }
        }
        private static MessageQueue _commandQueue;

        private static readonly IDictionary<Type, IList<object>> _handlers = new Dictionary<Type, IList<object>>();

        #endregion

        private static readonly object _syncLock = new object();
        private static MessageQueue GetOrCreateMessageQueue(string path)
        {
            lock (_syncLock)
            {
                if (!MessageQueue.Exists(path))
                {
                    return MessageQueue.Create(path);
                }
            }

            return new MessageQueue(path);
        }

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
            var message =
                new System.Messaging.Message
                {
                    Formatter = new BinaryMessageFormatter(),
                    Body = command,
                    Label = command.GetType().AssemblyQualifiedName
                };

            // Perhaps execute the command asynchronously
            CommandQueue.Send(message);
        }
        public void Publish<T>(T @event) where T : Event
        {
            var message =
                new System.Messaging.Message
                {
                    Formatter = new BinaryMessageFormatter(),
                    Body = @event,
                    Label = @event.GetType().AssemblyQualifiedName
                };
            
            ThreadPool.QueueUserWorkItem(s => EventQueue.Send(message));
        }
        private static void InvokeHandle(Type messageType, object handler, object message)
        {
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    var method = typeof(IHandle<>).MakeGenericType(messageType).GetMethod("Handle");
                    method.Invoke(handler, new[] { message });
                });
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
