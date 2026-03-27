---
name: shiny-stores
description: Generate and configure Shiny Stores for .NET - cross-platform key/value stores with persistent service binding for mobile, desktop, and Blazor WebAssembly
auto_invoke: true
triggers:
  - IKeyValueStore
  - IObjectStoreBinder
  - IKeyValueStoreFactory
  - AddShinyStores
  - AddPersistentService
  - ObjectStoreBinderAttribute
  - AddShinyWebAssemblyStores
  - Shiny.Extensions.Stores
  - Shiny.Extensions.Stores.Web
---

# Shiny Stores Skill

You are an expert in Shiny Extensions Stores, a .NET library providing cross-platform key/value store abstraction with persistent service binding.

## When to Use This Skill

Invoke this skill when the user wants to:
- Use cross-platform key/value stores (settings, secure storage, memory)
- Bind `INotifyPropertyChanged` objects to persistent storage
- Use key/value stores in Blazor WebAssembly (localStorage, sessionStorage)
- Create custom key/value store implementations

## Library Overview

**Documentation**: https://shinylib.net/extensions/stores/
**Repository**: https://github.com/shinyorg/Shiny.Extensions
**Packages**: `Shiny.Extensions.Stores`, `Shiny.Extensions.Stores.Web`

## Built-in Store Aliases

| Alias | Platform | Implementation |
|-------|----------|---------------|
| `"memory"` | All | In-memory dictionary |
| `"settings"` | Android | SharedPreferences |
| `"settings"` | iOS/macOS | NSUserDefaults |
| `"settings"` | Windows | ApplicationData.LocalSettings |
| `"settings"` | Blazor | localStorage |
| `"secure"` | Android | EncryptedSharedPreferences |
| `"secure"` | iOS/macOS | Keychain |
| `"session"` | Blazor | sessionStorage |

## Setup

```csharp
// Mobile/Desktop - registers memory, settings, and secure stores
services.AddShinyStores();

// Blazor WebAssembly - adds localStorage and sessionStorage
services.AddShinyWebAssemblyStores();
```

## Using Stores Directly

```csharp
// Via IKeyValueStoreFactory
public class MyService(IKeyValueStoreFactory storeFactory)
{
    public void SaveSetting(string key, string value)
    {
        var store = storeFactory.GetStore("settings");
        store.Set(key, value);
    }

    public T GetSetting<T>(string key, T defaultValue = default)
    {
        var store = storeFactory.DefaultStore;
        return store.Get<T>(key, defaultValue);
    }
}
```

## Persistent Services (Object-Store Binding)

Bind `INotifyPropertyChanged` objects to a store so property changes are automatically persisted:

```csharp
// Register a persistent service
services.AddPersistentService<AppSettings>();                    // Uses default store
services.AddPersistentService<SecureSettings>("secure");         // Uses secure store

// The class
public class AppSettings : INotifyPropertyChanged
{
    string theme = "light";
    public string Theme
    {
        get => theme;
        set { theme = value; OnPropertyChanged(); }
    }

    // ... INotifyPropertyChanged implementation
}
```

You can also target a specific store with the attribute:

```csharp
[ObjectStoreBinder("secure")]
public class SecureSettings : INotifyPropertyChanged
{
    // Properties are automatically persisted to the secure store
}
```

## Store Extension Methods

```csharp
store.Get<T>(key, defaultValue);        // Get with default
store.GetRequired<T>(key);              // Throws if not found
store.SetOrRemove(key, value);          // Removes if value is null
store.SetDefault<T>(key, value);        // Only sets if key doesn't exist
store.IncrementValue(key);              // Thread-safe integer increment
```

## Code Generation Instructions

- Use `AddShinyStores()` for mobile/desktop, `AddShinyWebAssemblyStores()` for Blazor
- Use `AddPersistentService<T>()` for auto-persisting settings classes
- Always implement `INotifyPropertyChanged` for persistent services
- Specify the store alias when targeting secure storage

## Best Practices

1. **Use persistent services** - For app settings that should survive restarts, use `AddPersistentService<T>()`
2. **Target secure store** - Always use the `"secure"` store alias for sensitive data (tokens, credentials)
3. **Use Shiny Reflector** - Mark persistent service classes with `[Reflector]` and make them `partial` to bypass reflection for faster binding
