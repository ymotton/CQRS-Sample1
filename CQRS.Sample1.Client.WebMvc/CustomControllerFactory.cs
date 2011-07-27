using System;
using System.Web.Mvc;
using System.Web.Routing;
using CQRS.Sample1.Process;

namespace CQRS.Sample1.Client.WebMvc
{
    public class CustomControllerFactory : IControllerFactory
    {
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            Type modelType = null;

            object instance = ReadOnlyStore.Get(modelType);
            if (instance == null)
            {
                instance = Activator.CreateInstance(modelType, null);
                ReadOnlyStore.Put(instance);
            }

            return null;
        }

        public void ReleaseController(IController controller)
        {
        }
    }
}