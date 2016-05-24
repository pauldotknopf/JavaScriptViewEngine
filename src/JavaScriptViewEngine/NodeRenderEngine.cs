using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Routing;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default implementing of <see cref="IRenderEngine"/> that uses node
    /// </summary>
    /// <seealso cref="IRenderEngine" />
    public class NodeRenderEngine : IRenderEngine
    {
        private INodeServices _nodeServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRenderEngine"/> class.
        /// </summary>
        /// <param name="projectDirectory">The project directory.</param>
        public NodeRenderEngine(string projectDirectory)
        {
            _nodeServices = Configuration.CreateNodeServices(new NodeServicesOptions
            {
                HostingModel = NodeHostingModel.Http,
                ProjectPath = projectDirectory
            });
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
        public Task<RenderResult> Render(string path, object model, dynamic viewBag, RouteValueDictionary routeValues, string area, ViewType viewType)
        {
            return _nodeServices.InvokeExport<RenderResult>(area,
                viewType == ViewType.Full ? "renderView" : "renderPartialView",
                path,
                model,
                viewBag,
                routeValues);
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
