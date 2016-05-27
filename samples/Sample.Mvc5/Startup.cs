using Microsoft.Owin;
using Owin;
using JavaScriptViewEngine;
using JavaScriptViewEngine.Pool;

[assembly: OwinStartupAttribute(typeof(Sample.Mvc5.Startup))]
namespace Sample.Mvc5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var renderPoolOptions = new Options<RenderPoolOptions>(new RenderPoolOptions
            {
            });
            var nodeRenderOptions = new Options<NodeRenderEngineOptions>(new NodeRenderEngineOptions
            {
            });
            var renderEngineBuilder = new NodeRenderEngineBuilder(nodeRenderOptions);
            var renderEnginePool = new RenderEnginePool(renderPoolOptions, renderEngineBuilder, new FileWatcher());
            app.UseJsEngine(new RenderEngineFactory(renderEnginePool));
        }
    }
}
