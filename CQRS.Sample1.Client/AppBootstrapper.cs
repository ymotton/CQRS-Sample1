using Caliburn.Micro;
using CQRS.Sample1.Client.Bus;
using CQRS.Sample1.Client.Domains.Products;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Events;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client
{
    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void Configure()
        {
            var serviceBus = new FakeServiceBus();
            IoCManager.InjectInstance<IServiceBus>(serviceBus);
            
            // Command handlers
            var productListCommandHandlers = new ProductListCommandHandlers();
            serviceBus.SubscribeCommandHandler<ProductRenaming>(productListCommandHandlers);
            serviceBus.SubscribeCommandHandler<ProductCreation>(productListCommandHandlers);

            // Event handlers
            var productListHandlers = new ProductListHandlers();
            serviceBus.SubscribeEventHandler<ProductRenamed>(productListHandlers);
            serviceBus.SubscribeEventHandler<ProductCreated>(productListHandlers);
        }
        protected override object GetInstance(System.Type service, string key)
        {
            if (service ==  typeof(IWindowManager))
            {
                return new WindowManager();
            }

            return base.GetInstance(service, key);
        }
    }
}
