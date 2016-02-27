using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The type of full being rendered
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// Full request, including "<html></html>"
        /// </summary>
        Full,
        /// <summary>
        /// A small chunk of html
        /// </summary>
        Partial
    }

    /// <summary>
    /// The result of an invocation of a JavaScript engine.
    /// </summary>
    public class ViewInvokeResult
    {
        /// <summary>
        /// The html
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// The response status code. Only used if the ViewType = Full
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Did the JavaScript engine ask us to redirect the user to another location?
        /// </summary>
        public string Redirect { get; set; }
    }

    /// <summary>
    /// The service that actually passes mvc values (model, path, etc) to the javascript engine to render.
    /// </summary>
    public interface IJsEngineInvoker
    {
        /// <summary>
        /// Invokes the engine and returns the result of the invocation.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="type">The type.</param>
        /// <param name="path">The path.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task<ViewInvokeResult> InvokeEngine(IJsEngine engine, ViewType type, string path, ViewContext context);
    }
}
