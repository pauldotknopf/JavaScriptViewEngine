using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

namespace JavaScriptViewEngine
{
    public class JsViewEngine : IViewEngine
    {
        private IJsEngineInvoker _jsEngineInvoker;

        public JsViewEngine(IJsEngineInvoker jsEngineInvoker = null)
        {
            if(jsEngineInvoker == null)
                jsEngineInvoker = new JsEngineInvoker();

            _jsEngineInvoker = jsEngineInvoker;
        }

        public ViewEngineResult FindPartialView(ActionContext context, string partialViewName)
        {
            return ViewEngineResult.Found(partialViewName, new ReactView(_jsEngineInvoker) { ViewType = ViewType.Partial, Path = partialViewName });
        }

        public ViewEngineResult FindView(ActionContext context, string viewName)
        {
            return ViewEngineResult.Found(viewName, new ReactView(_jsEngineInvoker) { ViewType = ViewType.Full, Path = viewName });
        }

        class ReactView : IView
        {
            private IJsEngineInvoker _jsEngineInvoker;

            public ReactView(IJsEngineInvoker jsEngineInvoker)
            {
                _jsEngineInvoker = jsEngineInvoker;
            }

            public string Path { get; set; }
            
            public ViewType ViewType { get; set; }

            public async Task RenderAsync(ViewContext context)
            {
                var jsEngine = context.HttpContext.Request.HttpContext.Items["JsEngine"] as IJsEngine;

                if (jsEngine == null) throw new Exception("Couldn't get IJsEngine from the context request items.");

                var result = await _jsEngineInvoker.InvokeEngine(jsEngine, ViewType, Path, context);
                
                await context.Writer.WriteAsync(result);
            }
        }
    }
}
