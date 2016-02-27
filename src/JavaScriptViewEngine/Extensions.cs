using JavaScriptViewEngine.Middleware;
using Microsoft.AspNet.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JavaScriptViewEngine.Pool;
using Microsoft.Extensions.DependencyInjection;

namespace JavaScriptViewEngine
{
    public static class Extensions
    {
        public static void UseJsEngine(this IApplicationBuilder app)
        {
            app.UseMiddleware<JsEngineMiddleware>();
        }

        public static void AddJsEngine<T>(this IServiceCollection services) where T : class, IJsEngineInitializer
        {
            services.AddTransient<IFileWatcher, FileWatcher>();
            services.AddTransient<IJsPool, JsPool>();
            services.AddTransient<IJsEngineInitializer, T>();
            services.AddTransient<IJsEngineBuilder, JsEngineBuilder>();
            services.AddSingleton<IJsEngineFactory, JsEngineFactory>();
        }
    }
}
