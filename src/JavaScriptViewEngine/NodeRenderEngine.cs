using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;
using JavaScriptViewEngine.Utils;
using System;
#if DOTNETCORE
using Microsoft.AspNetCore.Routing;
#else
using System.Web.Routing;
#endif

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default implementing of <see cref="IRenderEngine"/> that uses node
    /// </summary>
    /// <seealso cref="IRenderEngine" />
    public class NodeRenderEngine : IRenderEngine
    {
        private readonly NodeRenderEngineOptions _options;
        private INodeServices _nodeServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRenderEngine" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="options">The options.</param>
        public NodeRenderEngine(IServiceProvider serviceProvider, NodeRenderEngineOptions options)
        {
            _options = options;

            var nodeOptions = new NodeServicesOptions(serviceProvider)
            {
                HostingModel = options.NodeHostingModel,
                ProjectPath = options.ProjectDirectory,
                WatchFileExtensions = options.WatchFileExtensions
            };
            if (options.NodeInstanceOutputLogger != null)
                nodeOptions.NodeInstanceOutputLogger = options.NodeInstanceOutputLogger;

            _nodeServices = NodeServicesFactory.CreateNodeServices(nodeOptions);
        }

        /// <summary>
        /// Perform a render
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="model">The model.</param>
        /// <param name="viewBag">The view bag.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="area">The area.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        public Task<RenderResult> RenderAsync(string path, object model, dynamic viewBag, RouteValueDictionary routeValues, string area, ViewType viewType)
        {
            var moduleName = _options.GetModuleName != null
                ? _options.GetModuleName(path, model, viewBag, routeValues, area, viewType)
                : "default";

            return _nodeServices.InvokeExport<RenderResult>(moduleName,
                viewType == ViewType.Full ? "renderView" : "renderPartialView",
                path,
                model,
                viewBag,
                routeValues,
                area);
        }

        /// <summary>
        /// Perform a render
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="model">The model.</param>
        /// <param name="viewBag">The view bag.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="area">The area.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        public RenderResult Render(string path, object model, dynamic viewBag, RouteValueDictionary routeValues, string area, ViewType viewType)
        {
            return AsyncHelpers.RunSync<RenderResult>(() => RenderAsync(path, model, viewBag, routeValues, area, viewType));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _nodeServices?.Dispose();
            _nodeServices = null;
        }
    }
}
