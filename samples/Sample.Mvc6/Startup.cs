using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JavaScriptViewEngine;
using System.IO;

namespace Sample.Mvc6
{
    public class Startup :  IJsEngineInitializer
    {
        IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            VroomJs.AssemblyLoader.EnsureLoaded();

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<MvcViewOptions>(options => {
                options.ViewEngines.Clear(); // no razor engine
                options.ViewEngines.Add(new JsViewEngine());
            });

            services.AddSingleton<IJsEngineFactory, JsEngineFactory>();
            services.AddInstance<IJsEngineInitializer>(this);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _env = env;

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseJsEngine(); // this needs to be before MVC

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        public void Initialize(IJsEngine engine)
        {
            engine.ExecuteFile(_env.MapPath("server.js"));
        }
    }
}
