using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JavaScriptViewEngine.Middleware
{
    /// <summary>
    /// The middleware that adds a <see cref="IJsEngine"/> to the request items
    /// to be used. After the request, the engine get's either disposed, or added
    /// back to a pool of engines.
    /// </summary>
    public class JsEngineMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsEngineFactory _javaScriptEngineFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsEngineMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="javaScriptEngineFactory">The java script engine factory.</param>
        public JsEngineMiddleware(RequestDelegate next,
            IJsEngineFactory javaScriptEngineFactory)
        {
            _next = next;
            _javaScriptEngineFactory = javaScriptEngineFactory;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            IJsEngine engine = null;

            try
            {
                engine = _javaScriptEngineFactory.GetEngine();

                context.Items["JsEngine"] = engine;

                await _next(context);
            }
            finally
            {
                if (engine != null)
                    _javaScriptEngineFactory.ReturnEngineToPool(engine);
            }
        }
    }
}
