using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default <see cref="IJsEngineInvoker"/>
    /// </summary>
    /// <seealso cref="JavaScriptViewEngine.IJsEngineInvoker" />
    public class JsEngineInvoker : IJsEngineInvoker
    {
        /// <summary>
        /// Invokes the engine and returns the result of the invocation.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="type">The type.</param>
        /// <param name="path">The path.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Unknown view type.</exception>
        public Task<ViewInvokeResult> InvokeEngine(IJsEngine engine, ViewType type, string path, ViewContext context)
        {
            if (type == ViewType.Full)
            {
                if(string.Equals(path, "{auto}", StringComparison.OrdinalIgnoreCase))
                {
                    path = context.HttpContext.Request.Path;
                    if(context.HttpContext.Request.QueryString.HasValue)
                    {
                        path += context.HttpContext.Request.QueryString.Value;
                    }
                }

                var result = engine.CallFunction("RenderView", path, context.ViewData.Model, context.ViewBag);
                return Task.FromResult(new ViewInvokeResult {
                    Html = result.html,
                    Status = result.status,
                    Redirect = result.redirect
                });
            }
            if (type == ViewType.Partial)
                return Task.FromResult(new ViewInvokeResult
                {
                    Html = (string)engine.CallFunction("RenderPartialView", path, context.ViewData.Model, context.ViewBag)
                });
            throw new Exception("Unknown view type.");
        }
    }
}
