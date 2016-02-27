using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default <see cref="IJsEngineBuilder"/> that uses VroomJs (V8)
    /// </summary>
    /// <seealso cref="JavaScriptViewEngine.IJsEngineBuilder" />
    public class JsEngineBuilder : IJsEngineBuilder
    {
        /// <summary>
        /// Build a new clean JavaScript engine
        /// </summary>
        /// <returns></returns>
        public IJsEngine Build()
        {
            return new VroomJsEngine();
        }
    }
}
