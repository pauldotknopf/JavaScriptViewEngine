using JavaScriptViewEngine;
using Microsoft.AspNetCore.Hosting;

namespace Sample.Mvc6
{
    public class CustomEngineInitializer : IJsEngineInitializer
    {
        private IHostingEnvironment _env;

        public CustomEngineInitializer(IHostingEnvironment env)
        {
            _env = env;
        }

        public void Initialize(IRenderEngine engine)
        {
            // TODO:
            //engine.ExecuteFile(_env.MapPath("server.js"));
        }
    }
}
