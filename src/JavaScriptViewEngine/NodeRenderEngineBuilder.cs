using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

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

        public NodeRenderEngineBuilder(IHostingEnvironment hostingEnvironment, IOptions<NodeRenderEngineOptions> options)
        {
            _options = options.Value;
            if (string.IsNullOrEmpty(_options.ProjectDirectory))
                _options.ProjectDirectory = hostingEnvironment.WebRootPath;
        }

        public IRenderEngine Build()
        {
            return new NodeRenderEngine(_options);
        }
    }
}
