using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine.Babel
{
    public class BabelEngine : IBabelEngine
    {
        IJsEngine _engine;

        public BabelEngine(IJsEngineBuilder jsEngineBuilder)
        {
            _engine = jsEngineBuilder.Build();
            _engine.ExecuteResource("JavaScriptViewEngine.Babel.Resources.babel.generated.min.js", typeof(IBabelEngine));
        }
        
        public string Transform(string code, BabelConfig config, string fileName = "unknown")
        {
            return _engine.CallFunction<string>("babelTransform", code, config.Serialize(), fileName);
        }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}
