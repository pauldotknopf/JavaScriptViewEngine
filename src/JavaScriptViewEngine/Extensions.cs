using JavaScriptViewEngine.Middleware;
using Microsoft.AspNet.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    public static class Extensions
    {
        public static void UseJsEngine(this IApplicationBuilder app)
        {
            app.UseMiddleware<JsEngineMiddleware>();
        }
    }
}
