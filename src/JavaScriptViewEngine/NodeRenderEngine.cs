using System;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default implementing of <see cref="IRenderEngine"/> that uses V8
    /// </summary>
    /// <seealso cref="IRenderEngine" />
    public class NodeRenderEngine : IRenderEngine
    {
        public NodeRenderEngine(string projectDirectory)
        {
            
        }

        public Task<RenderResult> Render(string path, object model, dynamic viewBag, ViewType viewType)
        {
            return Task.FromResult(new RenderResult
            {
                Html = "<html><head></head><body>TEST!</body>",
                Status = 200
            });
        }

        public void Dispose()
        {

        }
    }
}
