#if DI
using Microsoft.Extensions.Options;
#endif

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

        public NodeRenderEngineBuilder(
            #if DOTNETCORE
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, 
            #endif
            IOptions<NodeRenderEngineOptions> options)
        {
            _options = options.Value;
            if (string.IsNullOrEmpty(_options.ProjectDirectory))
            {
                #if DOTNETCORE
                _options.ProjectDirectory = hostingEnvironment.WebRootPath;
                #else
                _options.ProjectDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
                #endif
            }
        }

        public IRenderEngine Build()
        {
            return new NodeRenderEngine(_options);
        }
    }
}
