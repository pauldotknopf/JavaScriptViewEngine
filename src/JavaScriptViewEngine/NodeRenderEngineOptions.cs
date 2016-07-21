using System;
#if DOTNETCORE
using Microsoft.AspNetCore.Routing;
#else
using System.Web.Routing;
#endif

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The options to use when using the node renderer.
    /// </summary>
    public class NodeRenderEngineOptions
    {
        /// <summary>
        /// The project directory that node will be started in.
        /// If you are using npm modules, the modules must be in
        /// this directory.
        /// </summary>
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Areas in MVC are translated to the file to invoke in node.
        /// "default" is default. If you would like to invoke another
        /// script based on a route value, implement that logic here.
        /// </summary>
        [Obsolete("This isn't used any more. Use 'GetModuleName' instead.", true)]
        public Func<string, string> GetArea = (area) => area;

        /// <summary>
        /// The delegate that determines what node module to invoke the 'RenderView' and 'RenderPartialView' methods from.
        /// </summary>
        public GetModuleNameDelegate GetModuleName = (path, model, viewBag, routeValues, area, viewType) => "default";
    }

    public delegate string GetModuleNameDelegate(string path, object model, dynamic viewBag, RouteValueDictionary routeValues, string area, ViewType viewType);
}
