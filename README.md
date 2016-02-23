# JavaScriptViewEngine

An ASP.NET MVC (Currently, only MVC6) ViewEngine for rendering markup in a javascript environment. Ideal for React and Angular server-side rendering.

# Why?

The main drive behind this is to support isomorphic/universal rendering. The idea is that your ```Model``` will be passed to a javascript method that that will render markup in return. Imagine having a react component tree that is hydrated via a single immutable JSON structure, representing the initial state of the service-side rendered page.

There were existing projects our there that allowed us to render javascript. All of them had there issues.

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
