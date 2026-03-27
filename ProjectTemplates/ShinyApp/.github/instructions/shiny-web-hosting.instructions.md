---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
---

# Shiny Web Hosting Skill

You are an expert in Shiny Extensions Web Hosting, a .NET library providing modular ASP.NET Core application configuration via `IWebModule`.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create ASP.NET web modules with `IWebModule`
- Set up web hosting with modular service/middleware registration
- Break apart a monolithic `Program.cs` into focused module classes

## Library Overview

**Documentation**: https://shinylib.net/extensions/webhost/
**Repository**: https://github.com/shinyorg/Shiny.Extensions
**Package**: `Shiny.Extensions.WebHosting`
**Namespace**: `Shiny`

## IWebModule Interface

```csharp
public interface IWebModule
{
    void Add(WebApplicationBuilder builder);    // Register services
    void Use(WebApplication app);               // Configure middleware
}
```

## Creating Web Modules

Each module implements `IWebModule` with two methods:

- **`Add(WebApplicationBuilder builder)`** — register services, configuration, and anything before `Build()`
- **`Use(WebApplication app)`** — configure middleware, endpoints, and anything after `Build()`

```csharp
public class SwaggerModule : IWebModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public void Use(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}

public class CorsModule : IWebModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
    }

    public void Use(WebApplication app)
    {
        app.UseCors();
    }
}

public class AuthModule : IWebModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication().AddJwtBearer();
        builder.Services.AddAuthorization();
    }

    public void Use(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
```

## Setup

```csharp
using Shiny;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructureModules(
    new SwaggerModule(),
    new CorsModule(),
    new AuthModule()
);

var app = builder.Build();
app.UseInfrastructureModules();

app.Run();
```

## API

```csharp
public static class WebExtensions
{
    // Register module instances and call Add() on each
    public static WebApplicationBuilder AddInfrastructureModules(
        this WebApplicationBuilder builder,
        params IEnumerable<IWebModule> modules);

    // Call Use() on each module to configure middleware
    public static WebApplication UseInfrastructureModules(
        this WebApplication app,
        params IEnumerable<IWebModule> modules);
}
```

## Code Generation Instructions

- One module per concern (CORS, auth, swagger, health checks, etc.)
- Keep `Add()` for service registration and `Use()` for middleware configuration
- Pass explicit module instances to `AddInfrastructureModules()` — no assembly scanning

## Best Practices

1. **One concern per module** - Create separate modules for CORS, auth, swagger, health checks, etc.
2. **Order matters for Use()** - Middleware order in `Use()` follows the order modules are registered
3. **Fully AOT-compatible** - No reflection or assembly scanning, explicit module registration only
