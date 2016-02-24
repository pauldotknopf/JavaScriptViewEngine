using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    public class JsEngineBuilder : IJsEngineBuilder
    {
        public IJsEngine Build()
        {
            return new VroomJsEngine();
        }
    }
}
