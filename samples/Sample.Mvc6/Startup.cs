using System.Collections.Generic;
using System.IO;
using JavaScriptViewEngine.Pool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JavaScriptViewEngine;

namespace Sample.Mvc6
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsEngine();
            services.Configure<RenderPoolOptions>(options =>
            {
                options.WatchPath = _env.WebRootPath;
                options.WatchFiles = new List<string>
                {
                    Path.Combine(options.WatchPath, "default.js")
                };
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseJsEngine(); // this needs to be before MVC
            
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
