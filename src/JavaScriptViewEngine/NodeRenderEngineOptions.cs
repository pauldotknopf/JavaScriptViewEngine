using System;
using JavaScriptViewEngine.Pool;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        /// Initializes a new instance of the <see cref="NodeRenderEngineOptions"/> class.
        /// </summary>
        public NodeRenderEngineOptions()
        {
            WatchFileExtensions = new string[] { ".js", ".jsx", ".ts", ".tsx", ".json", ".html" };
            NodeHostingModel = NodeHostingModel.Http;
        }

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

        /// <summary>
        /// The JavaScriptServices node engine has supported for auto-restarting itself when files change.
        /// This is different than the <see cref="IRenderEnginePool" />, which has it's own file watching
        /// and pool recycling.
        /// If you are using the node rendering engine with the <see cref="IRenderEnginePool" />, you should
        /// set this array to empty to avoid the JavaScriptServices for restarting node, and allowing the
        /// engine pool to manage the recycling.
        /// </summary>
        public string[] WatchFileExtensions { get; set; }

        /// <summary>
        /// How should the node instance be invoked remotely?
        /// </summary>
        public NodeHostingModel NodeHostingModel { get; set; }

        public ILogger NodeInstanceOutputLogger { get; set; }
    }

    public delegate string GetModuleNameDelegate(string path, object model, dynamic viewBag, RouteValueDictionary routeValues, string area, ViewType viewType);
}
