using System;
using System.Diagnostics;
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
        private readonly JsViewEngineOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsViewEngine" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public JsViewEngine(IOptions<JsViewEngineOptions> options)
        {
            _options = options.Value;
        }
        
        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            // TODO: Fix
            return null;
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            return ViewEngineResult.Found(viewPath, new JsView()
            {
                Path = !string.IsNullOrEmpty(_options.ViewNamePrefix) ? viewPath.Substring(_options.ViewNamePrefix.Length) : viewPath,
                ViewType = ViewType.Full
            });
        }

        /// <summary>
        /// The view that invokes a javascript engine with the model, and writes the output to the response.
        /// </summary>
        /// <seealso cref="IView" />
        public class JsView : IView
        {
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
                var renderEngine = context.HttpContext.Request.HttpContext.Items["RenderEngine"] as IRenderEngine;
                if (renderEngine == null) throw new Exception("Couldn't get IRenderEngine from the context request items.");

                var path = Path;
                if (string.Equals(path, "{auto}", StringComparison.OrdinalIgnoreCase))
                {
                    path = context.HttpContext.Request.Path;
                    if (context.HttpContext.Request.QueryString.HasValue)
                    {
                        path += context.HttpContext.Request.QueryString.Value;
                    }
                }
                
                var result = await renderEngine.Render(path, context.ViewData.Model, context.ViewBag, ViewType);

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
