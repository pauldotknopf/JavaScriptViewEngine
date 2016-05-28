#if MVCCORE1
using Microsoft.AspNetCore.Mvc.ViewEngines;
#else
using System.Web.Mvc;
#endif
namespace JavaScriptViewEngine
{
    /// <summary>
    /// Interface that is just a marker for a <see cref="IViewEngine"/>.
    /// It provides no additional functionality, except to help with IoC containers.
    /// </summary>
    public interface IJsViewEngine : IViewEngine
    {
    }
}
