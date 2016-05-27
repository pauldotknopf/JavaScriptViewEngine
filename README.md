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
    - Too much .NET integration than what is needed
    - Razor is required
- React.NET - https://github.com/reactjs/React.NET
  - pros
    - Embedded javascript engine
    - Fast
  - cons
    - Narrow focus (only React, not Angular)
    - Limited support for libraries
    - Opinionated
    - No .NET Core support.

**TO BE DONE**
- [X] Fix ```dnxcore50``` support in the VroomJs dependency. It has many peices that need to be ```#ifdef```'d or updated to use the newer .NET API.
- [ ] Support older versions of MVC. The older versions aren't really conducive to *gulp*y environments, but it is nice to have the support there in case anybody needs it.
- [ ] Create "starter-kits" for getting started with both ~~React~~ (done, [react-aspnet-boilerplate](react-aspnet-boilerplate)) and Angular.

# In a nutshell

Getting started is pretty simple.

1. Add a reference to the ```JavaScriptViewEngine``` NuGet package.
2. Setup things app in your ```Startup.cs```.
```c#
public class Startup
{
    private readonly IHostingEnvironment _env;

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
```
3. Create ```default.js``` in your ```WebRootPath``` that will be invoked when rendering.
```javascript
module.exports = {
    renderView: function (callback, path, model, viewBag, routeValues) {
        callback(null, {
            html: "<html><head></head><body><p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p></body>",
            status: 200,
            redirect: null
        });
    },
    renderPartialView: function (callback, path, model, viewBag, routeValues) {
        callback(null, {
            html: "<p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p>"
        });
    }
};
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
