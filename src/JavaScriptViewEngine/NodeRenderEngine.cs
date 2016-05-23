using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default implementing of <see cref="IRenderEngine"/> that uses V8
    /// </summary>
    /// <seealso cref="IRenderEngine" />
    public class NodeRenderEngine : IRenderEngine
    {
        private INodeServices _nodeServices;

        public NodeRenderEngine(string projectDirectory)
        {
            _nodeServices = Configuration.CreateNodeServices(new NodeServicesOptions
            {
                HostingModel = NodeHostingModel.Http,
                ProjectPath = projectDirectory
            });
        }

        public Task<RenderResult> Render(string path, object model, dynamic viewBag, ViewType viewType)
        {
            return _nodeServices.InvokeExport<RenderResult>("server", 
                viewType == ViewType.Full ? "renderView" : "renderPartialView", 
                path, 
                model, 
                viewBag);
        }

        public void Dispose()
        {
            _nodeServices?.Dispose();
            _nodeServices = null;
        }
    }
}
