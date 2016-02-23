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
    public interface IJsEngineInvoker
    {
        Task<string> InvokeEngine(IJsEngine engine, ViewType type, string path, ViewContext context);
    }
}
