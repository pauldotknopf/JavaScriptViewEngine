using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace JavaScriptViewEngine.Middleware
{
    public class JsEngineMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsEngineFactory _javaScriptEngineFactory;

        public JsEngineMiddleware(RequestDelegate next,
            IJsEngineFactory javaScriptEngineFactory)
        {
            _next = next;
            _javaScriptEngineFactory = javaScriptEngineFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var engine = _javaScriptEngineFactory.GetEngineForCurrentThread();

                context.Items["JsEngine"] = engine;

                await _next(context);
            }
            finally
            {
                _javaScriptEngineFactory.DisposeEngineForCurrentThread();
            }
        }
    }
}
