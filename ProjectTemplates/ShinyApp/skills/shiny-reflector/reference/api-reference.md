# API Reference

## Installation

```bash
dotnet add package Shiny.Reflector
```

The NuGet package includes both the runtime library and the source generator. No additional analyzer package is needed.

## Namespace

All public types are in the `Shiny.Reflector` namespace:
```csharp
using Shiny.Reflector;
```

When implicit usings are enabled, this namespace is automatically imported.

## ReflectorAttribute

Marks a class or record for source generation. The type **must** be declared as `partial`.

```csharp
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ReflectorAttribute : Attribute;
```

### Usage
```csharp
[Reflector]
public partial class MyClass
{
    public string Name { get; set; }
}
```

### Error SHINYREFL001
If `[Reflector]` is applied to a non-partial class, the source generator emits diagnostic `SHINYREFL001`. Fix by adding the `partial` keyword.

## IReflectorClass Interface

The core interface for accessing properties dynamically on a reflected object.

```csharp
public interface IReflectorClass
{
    // The original object being reflected
    object ReflectedObject { get; }

    // Metadata for all public properties
    PropertyGeneratedInfo[] Properties { get; }

    // Get a property value with type casting
    T? GetValue<T>(string key);

    // Set a property value with type validation
    void SetValue<T>(string key, T? value);

    // Indexer for loose-typed access (case-insensitive key)
    object? this[string key] { get; set; }
}
```

### Property Access Behavior
- Property names are **case-insensitive** (`reflector["name"]` == `reflector["Name"]`)
- `GetValue<T>()` throws `InvalidOperationException` if property not found
- `SetValue<T>()` throws `InvalidOperationException` if property not found or type mismatch
- Indexer getter throws `InvalidOperationException` if property not found
- Indexer setter throws `InvalidOperationException` if property not found or has no setter

## IHasReflectorClass Interface

Marker interface auto-implemented on classes with `[Reflector]` attribute.

```csharp
public interface IHasReflectorClass
{
    IReflectorClass Reflector { get; }
}
```

The source generator adds this interface to your class and generates a lazy-initialized `Reflector` property.

## PropertyGeneratedInfo Record

Metadata about a generated property.

```csharp
public record PropertyGeneratedInfo(
    string Name,      // Property name
    Type Type,        // Property type
    bool HasSetter    // Whether the property has a public setter
);
```

### Notes
- For records with primary constructor parameters, `HasSetter` is `false` (init-only)
- For classes, `HasSetter` reflects whether a public setter exists

## Extension Methods (ReflectorClassExtensions)

### GetReflector

Gets the reflector for any object.

```csharp
public static IReflectorClass? GetReflector(
    this object @this,
    bool fallbackToTrueReflection = false
);
```

- Returns the source-generated reflector if the object implements `IHasReflectorClass`
- If `fallbackToTrueReflection` is `true`, creates a `TrueReflectionReflectorClass` for non-attributed types
- Returns `null` if no reflector is available and fallback is disabled

### HasProperty

Checks if a property exists (case-insensitive).

```csharp
public static bool HasProperty(this IReflectorClass @this, string propertyName);
```

### TryGetPropertyInfo

Gets property metadata by name (case-insensitive).

```csharp
public static PropertyGeneratedInfo? TryGetPropertyInfo(
    this IReflectorClass @this,
    string propertyName
);
```

### TryGetValue (object)

Safely gets a property value.

```csharp
public static bool TryGetValue(
    this IReflectorClass @this,
    string propertyName,
    out object? value
);
```

### TryGetValue\<T\>

Safely gets a typed property value.

```csharp
public static bool TryGetValue<T>(
    this IReflectorClass @this,
    string propertyName,
    out T value
);
```

Returns `false` if property doesn't exist or value is not of type `T`.

### TrySetValue (object)

Safely sets a property value with type checking.

```csharp
public static bool TrySetValue(
    this IReflectorClass @this,
    string propertyName,
    object value
);
```

Returns `false` if property doesn't exist or value type doesn't match.

### TrySetValue\<T\>

Safely sets a typed property value.

```csharp
public static bool TrySetValue<T>(
    this IReflectorClass @this,
    string propertyName,
    T value
);
```

Returns `false` if property doesn't exist, has no setter, or type is not assignable.

### ToDictionary

Converts reflector properties to a dictionary.

```csharp
public static IDictionary<string, object> ToDictionary(
    this IReflectorClass @this,
    bool deepFallbackCanUseReflection = false
);
```

## ReflectorJsonConverter\<T\>

System.Text.Json converter for a specific type using reflectors.

```csharp
public class ReflectorJsonConverter<T> : JsonConverter<T> where T : class, new()
{
    public ReflectorJsonConverter(
        bool useSourceGeneratedReflector = true,
        bool fallbackToTrueReflection = true
    );
}
```

### Parameters
- `useSourceGeneratedReflector` - Prefer source-generated reflectors when available
- `fallbackToTrueReflection` - Use true reflection if no source-generated reflector exists

### Behavior
- Serializes all properties from the reflector
- Respects `JsonSerializerOptions.PropertyNamingPolicy`
- Skips unknown properties during deserialization
- Skips read-only properties during deserialization
- Throws `JsonException` on type mismatch or missing reflector

## ReflectorJsonConverter (Factory)

Non-generic factory version that handles any compatible type.

```csharp
public class ReflectorJsonConverter : JsonConverterFactory
{
    public ReflectorJsonConverter(
        bool useSourceGeneratedReflector = true,
        bool fallbackToTrueReflection = true
    );
}
```

### CanConvert Rules
- Only handles reference types (classes, not structs)
- Skips abstract types, strings, and primitives
- Requires a parameterless constructor
- If `fallbackToTrueReflection` is `false`, only converts types with `[Reflector]` attribute

## TypeExtensions

### IsSimpleType

Checks if a type is a simple/primitive type.

```csharp
public static bool IsSimpleType(this Type type);
```

Returns `true` for: primitives, `string`, `int`, `byte`, `short`, `double`, `float`, `decimal`, `DateOnly`, `TimeOnly`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, and enums. Also handles nullable versions of these types.

## MSBuild Configuration

### Properties

| Property | Default | Description |
|----------|---------|-------------|
| `ShinyReflectorUseInternalAccessors` | `false` | Generate `internal` instead of `public` accessors |
| `ShinyReflectorGenerateAssemblyInfo` | `true` | Enable/disable AssemblyInfo class generation |
| `ShinyReflectorAssemblyInfoClassName` | `AssemblyInfo` | Name of the generated static class |
| `ShinyReflectorAssemblyInfoNamespace` | Root namespace | Namespace for the generated class |

### Auto-Captured Build Properties

The following properties are automatically captured as constants in the generated AssemblyInfo class (when they have values):

| Property | Description |
|----------|-------------|
| `Company` | Company name |
| `Title` | Assembly title |
| `Description` | Assembly description |
| `Version` | Package version |
| `Configuration` | Build configuration (Debug/Release) |
| `ApplicationTitle` | Application title (MAUI) |
| `ApplicationId` | Application identifier (MAUI) |
| `ApplicationVersion` | Application version (MAUI) |
| `ApplicationDisplayVersion` | Display version (MAUI) |
| `AssemblyCompany` | Assembly company |
| `AssemblyProduct` | Assembly product |
| `AssemblyCopyright` | Assembly copyright |
| `AssemblyVersion` | Assembly version |
| `AssemblyFileVersion` | File version |
| `AssemblyInformationalVersion` | Informational version |
| `TargetFramework` | Target framework moniker |
| `TargetFrameworkVersion` | Target framework version |
| `Platform` | Build platform |

### Custom Build Items

Add custom constants via `ReflectorItem`:

```xml
<ItemGroup>
    <ReflectorItem Include="MyConstantName"
                   Value="My constant value"
                   Visible="false" />

    <!-- Capture an MSBuild property -->
    <ReflectorItem Include="BuildDate"
                   Value="$(BuildDate)"
                   Visible="false" />
</ItemGroup>
```

### Generated Output

The generator sanitizes property names to valid C# identifiers:
- Only letters, digits, and underscores are kept
- C# keywords are prefixed with `@`
- String values are properly escaped (backslashes, quotes, newlines)

## Source Generation Details

### What Gets Generated

For each class with `[Reflector]`:

1. **`{ClassName}Reflector.g.cs`** - The reflector implementation class
   - Implements `IReflectorClass`
   - Uses switch statements for O(1) property access
   - Case-insensitive property name matching via `ToLower()`

2. **`{ClassName}_ReflectorProperty.g.cs`** - Partial class extension
   - Adds `IHasReflectorClass` interface to the class
   - Adds lazy-initialized `Reflector` property

### Property Detection Rules

- Only **public** properties with getters are included
- For **classes**: checks for public setter
- For **records**: checks that setter is not init-only (`IsExternalInit`)
- `HasSetter` flag is set accordingly

## Troubleshooting

### Error SHINYREFL001: Class must be partial
- **Cause:** `[Reflector]` attribute is on a non-partial class
- **Fix:** Add `partial` keyword to the class declaration

### GetReflector returns null
- Verify `[Reflector]` attribute is applied to the class
- Verify the class is `partial`
- Or pass `fallbackToTrueReflection: true` for non-attributed types

### Properties array is empty
- Only public properties with getters are included
- Check that your properties are `public`
- For records, primary constructor parameters are included as read-only properties

### JSON converter throws "No reflector available"
- Ensure `fallbackToTrueReflection` is `true` on the converter
- Or add `[Reflector]` attribute to the type being serialized

### AssemblyInfo class not generated
- Check that `ShinyReflectorGenerateAssemblyInfo` is not set to `false`
- Verify the Shiny.Reflector NuGet package is installed
- Clean and rebuild the project

### MVVM Toolkit properties not detected
- Ensure `<LangVersion>preview</LangVersion>` is set in the project file
- Use C# partial properties: `public partial string MyProp { get; set; }`
