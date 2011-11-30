using System.Web.Mvc;
using System.Web.Routing;
using CQRS.Sample1.EventStore;
using CQRS.Sample1.Process;
using CQRS.Sample1.Shared;
using CQRS.Sample1.Process.Domains;

namespace CQRS.Sample1.Client.WebMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

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

            IServiceBus serviceBus = new MsmqServiceBus();
            //IServiceBus serviceBus = new FakeServiceBus();
            IRepository repository = new RavenRepository();

            IoCManager.InjectInstance<IServiceBus>(serviceBus);
            IoCManager.InjectInstance<IEventStore>(new RavenEventStore(repository, serviceBus));
            //IoCManager.InjectInstance<IEventStore>(new FakeEventStore(serviceBus));

            ProcessHandler.RegisterHandlers(new ReadOnlyStore(repository), serviceBus); 

            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory());
        }
    }
}