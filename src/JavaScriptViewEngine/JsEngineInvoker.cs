using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;

namespace JavaScriptViewEngine
{
    public class JsEngineInvoker : IJsEngineInvoker
    {
        public Task<ViewInvokeResult> InvokeEngine(IJsEngine engine, ViewType type, string path, ViewContext context)
        {
            if (type == ViewType.Full)
            {
                var result = (dynamic)engine.CallFunction(type == ViewType.Full ? "RenderView" : "RenderPartialView", path, context.ViewData.Model);
                return Task.FromResult(new ViewInvokeResult {
                    Html = result.html,
                    Status = result.status,
                    Redirect = result.redirect
                });
            }
            if (type == ViewType.Partial)
                return Task.FromResult(new ViewInvokeResult
                {
                    Html = (string)engine.CallFunction(type == ViewType.Full ? "RenderView" : "RenderPartialView", path, context.ViewData.Model)
                });
            throw new Exception("Unknown view type.");
        }
    }
}
