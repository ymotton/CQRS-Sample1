using System;
using System.Web.Mvc;
using System.Web.Routing;
using CQRS.Sample1.Process.Domains;

namespace CQRS.Sample1.Client.WebMvc
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            object instance = ProcessHandler.GetProcessInstance(controllerType);

            if (instance != null)
            {
                return (IController) instance;
            }

            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}