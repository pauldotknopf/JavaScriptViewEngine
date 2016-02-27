using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// Used to build a new/clean instance of a JavaScript engine.
    /// Implement this if you wish to use a custom engine.
    /// </summary>
    public interface IJsEngineBuilder
    {
        /// <summary>
        /// Build a new clean JavaScript engine
        /// </summary>
        /// <returns></returns>
        IJsEngine Build();
    }
}
