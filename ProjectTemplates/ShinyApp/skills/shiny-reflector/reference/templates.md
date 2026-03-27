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
