---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
---

# Shiny Reflector Skill

You are an expert in Shiny Reflector, a high-performance reflection library for .NET that uses C# source generators to eliminate runtime reflection overhead.

## When to Use This Skill

Invoke this skill when the user wants to:
- Add source-generated reflection to classes or records
- Access properties dynamically without runtime reflection
- Generate assembly info static classes from build properties
- Use JSON serialization with source-generated reflectors
- Integrate Shiny Reflector with MVVM Community Toolkit
- Migrate from traditional .NET reflection to source-generated reflection
- Configure MSBuild properties for Shiny Reflector

## Library Overview

**Repository**: https://github.com/shinyorg/reflector
**NuGet**: `Shiny.Reflector`
**Namespace**: `Shiny.Reflector`

Shiny Reflector gives you the power of reflection without the actual reflection. It is:
- **AOT compliant** - No runtime reflection introspection needed
- **Fast and zero-allocation** - Uses switch statements instead of reflection
- **Easy to use** - Just add `[Reflector]` attribute and mark class as `partial`

### Core Concepts

| Concept | Description |
|---------|-------------|
| `[Reflector]` attribute | Marks a class/record for source generation |
| `IReflectorClass` | Interface for accessing properties dynamically |
| `IHasReflectorClass` | Marker interface auto-implemented on attributed classes |
| `PropertyGeneratedInfo` | Metadata record for each property (Name, Type, HasSetter) |
| `ReflectorJsonConverter` | System.Text.Json converter using reflectors |
| Assembly Info Generation | Auto-generates static class with build constants |

### Critical: Partial Class Requirement

Classes and records using the `[Reflector]` attribute **must be declared as `partial`**. Without `partial`, you'll get compiler error `SHINYREFL001`:

```csharp
[Reflector]
public partial class MyClass  // partial required!
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

This applies to records as well:
```csharp
[Reflector]
public partial record MyRecord(string Name, int Age);
```

### Basic Setup

**Install NuGet Package:**
```bash
dotnet add package Shiny.Reflector
```

The NuGet package includes both the runtime library and the source generator. No additional package is needed.

## Code Generation Instructions

When generating code using Shiny Reflector, follow these conventions:

### 1. Reflector Classes

Always mark classes as `partial` and apply `[Reflector]`:

```csharp
using Shiny.Reflector;

[Reflector]
public partial class MyModel
{
    public string Name { get; set; }
    public int? Age { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 2. Using the Generated Reflector

Access properties through the generated reflector:

```csharp
var model = new MyModel { Name = "Hello", Age = 25 };
var reflector = model.GetReflector();

// Enumerate properties
foreach (var prop in reflector.Properties)
{
    Console.WriteLine($"{prop.Name} ({prop.Type.Name}) HasSetter={prop.HasSetter}");
}

// Typed access
var name = reflector.GetValue<string>("Name");

// Indexer access (case-insensitive)
var age = reflector["age"];

// Set values
reflector.SetValue("Name", "Updated");
reflector["Age"] = 30;
```

### 3. Fallback to True Reflection

For objects without `[Reflector]`, use the fallback:

```csharp
var reflector = someObject.GetReflector(fallbackToTrueReflection: true);
```

### 4. JSON Serialization

Use `ReflectorJsonConverter` for reflection-free JSON:

```csharp
// Generic converter for a specific type
var options = new JsonSerializerOptions();
options.Converters.Add(new ReflectorJsonConverter<MyModel>());

// Or factory converter for all types
options.Converters.Add(new ReflectorJsonConverter());

var json = JsonSerializer.Serialize(model, options);
var deserialized = JsonSerializer.Deserialize<MyModel>(json, options);
```

### 5. Assembly Info Generation

Configure in the project file:

```xml
<Project>
    <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <ReflectorItem Include="MyCustomVar"
                       Value="Hello World"
                       Visible="false" />
    </ItemGroup>
</Project>
```

This generates:
```csharp
public static class AssemblyInfo
{
    public const string Company = "MyCompany";
    public const string Version = "1.0.0";
    public const string TargetFramework = "net9.0";
    public const string MyCustomVar = "Hello World";
}
```

Usage:
```csharp
Console.WriteLine("Version: " + AssemblyInfo.Version);
Console.WriteLine("Custom: " + AssemblyInfo.MyCustomVar);
```

### 6. MVVM Community Toolkit Integration

When using CommunityToolkit.Mvvm source generation with partial properties:

```xml
<PropertyGroup>
    <LangVersion>preview</LangVersion>
</PropertyGroup>
```

```csharp
[Reflector]
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string MyProperty { get; set; }
}
```

### 7. File Organization

- Models: Place in `Models/` or domain-appropriate folders
- Keep `[Reflector]` classes focused - one per file when possible

## Usage Examples

**Property enumeration:**
```csharp
var reflector = myObject.GetReflector();
foreach (var prop in reflector.Properties)
{
    var value = reflector[prop.Name];
    Console.WriteLine($"{prop.Name}: {value}");
}
```

**Safe property access:**
```csharp
var reflector = myObject.GetReflector();

if (reflector.TryGetValue<string>("Name", out var name))
    Console.WriteLine($"Name = {name}");

if (reflector.HasProperty("Email"))
    reflector["Email"] = "new@example.com";
```

**Dynamic property copying:**
```csharp
var source = sourceObject.GetReflector();
var target = targetObject.GetReflector();

foreach (var prop in source.Properties)
{
    if (target.HasProperty(prop.Name))
        target.TrySetValue(prop.Name, source[prop.Name]);
}
```

## Best Practices

1. **Always use `partial`** - Required for source generation; without it you get `SHINYREFL001`
2. **Prefer source-generated reflectors** - Only use `fallbackToTrueReflection` when working with unattributed types
3. **Use typed access** - Prefer `GetValue<T>()` and `SetValue<T>()` over indexers for type safety
4. **Property names are case-insensitive** - `reflector["name"]` and `reflector["Name"]` are equivalent
5. **Records work too** - Both classes and records support `[Reflector]`, but records must use `partial` keyword
6. **Use `ReflectorJsonConverter` factory** - Register `new ReflectorJsonConverter()` (non-generic) for broad coverage
7. **Assembly info is auto-generated** - Common properties (Company, Version, TargetFramework, etc.) are captured by default

## MSBuild Configuration

| Property | Default | Purpose |
|----------|---------|---------|
| `ShinyReflectorUseInternalAccessors` | `false` | Generate internal vs public accessors |
| `ShinyReflectorGenerateAssemblyInfo` | `true` | Enable/disable AssemblyInfo generation |
| `ShinyReflectorAssemblyInfoClassName` | `AssemblyInfo` | Class name for generated constants |
| `ShinyReflectorAssemblyInfoNamespace` | Root namespace | Namespace for generated AssemblyInfo |

## Reference Files

For detailed templates and examples, see:
- `reference/templates.md` - Code generation templates for reflector classes
- `reference/api-reference.md` - Full API surface, interfaces, and configuration

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

# Shiny Reflector Code Templates

> **Important: Partial Class Requirement**
>
> All classes and records using `[Reflector]` **must be declared as `partial`**. Without `partial`, you'll get compiler error `SHINYREFL001`.

## Basic Reflector Class Template

```csharp
// Models/{Name}.cs
using Shiny.Reflector;

namespace {Namespace}.Models;

[Reflector]
public partial class {Name}
{
    public {PropertyType} {PropertyName} { get; set; }
}
```

## Reflector Record Template

```csharp
// Models/{Name}.cs
using Shiny.Reflector;

namespace {Namespace}.Models;

[Reflector]
public partial record {Name}({PropertyType} {PropertyName});
```

> **Note:** Record properties defined in the primary constructor are read-only (no setter). To have settable properties on records, define them as regular properties in the body.

## Record with Settable Properties Template

```csharp
// Models/{Name}.cs
using Shiny.Reflector;

namespace {Namespace}.Models;

[Reflector]
public partial record {Name}
{
    public {PropertyType} {PropertyName} { get; set; }
    public {ReadOnlyType} {ReadOnlyPropertyName} { get; }
}
```

## Reflector Usage Template

```csharp
// Get reflector from an attributed object
var reflector = myObject.GetReflector();

// Enumerate all properties
foreach (var prop in reflector.Properties)
{
    var value = reflector[prop.Name];
    Console.WriteLine($"Property: {prop.Name} ({prop.Type.Name}) HasSetter={prop.HasSetter} Value={value}");
}

// Typed read
var typedValue = reflector.GetValue<{Type}>("{PropertyName}");

// Typed write
reflector.SetValue("{PropertyName}", newValue);

// Indexer read (case-insensitive)
var objValue = reflector["{propertyName}"];

// Indexer write
reflector["{propertyName}"] = newValue;
```

## Safe Access Template

```csharp
var reflector = myObject.GetReflector();

// Check property exists
if (reflector.HasProperty("{PropertyName}"))
{
    // Safe typed get
    if (reflector.TryGetValue<{Type}>("{PropertyName}", out var value))
    {
        // Use value
    }

    // Safe typed set
    reflector.TrySetValue("{PropertyName}", newValue);
}
```

## Fallback Reflection Template

For objects without the `[Reflector]` attribute:

```csharp
// Get reflector with true reflection fallback
var reflector = someObject.GetReflector(fallbackToTrueReflection: true);
if (reflector != null)
{
    foreach (var prop in reflector.Properties)
    {
        Console.WriteLine($"{prop.Name}: {reflector[prop.Name]}");
    }
}
```

## JSON Serialization Template

### Generic Converter (Single Type)

```csharp
using System.Text.Json;
using Shiny.Reflector;

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
options.Converters.Add(new ReflectorJsonConverter<{TypeName}>());

// Serialize
var json = JsonSerializer.Serialize(myObject, options);

// Deserialize
var deserialized = JsonSerializer.Deserialize<{TypeName}>(json, options);
```

### Factory Converter (All Types)

```csharp
using System.Text.Json;
using Shiny.Reflector;

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

// Handles any class with [Reflector] attribute or parameterless constructor
options.Converters.Add(new ReflectorJsonConverter(
    useSourceGeneratedReflector: true,
    fallbackToTrueReflection: true
));

var json = JsonSerializer.Serialize(myObject, options);
```

## Assembly Info Configuration Template

### Project File Configuration

```xml
<Project>
    <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
        <!-- Optional: customize generated class -->
        <ShinyReflectorAssemblyInfoClassName>AssemblyInfo</ShinyReflectorAssemblyInfoClassName>
        <ShinyReflectorAssemblyInfoNamespace>{Namespace}</ShinyReflectorAssemblyInfoNamespace>
        <!-- Optional: disable assembly info generation -->
        <!-- <ShinyReflectorGenerateAssemblyInfo>false</ShinyReflectorGenerateAssemblyInfo> -->
    </PropertyGroup>

    <ItemGroup>
        <!-- Custom build-time constants -->
        <ReflectorItem Include="{ConstantName}"
                       Value="{ConstantValue}"
                       Visible="false" />

        <!-- Capture MSBuild property as constant -->
        <ReflectorItem Include="{ConstantName}"
                       Value="$({MSBuildPropertyName})"
                       Visible="false" />
    </ItemGroup>
</Project>
```

### Generated Output

```csharp
// Auto-generated static class
public static class AssemblyInfo
{
    public const string Company = "{CompanyValue}";
    public const string Version = "{VersionValue}";
    public const string TargetFramework = "net9.0";
    public const string {ConstantName} = "{ConstantValue}";
}
```

### Usage

```csharp
Console.WriteLine($"App Version: {AssemblyInfo.Version}");
Console.WriteLine($"Target: {AssemblyInfo.TargetFramework}");
Console.WriteLine($"Custom: {AssemblyInfo.{ConstantName}}");
```

## Internal Accessors Template

For library authors who want internal visibility:

```xml
<PropertyGroup>
    <ShinyReflectorUseInternalAccessors>true</ShinyReflectorUseInternalAccessors>
</PropertyGroup>
```

This generates the reflector class and `Reflector` property as `internal` instead of `public`.

## MVVM Community Toolkit Integration Template

```csharp
// ViewModels/{Name}ViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Shiny.Reflector;

namespace {Namespace}.ViewModels;

[Reflector]
public partial class {Name}ViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string {PropertyName} { get; set; }

    [ObservableProperty]
    public partial int {NumericPropertyName} { get; set; }
}
```

> **Note:** Requires `<LangVersion>preview</LangVersion>` in the project file for partial properties.

## Dynamic Property Copying Template

```csharp
// Copy matching properties between two reflector objects
var source = sourceObject.GetReflector();
var target = targetObject.GetReflector();

foreach (var prop in source.Properties)
{
    if (prop.HasSetter || true) // source needs getter (always has)
    {
        var targetProp = target.TryGetPropertyInfo(prop.Name);
        if (targetProp is { HasSetter: true } && prop.Type.IsAssignableTo(targetProp.Type))
        {
            target[prop.Name] = source[prop.Name];
        }
    }
}
```
