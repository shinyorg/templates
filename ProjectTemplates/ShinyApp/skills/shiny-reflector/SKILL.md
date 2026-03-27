---
name: shiny-reflector
description: Generate code using Shiny Reflector for AOT-compliant, source-generated property access, JSON serialization, and assembly info generation in .NET applications
auto_invoke: true
triggers:
  - reflector
  - reflection
  - ReflectorAttribute
  - IReflectorClass
  - IHasReflectorClass
  - GetReflector
  - PropertyGeneratedInfo
  - ReflectorJsonConverter
  - assembly info
  - AssemblyInfo
  - ReflectorItem
  - Shiny.Reflector
  - source generated reflection
  - property access
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
