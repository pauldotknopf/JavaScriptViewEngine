using Microsoft.AspNetCore.Hosting;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// An <see cref="IRenderEngineBuilder"/> that builds a <see cref="IRenderEngine"/>
    /// that uses node.exe
    /// </summary>
    /// <seealso cref="JavaScriptViewEngine.IRenderEngineBuilder" />
    public class NodeRenderEngineBuilder : IRenderEngineBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public NodeRenderEngineBuilder(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IRenderEngine Build()
        {
            return new NodeRenderEngine(_hostingEnvironment.WebRootPath);
        }
    }
}
