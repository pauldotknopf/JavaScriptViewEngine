using JavaScriptViewEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Samples.Mvc5
{
    public class MvcApplication : System.Web.HttpApplication
    {
        IRenderEngineFactory _renderEngineFactor;

        public MvcApplication()
        {
            _renderEngineFactor = new SingletonRenderEngineFactory(new NodeRenderEngineBuilder(null, new Options<NodeRenderEngineOptions>(new NodeRenderEngineOptions())));
            BeginRequest += OnBeginRequest;
            EndRequest += OnEndRequest;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            ViewEngines.Engines.Add(new JsViewEngine(new Options<JsViewEngineOptions>(new JsViewEngineOptions())));
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Items["RenderEngine"] = _renderEngineFactor.RequestEngine();
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            _renderEngineFactor.ReturnEngine((IRenderEngine)HttpContext.Current.Items["RenderEngine"]);
        }
    }
}
