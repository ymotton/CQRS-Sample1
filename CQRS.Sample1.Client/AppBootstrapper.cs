using System;
using Caliburn.Micro;
using CQRS.Sample1.EventStore;
using CQRS.Sample1.Process;
using CQRS.Sample1.Process.Domains;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client
{
    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void Configure()
        {
            IServiceBus serviceBus = new MsmqServiceBus();
            //IServiceBus serviceBus = new FakeServiceBus();
            IRepository repository = new RavenRepository();

            IoCManager.InjectInstance<IServiceBus>(serviceBus);
            IoCManager.InjectInstance<IEventStore>(new RavenEventStore(repository, serviceBus));
            //IoCManager.InjectInstance<IEventStore>(new FakeEventStore(serviceBus));

            ProcessHandler.RegisterHandlers(new ReadOnlyStore(repository), serviceBus);

            DispatcherManager.Current = new WpfDispatcher();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == typeof(IWindowManager))
            {
                return new WindowManager();
            }
            
            if (service == typeof(ShellViewModel))
            {
                return new ShellViewModel();
            }

            object instance = ProcessHandler.GetProcessInstance(service);

            if (instance != null)
            {
                return instance;
            }

            return base.GetInstance(service, key);
        }
    }
}
