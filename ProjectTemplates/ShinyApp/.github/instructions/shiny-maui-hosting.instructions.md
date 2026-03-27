---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
---

# Shiny MAUI Hosting Skill

You are an expert in Shiny Extensions MAUI Hosting, a .NET library providing modular MAUI app configuration via `IMauiModule` with a static service provider accessor and platform lifecycle hooks.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create MAUI hosting modules with `IMauiModule`
- Use platform lifecycle hooks (`IAppForeground`, `IAppBackground`, etc.)
- Access the service provider via `Host.Services`
- Handle platform-specific events (activity results, universal links)

## Library Overview

**Documentation**: https://shinylib.net/extensions/mauihost/
**Repository**: https://github.com/shinyorg/Shiny.Extensions
**Package**: `Shiny.Extensions.MauiHosting`
**Namespace**: `Shiny`

## IMauiModule Interface

```csharp
public interface IMauiModule
{
    void Add(MauiAppBuilder builder);           // Register services
    void Use(IPlatformApplication app);         // Post-build initialization (do NOT block)
}
```

## Creating MAUI Modules

Each module implements `IMauiModule` with two methods:

- **`Add(MauiAppBuilder builder)`** — register services, configure the builder. Runs before the app is built.
- **`Use(IPlatformApplication app)`** — post-build initialization. `Host.Services` is available here. **Do NOT block** — runs on the main thread.

```csharp
public class AnalyticsModule : IMauiModule
{
    public void Add(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IAnalytics, AppCenterAnalytics>();
    }

    public void Use(IPlatformApplication app)
    {
        var analytics = Host.Services.GetRequiredService<IAnalytics>();
        analytics.TrackEvent("AppStarted");
    }
}
```

## Setup

```csharp
using Shiny;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .AddInfrastructureModules(new AnalyticsModule(), new NetworkModule());

return builder.Build();
```

## Static Host Access

After initialization, `Host.Services` provides access to the service provider from anywhere:

```csharp
var service = Host.Services.GetRequiredService<IMyService>();
```

:::caution
`Host.Services` throws `InvalidOperationException` if accessed before initialization. `Host.Lifecycle` is internal — lifecycle dispatch is handled automatically.
:::

## Platform Lifecycle Hooks

Register services that implement lifecycle interfaces to respond to platform events. The `ILifecycleExecutor` dispatches events to all registered handlers automatically.

### Shared (All Platforms)

```csharp
[Singleton]
public class AppLifecycleHandler : IAppForeground, IAppBackground
{
    public void OnForeground() { /* app came to foreground */ }
    public void OnBackground() { /* app went to background */ }
}
```

| Interface | Purpose |
|-----------|---------|
| `IAppForeground` | App entering foreground |
| `IAppBackground` | App entering background |

### Apple (iOS / macOS)

```csharp
[Singleton]
public class DeepLinkHandler : IContinueActivity
{
    public bool Handle(NSUserActivity activity)
    {
        // Handle universal links, handoff, etc.
        return true;
    }
}
```

| Interface | Purpose |
|-----------|---------|
| `IContinueActivity` | Universal links, handoff |
| `IOnFinishedLaunching` | App finished launching |

### Android

```csharp
[Singleton]
public class PermissionHandler : IOnActivityRequestPermissionsResult
{
    public void Handle(Activity activity, int requestCode, string[] permissions, Permission[] grantResults)
    {
        // Handle permission results
    }
}
```

| Interface | Purpose |
|-----------|---------|
| `IOnApplicationCreated` | Application creation |
| `IOnActivityOnCreate` | Activity creation |
| `IOnActivityRequestPermissionsResult` | Permission request results |
| `IOnActivityNewIntent` | New intent received |
| `IOnActivityResult` | Activity result callback |

## API

```csharp
public static class MauiExtensions
{
    // Register MAUI modules - calls Add() on each, sets up Host initialization
    public static MauiAppBuilder AddInfrastructureModules(
        this MauiAppBuilder builder,
        params IEnumerable<IMauiModule> modules);
}

public class Host : IMauiInitializeService
{
    public static IServiceProvider Services { get; }
}
```

## Code Generation Instructions

- One module per concern (similar to web modules)
- Keep `Add()` for service registration and `Use()` for post-build initialization
- Do NOT block in `Use()` - it runs on the main thread during app startup
- Use `Host.Services` to resolve services after the app is built
- Register lifecycle handlers (`IAppForeground`, `IAppBackground`, etc.) via DI for platform events

## Best Practices

1. **One concern per module** - Separate modules for analytics, networking, auth, etc.
2. **Never block in Use()** - If you need async work, use `Task.Run` or similar
3. **Use Host.Services sparingly** - Prefer constructor injection; use `Host.Services` only where DI is unavailable
4. **Register lifecycle handlers via DI** - Use `[Singleton]` attributes on lifecycle handler classes
