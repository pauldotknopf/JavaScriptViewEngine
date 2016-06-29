using System;
using System.IO;
using System.Threading.Tasks;
#if MVCCORE1
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
#else
using System.Web.Mvc;
#endif
#if DI
using Microsoft.Extensions.Options;
#endif

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The aspnet view engine that will pass the model to a render engine to render the markup.
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

        #if MVCCORE1

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            if (!string.IsNullOrEmpty(_options.ViewNamePrefix))
            {
                if (!viewName.StartsWith(_options.ViewNamePrefix))
                    return ViewEngineResult.NotFound(viewName, new string[] { viewName });
            }

            return ViewEngineResult.Found(viewName, new JsView
            {
                Path = !string.IsNullOrEmpty(_options.ViewNamePrefix) ? viewName.Substring(_options.ViewNamePrefix.Length) : viewName,
                ViewType = ViewType.Full
            });
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            if (!string.IsNullOrEmpty(_options.ViewNamePrefix))
            {
                if (!viewPath.StartsWith(_options.ViewNamePrefix))
                    return ViewEngineResult.NotFound(viewPath, new string[] { viewPath });
            }

            return ViewEngineResult.Found(viewPath, new JsView
            {
                Path = !string.IsNullOrEmpty(_options.ViewNamePrefix) ? viewPath.Substring(_options.ViewNamePrefix.Length) : viewPath,
                ViewType = ViewType.Full
            });
        }

        #else

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (!string.IsNullOrEmpty(_options.ViewNamePrefix))
            {
                if (!partialViewName.StartsWith(_options.ViewNamePrefix))
                    return new ViewEngineResult(new string[] { partialViewName });
            }

            return new ViewEngineResult(new JsView
            {
                Path = !string.IsNullOrEmpty(_options.ViewNamePrefix) ? partialViewName.Substring(_options.ViewNamePrefix.Length) : partialViewName,
                ViewType = ViewType.Partial
            }, this);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (!string.IsNullOrEmpty(_options.ViewNamePrefix))
            {
                if (!viewName.StartsWith(_options.ViewNamePrefix))
                    return new ViewEngineResult(new string[] { viewName });
            }

            return new ViewEngineResult(new JsView
            {
                Path = !string.IsNullOrEmpty(_options.ViewNamePrefix) ? viewName.Substring(_options.ViewNamePrefix.Length) : viewName,
                ViewType = ViewType.Full
            }, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {

        }

        #endif

        /// <summary>
        /// The view that invokes a javascript engine with the model, and writes the output to the response.
        /// </summary>
        /// <seealso cref="System.Web.Mvc.IView" />
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

#if MVCCORE1
            
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

                string areaObject;
                context.ActionDescriptor.RouteValues.TryGetValue("area", out areaObject);

                if (areaObject == null)
                {
                    areaObject = "default";
                }

                var result = await renderEngine.RenderAsync(path, context.ViewData.Model, context.ViewBag, context.RouteData.Values, areaObject, ViewType);

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

#else

            public void Render(ViewContext viewContext, TextWriter writer)
            {
                var renderEngine = viewContext.HttpContext.Items["RenderEngine"] as IRenderEngine;
                if (renderEngine == null) throw new Exception("Couldn't get IRenderEngine from the context request items.");

                var path = Path;
                if (string.Equals(path, "{auto}", StringComparison.OrdinalIgnoreCase))
                {
                    path = viewContext.HttpContext.Request.Path;
                    if (viewContext.HttpContext.Request.QueryString != null && viewContext.HttpContext.Request.QueryString.Count > 0)
                    {
                        path += "?" + viewContext.HttpContext.Request.QueryString.ToString();
                    }
                }

                object areaObject;
                viewContext.RouteData.Values.TryGetValue("area", out areaObject);

                if (areaObject == null)
                {
                    areaObject = "default";
                }

                var result = renderEngine.Render(path, viewContext.ViewData.Model, viewContext.ViewBag, viewContext.RouteData.Values, areaObject.ToString(), ViewType);

                if (ViewType == ViewType.Full)
                {
                    if (!string.IsNullOrEmpty(result.Redirect))
                    {
                        viewContext.HttpContext.Response.Redirect(result.Redirect);
                        return;
                    }
                    viewContext.HttpContext.Response.StatusCode = result.Status;
                    writer.Write(result.Html);
                }
                else
                {
                    writer.Write(result.Html);
                }
            }

#endif
        }
    }
}
