using JavaScriptViewEngine.Middleware;
using System;
using JavaScriptViewEngine.Pool;
#if DOTNETCORE
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using AppBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;
#else
using Owin;
using AppBuilder = Owin.IAppBuilder;
#endif
#if DI
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
#endif

namespace JavaScriptViewEngine
{
    public static class Extensions
    {
        /// <summary>
        /// Added the middlware that creates and disposes a <see cref="IRenderEngine"/> for each request
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseJsEngine(
            this AppBuilder app
            #if !DOTNETCORE
            , IRenderEngineFactory renderEngineFactory
            #endif
            )
        {
            #if !DOTNETCORE
            app.Use<RenderEngineMiddleware>(renderEngineFactory);
            #else
            app.UseMiddleware<RenderEngineMiddleware>();
            #endif
        }

        #if DI

        /// <summary>
        /// Add the services required to use a render engine, a pool, etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">The services.</param>
        public static void AddJsEngine(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, JavaScriptViewEngineMvcViewOptionsSetup>());
            services.TryAddTransient<IFileWatcher, FileWatcher>();
            services.TryAddTransient<IRenderEnginePool, RenderEnginePool>();
            services.TryAddTransient<IRenderEngineBuilder, NodeRenderEngineBuilder>();
            services.TryAddSingleton<IRenderEngineFactory, RenderEngineFactory>();
            services.TryAddTransient<IJsViewEngine, JsViewEngine>();
        }

        /// <summary>
        /// This will add <see cref="JsViewEngine"/> to a collection used by mvc to render
        /// </summary>
        internal class JavaScriptViewEngineMvcViewOptionsSetup : ConfigureOptions<MvcViewOptions>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="JavaScriptViewEngineMvcViewOptionsSetup"/>.
            /// </summary>
            /// <param name="serviceProvider">The application's <see cref="IServiceProvider"/>.</param>
            public JavaScriptViewEngineMvcViewOptionsSetup(IServiceProvider serviceProvider)
                : base(options => ConfigureMvc(serviceProvider, options))
            {
            }

            /// <summary>
            /// Configures <paramref name="options"/> to use <see cref="JsViewEngine"/>.
            /// </summary>
            /// <param name="serviceProvider">The application's <see cref="IServiceProvider"/>.</param>
            /// <param name="options">The <see cref="MvcViewOptions"/> to configure.</param>
            public static void ConfigureMvc(
                IServiceProvider serviceProvider,
                MvcViewOptions options)
            {
                if (serviceProvider == null)
                    throw new ArgumentNullException(nameof(serviceProvider));

                if (options == null)
                    throw new ArgumentNullException(nameof(options));

                var jsViewEngine = serviceProvider.GetRequiredService<IJsViewEngine>();
                options.ViewEngines.Add(jsViewEngine);
            }
        }

        #endif
    }
}
