using JavaScriptViewEngine;
using Microsoft.AspNet.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Mvc6
{
    public class JsEngineInitializer : IJsEngineInitializer
    {
        private IHostingEnvironment _env;

        public JsEngineInitializer(IHostingEnvironment env)
        {
            _env = env;
        }

        public void Initialize(IJsEngine engine)
        {
            engine.ExecuteFile(_env.MapPath("server.js"));
        }
    }
}
