using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JavaScriptViewEngine.Middleware
{
    /// <summary>
    /// The middleware that adds a <see cref="IRenderEngine"/> to the request items
    /// to be used. After the request, the engine get's either disposed, or added
    /// back to a pool of engines.
    /// </summary>
    public class RenderEngineMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRenderEngineFactory _renderEngineFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderEngineMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="renderEngineFactory">The render engine factory.</param>
        public RenderEngineMiddleware(RequestDelegate next,
            IRenderEngineFactory renderEngineFactory)
        {
            _next = next;
            _renderEngineFactory = renderEngineFactory;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            IRenderEngine engine = null;
            
            try
            {
                engine = _renderEngineFactory.RequestEngine();

                context.Items["RenderEngine"] = engine;

                await _next(context);
            }
            finally
            {
                if (engine != null)
                    _renderEngineFactory.ReturnEngine(engine);
            }
        }
    }
}
