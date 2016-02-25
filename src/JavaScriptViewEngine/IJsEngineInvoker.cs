using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    public enum ViewType
    {
        Full,
        Partial
    }

    public class ViewInvokeResult
    {
        public string Html { get; set; }

        public int Status { get; set; }

        public string Redirect { get; set; }
    }

    public interface IJsEngineInvoker
    {
        Task<ViewInvokeResult> InvokeEngine(IJsEngine engine, ViewType type, string path, ViewContext context);
    }
}
