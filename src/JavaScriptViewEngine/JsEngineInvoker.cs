using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;

namespace JavaScriptViewEngine
{
    public class JsEngineInvoker : IJsEngineInvoker
    {
        public Task<string> InvokeEngine(IJsEngine engine, ViewType type, string path, ViewContext context)
        {
            return Task.FromResult((string)engine.CallFunction(type == ViewType.Full ? "RenderView" : "RenderPartialView", path, context.ViewData.Model));
        }
    }
}
