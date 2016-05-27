using JavaScriptViewEngine;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sample.Mvc5
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Add(new JsViewEngine(new Options<JsViewEngineOptions>(new JsViewEngineOptions())));
        }
    }
}
