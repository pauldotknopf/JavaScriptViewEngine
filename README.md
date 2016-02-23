# JavaScriptViewEngine

An ASP.NET MVC (Currently, only MVC6) ViewEngine for rendering markup in a javascript environment. Ideal for React and Angular server-side rendering.

# Why?

The main drive behind this is to support isomorphic/universal rendering. The idea is that your ```Model``` will be passed to a javascript method that that will render markup in return. Imagine having a react component tree that is hydrated via a single immutable JSON structure, representing the initial state of the service-side rendered page.

There were existing projects our there that allowed us to render javascript. All of them had there issues.

- https://github.com/aspnet/NodeServices
  - pros
    - Supports ASP.NET 5 (ASP.NET Core 1)
    - .NET Core (Windows/Linux/Mac)
  - cons
    - Still in beta (as of 2/22/2016)
    - Bad performance, local node.exe (with REST endpoints)
- https://github.com/reactjs/React.NET

# What about [ReactJS .NET](http://reactjs.net/)?

