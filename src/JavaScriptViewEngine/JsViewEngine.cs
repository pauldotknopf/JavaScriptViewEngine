using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The aspnet view engine that will pass the model to a js engine to render the markup.
    /// </summary>
    public class JsViewEngine : IJsViewEngine
    {
        private readonly IJsEngineInvoker _jsEngineInvoker;
        private readonly JsViewEngineOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsViewEngine" /> class.
        /// </summary>
        /// <param name="jsEngineInvoker">The js engine invoker.</param>
        /// <param name="options">The options.</param>
        public JsViewEngine(IJsEngineInvoker jsEngineInvoker, IOptions<JsViewEngineOptions> options)
        {
            if(jsEngineInvoker == null)
                jsEngineInvoker = new JsEngineInvoker();

            _jsEngineInvoker = jsEngineInvoker;
            _options = options.Value;
        }
        
        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            // TODO: Fix
            return null;
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            // TODO: Fix
            return null;
        }

        /// <summary>
        /// The view that invokes a javascript engine with the model, and writes the output to the response.
        /// </summary>
        /// <seealso cref="IView" />
        public class JsView : IView
        {
            private readonly IJsEngineInvoker _jsEngineInvoker;

            /// <summary>
            /// Initializes a new instance of the <see cref="JsView"/> class.
            /// </summary>
            /// <param name="jsEngineInvoker">The js engine invoker.</param>
            public JsView(IJsEngineInvoker jsEngineInvoker)
            {
                _jsEngineInvoker = jsEngineInvoker;
            }

            /// <summary>
            /// The path that get's sent to the javascript method.
            /// </summary>
            public string Path { get; set; }
            
            /// <summary>
            /// The type of view that is being rendered.
            /// </summary>
            public ViewType ViewType { get; set; }

            /// <summary>
            /// Asynchronously renders the view using the specified <paramref name="context" />.
            /// </summary>
            /// <param name="context">The <see cref="T:Microsoft.AspNet.Mvc.Rendering.ViewContext" />.</param>
            /// <returns>
            /// A <see cref="T:System.Threading.Tasks.Task" /> that on completion renders the view.
            /// </returns>
            public async Task RenderAsync(ViewContext context)
            {
                var jsEngine = context.HttpContext.Request.HttpContext.Items["JsEngine"] as IJsEngine;

                if (jsEngine == null) throw new Exception("Couldn't get IJsEngine from the context request items.");
                
                var result = await _jsEngineInvoker.InvokeEngine(jsEngine, ViewType, Path, context);

                if (ViewType == ViewType.Full)
                {
                    if (!string.IsNullOrEmpty(result.Redirect))
                    {
                        context.HttpContext.Response.Redirect(result.Redirect);
                        return;
                    }
                    context.HttpContext.Response.StatusCode = result.Status;
                    await context.Writer.WriteAsync(result.Html);
                }
                else
                {
                    await context.Writer.WriteAsync(result.Html);   
                }
            }
        }
    }
}
