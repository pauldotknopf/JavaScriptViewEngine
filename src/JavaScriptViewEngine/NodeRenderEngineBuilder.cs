using Microsoft.Extensions.Options;
using System;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// An <see cref="IRenderEngineBuilder"/> that builds a <see cref="IRenderEngine"/>
    /// that uses node.exe
    /// </summary>
    /// <seealso cref="JavaScriptViewEngine.IRenderEngineBuilder" />
    public class NodeRenderEngineBuilder : IRenderEngineBuilder
    {
        private readonly NodeRenderEngineOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public NodeRenderEngineBuilder(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, 
            IServiceProvider serviceProvider,
            IOptions<NodeRenderEngineOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            if (string.IsNullOrEmpty(_options.ProjectDirectory))
            {
                _options.ProjectDirectory = hostingEnvironment.WebRootPath;
            }
        }

        public IRenderEngine Build()
        {
            return new NodeRenderEngine(_serviceProvider, _options);
        }
    }
}
