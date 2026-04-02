---
name: shiny-di
description: Generate and configure Shiny DI for .NET - attribute-driven service registration with source generators, keyed services, categories, and multi-interface support
auto_invoke: true
triggers:
  - ServiceAttribute
  - SingletonAttribute
  - ScopedAttribute
  - TransientAttribute
  - AddShinyServiceRegistry
  - AddGeneratedServices
  - AddSingletonAsImplementedInterfaces
  - AddScopedAsImplementedInterfaces
  - Shiny.Extensions.DependencyInjection
---

# Shiny Dependency Injection Skill

You are an expert in Shiny Extensions Dependency Injection, a .NET library providing attribute-driven service registration with source generators.

## When to Use This Skill

Invoke this skill when the user wants to:
- Register services using `[Singleton]`, `[Scoped]`, or `[Transient]` attributes with source generation
- Configure the Shiny DI service registry in their application
- Use keyed services, categories, TryAdd semantics
- Register services against multiple interfaces
- Use open generics with DI attributes

## Library Overview

**Documentation**: https://shinylib.net/extensions/di/
**Repository**: https://github.com/shinyorg/Shiny.Extensions
**Package**: `Shiny.Extensions.DependencyInjection`
**Namespace**: `Shiny`

## Registration Attributes

Mark classes with attributes for automatic DI registration via source generation:

```csharp
[Singleton]                                    // Singleton lifetime
public class MyService : IMyService { }

[Scoped]                                       // Scoped lifetime
public class MyRepository : IRepository { }

[Transient]                                    // Transient lifetime
public class MyFactory : IFactory { }
```

### Attribute Properties

All registration attributes support these properties:

| Property | Type | Description |
|----------|------|-------------|
| `AsSelf` | `bool` | Register as the class itself rather than its interface |
| `Type` | `Type` | Register against a specific interface (when class implements multiple) |
| `KeyedName` | `string?` | Register as a keyed service |
| `Category` | `string?` | Category for conditional registration |
| `TryAdd` | `bool` | Use TryAdd semantics (won't replace existing registrations) |

### Examples

```csharp
// Register as the class itself
[Singleton(AsSelf = true)]
public class AppState { }

// Register against a specific interface
[Singleton(Type = typeof(ISpecificInterface))]
public class MultiInterfaceService : ISpecificInterface, IOtherInterface { }

// Keyed service
[Singleton(KeyedName = "primary")]
public class PrimaryCache : ICache { }

// Category-based conditional registration
[Singleton(Category = "premium")]
public class PremiumFeature : IFeature { }

// TryAdd semantics
[Singleton(TryAdd = true)]
public class DefaultLogger : ILogger { }
```

## Setup

```csharp
// Register all source-generated services
builder.Services.AddShinyServiceRegistry();

// Register with specific categories only
builder.Services.AddShinyServiceRegistry("premium", "analytics");
```

## Helper Extensions

```csharp
// Register a singleton against all implemented interfaces
services.AddSingletonAsImplementedInterfaces<MyService>();
services.AddSingletonAsImplementedInterfaces<MyService>("keyName");

// Register scoped against all implemented interfaces
services.AddScopedAsImplementedInterfaces<MyService>();

// Check registrations
bool hasService = services.HasService<IMyService>();
bool hasImpl = services.HasImplementation<MyService>();

// Lazy resolution
Lazy<IMyService> lazy = services.GetLazyService<IMyService>(required: true);
```

## Code Generation Instructions

- Always use attributes (`[Singleton]`, `[Scoped]`, `[Transient]`) over manual registration
- Choose appropriate lifetime: Singleton for stateless services, Scoped for per-request (DbContext), Transient for lightweight factories
- Use `Category` for optional features that may not always be registered
- Use `KeyedName` when multiple implementations of the same interface exist
- Always call `AddShinyServiceRegistry()` to activate source-generated registrations

## Best Practices

1. **Use source generation** - Always prefer `[Singleton]`/`[Scoped]`/`[Transient]` attributes over manual `services.Add*()` calls
2. **Call AddShinyServiceRegistry()** - Required to activate source-generated registrations
3. **Appropriate lifetimes** - Singleton for stateless, Scoped for DbContext/unit-of-work, Transient for factories
4. **Use keyed services** - When multiple implementations exist, use `KeyedName` to disambiguate
