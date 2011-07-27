using System.Web.Mvc;
using System.Web.Routing;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Events;
using CQRS.Sample1.EventStore;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.WebMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Products", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            // ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory());

            var serviceBus = new FakeServiceBus();
            IoCManager.InjectInstance<IServiceBus>(serviceBus);
            IoCManager.InjectInstance<IRepository>(new RavenRepository());
            IoCManager.InjectInstance<IEventStore>(new RavenEventStore(serviceBus));

            // Command handlers
            var productListCommandHandlers = new ProductListCommandHandlers();
            serviceBus.SubscribeCommandHandler<ProductRenaming>(productListCommandHandlers);
            serviceBus.SubscribeCommandHandler<ProductCreation>(productListCommandHandlers);

            // Event handlers
            var productListEventHandlers = new ProductsHandlers();
            serviceBus.SubscribeEventHandler<ProductRenamed>(productListEventHandlers);
            serviceBus.SubscribeEventHandler<ProductCreated>(productListEventHandlers);
            
        }
    }
}