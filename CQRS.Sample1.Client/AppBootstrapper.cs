using System;
using System.Linq;
using Caliburn.Micro;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Events;
using CQRS.Sample1.EventStore;
using CQRS.Sample1.Process.Domains.Products;
using CQRS.Sample1.Shared;
using CQRS.Sample1.Process;

namespace CQRS.Sample1.Client
{
    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void Configure()
        {
            var serviceBus = new FakeServiceBus();
            IoCManager.InjectInstance<IServiceBus>(serviceBus);
            IoCManager.InjectInstance<IRepository>(new RavenRepository());
            IoCManager.InjectInstance<IEventStore>(new RavenEventStore(serviceBus));

            // Command handlers
            var productListCommandHandlers = new ProductListCommandHandlers();
            serviceBus.SubscribeCommandHandler<ProductRenaming>(productListCommandHandlers);
            serviceBus.SubscribeCommandHandler<ProductCreation>(productListCommandHandlers);

            // Event handlers
            var productListEventHandlers = new ProductListEventHandlers();
            serviceBus.SubscribeEventHandler<ProductRenamed>(productListEventHandlers);
            serviceBus.SubscribeEventHandler<ProductCreated>(productListEventHandlers);

            DispatcherManager.Current = new WpfDispatcher();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == typeof(IWindowManager))
            {
                return new WindowManager();
            }
            Type baseType = service.BaseType;
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(ExtendedScreen<>))
            {
                Type modelType = baseType.GetGenericArguments().First();
                object instance = ReadOnlyStore.Get(modelType);
                
                if (instance == null)
                {
                    instance = base.GetInstance(modelType, key);
                    ReadOnlyStore.Put(instance);
                }

                return Activator.CreateInstance(service, instance);
            }

            return base.GetInstance(service, key);
        }
    }
}
