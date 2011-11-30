using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Process.Domains.Products;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process.Domains
{
    public static class ProcessHandler
    {
        private static IDictionary<Type, object> _modelMap = new Dictionary<Type, object>();
        private static IServiceBus _serviceBus;

        public static void RegisterHandlers(IReadOnlyStore readOnlyStore, IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;

            RegisterCommandHandler(new ProductListCommandHandlers());
            RegisterEventHandler(new ProductListEventHandlers(readOnlyStore));
        }
        private static void RegisterCommandHandler(CommandHandlerBase commandHandler)
        {
            RegisterMessageHandler("SubscribeCommandHandler", commandHandler);
        }
        private static void RegisterEventHandler(IEventHandler eventHandler)
        {
            object instance = eventHandler.Model;
            _modelMap.Add(instance.GetType(), instance);

            RegisterMessageHandler("SubscribeEventHandler", eventHandler);
        }
        private static void RegisterMessageHandler(string handlerSubscription, object messageHandler)
        {
            foreach (Type @interface in messageHandler.GetType()
                                                      .GetInterfaces()
                                                      .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandle<>)))
            {
                _serviceBus.GetType()
                           .GetMethod(handlerSubscription)
                           .MakeGenericMethod(@interface.GetGenericArguments().First())
                           .Invoke(_serviceBus, new[] { messageHandler });
            }
        }

        public static object GetProcessInstance(Type processType)
        {
            Type modelType =
                processType.GetInterfaces()
                           .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHasModel<>))
                           .GetGenericArguments()
                           .First();

            IHasModel instance = (IHasModel)Activator.CreateInstance(processType);
            instance.Initialize(_modelMap[modelType]);

            return instance;
        }
    }
}
