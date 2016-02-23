# JavaScriptViewEngine

An ASP.NET MVC (Currently, only MVC6) ViewEngine for rendering markup in a javascript environment. Ideal for React and Angular server-side rendering.

# Why?

The main drive behind this is to support isomorphic/universal rendering. The idea is that your ```Model``` will be passed to a javascript method that that will render markup in return. Imagine having a react component tree that is hydrated via a single immutable JSON structure, representing the initial state of the service-side rendered page.

There were existing projects out there that allowed us to render javascript. All of them had their issues.

- NodeServices - https://github.com/aspnet/NodeServices
  - pros
    - Supports ASP.NET 5 (ASP.NET Core 1)
    - .NET Core (Windows/Linux/Mac)
  - cons
    - Bad performance, local node.exe (with REST endpoints)
- React.NET - https://github.com/reactjs/React.NET
  - pros
    - Embedded javascript engine
    - Fast
  - cons
    - Narrow focus (only React, not Angular)
    - Opinionated
    - No .NET Core support.

JavaScriptViewEngine solves the cons by:
- ~~Bad performance, local node.exe (with REST endpoints)~~ Using a custom VroomJs implementation that is embedded into the process.
- ~~Narrow focus (only React, not Angular)~~ This is just a simple view engine. You can do anything with it.
- ~~Opinionated~~ Again, just a view engine. The React and Angular start templates (TBD) are just that, starter templates. No opinions. Just low friction server-side javascript, exactly how you want it (gulp/grunt/webpack/browserify/babel/etc/etc).
- ~~No .NET Core support.~~ Using a custom VroomJs implementation that uses the ```project.json``` format, we can ran this on .NET Core.

**TO BE DONE**
- [ ] Fix ```dnxcore50``` support in the VroomJs dependency. It has many peices that need to be ```#ifdef```'d or updated to user the .NET API.
- [ ] Implement pooling for ```IJsEngine``` instances. Currently, we are creating and disposing of contexts for each request thread. This really becomes an issue if you have many ```npm``` modules on your server.
  - [ ] File watching for each ```IJsEngine``` in the pool to update the instances with any changes to local scripts. This is ideal for development.
- [ ] Support older versions of MVC. The older versions aren't really condusive to _gulp_y environments, but it is nice to have the support there in case anybody needs it.
- [ ] Create "starter-kits" for getting started with both React and Angular.

# In a nutshell

Getting started is pretty simple.

1. Add a reference to the ```JavaScriptViewEngine``` NuGet package.
2. Setup things app in your ```Startup.cs```.
```c#
public class Startup
{
    public Startup(IHostingEnvironment env)
    {
        ...
        VroomJs.AssemblyLoader.EnsureLoaded();
        ...
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddMvc();
        services.Configure<MvcViewOptions>(options => {
            options.ViewEngines.Clear(); // no razor engine
            options.ViewEngines.Add(new JsViewEngine());
        });
        services.AddJsEngine<JsEngineInitializer>();
        ...
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        ...
        app.UseJsEngine(); // this needs to be before MVC

        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        });
        ...
    }
}
```
3. Create an ```IJsEngineInitializer``` that will populate each engine with the runtime needed to render your data.
```c#
public class JsEngineInitializer : IJsEngineInitializer
{
    public void Initialize(IJsEngine engine)
    {
        engine.Execute(@"
            var RenderView = function(path, model) {
                return ""<html><head></head><body><strong>"" + model.Greeting + ""</strong ></body>"";
            };

            var RenderPartialView = function(path, model) {
                return ""<div><strong>"" + model.Greeting + ""</strong></div>"";
            };
        ");
    }
}
```
4. Get rolling in MVC.
```c#
public class HomeController : Controller
{
    public IActionResult Index(string greeting = "Hello word!")
    {
        return View(new GreetingViewModel { Greeting = greeting });
    }
}

public class GreetingViewModel
{
    public string Greeting { get; set; }
}
```
