# API Reference

## Installation

```bash
dotnet add package Shiny.Extensions.Localization.Generator
dotnet add package Microsoft.Extensions.Localization
```

The NuGet package is a development dependency that includes the Roslyn incremental source generator. It produces no runtime assembly in the consuming project.

## NuGet Packages

| Package | Purpose |
|---------|---------|
| `Shiny.Extensions.Localization.Generator` | Source generator (compile-time only) |
| `Microsoft.Extensions.Localization` | Runtime localization framework |
| `Microsoft.Extensions.DependencyInjection` | DI container (usually included transitively) |

## Generated Types

### {ClassName}Localized

For each `.resx` file, the generator produces a `partial` class named `{ClassName}Localized`:

```csharp
public partial class {ClassName}Localized
{
    readonly IStringLocalizer localizer;

    public {ClassName}Localized(IStringLocalizer<{ClassName}> localizer)
    {
        this.localizer = localizer;
    }

    // Exposes the raw IStringLocalizer for advanced scenarios
    public IStringLocalizer Localizer => this.localizer;

    // For simple strings: generates a property
    public string {PropertyName} => this.localizer["{ResourceKey}"];

    // For format strings: generates a method
    public string {PropertyName}Format(object parameter0, object parameter1, ...)
    {
        return string.Format(this.localizer["{ResourceKey}"], parameter0, parameter1, ...);
    }
}
```

**Key behaviors:**
- Constructor takes `IStringLocalizer<{AssociatedClass}>` for proper resource resolution
- Simple strings (no `{0}` placeholders) → property
- Format strings (with `{0}`, `{1}`, etc.) → method with `Format` suffix
- If the resource key already ends with `Format`, no additional suffix is added
- XML documentation comments are generated from `.resx` `<comment>` elements (falls back to `<value>` if no comment)

### ServiceCollectionExtensions_Generated

A static extension class is generated for DI registration:

```csharp
public static class ServiceCollectionExtensions_Generated
{
    public static IServiceCollection AddStronglyTypedLocalizations(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<{Namespace}.{ClassName}Localized>();
        // ... one line per generated Localized class
        return services;
    }
}
```

**Key behaviors:**
- Calls `AddLocalization()` automatically
- Registers all generated `*Localized` classes as singletons
- Namespace is the project's `RootNamespace` (or `ProjectName` if `RootNamespace` is not set)
- Uses `internal` accessor when `GenerateLocalizersInternal` is `true`

## MSBuild Properties

Configure the generator via MSBuild properties in your `.csproj`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `GenerateLocalizersInternal` | bool | `false` | Generate classes with `internal` instead of `public` access modifier |
| `RootNamespace` | string | (project name) | Root namespace for generated `ServiceCollectionExtensions` class |

### Example Configuration
```xml
<PropertyGroup>
    <GenerateLocalizersInternal>True</GenerateLocalizersInternal>
</PropertyGroup>
```

## Resource Key Transformation

Resource keys from `.resx` files are transformed into valid C# identifiers:

| Character in Key | Replacement |
|-----------------|-------------|
| `.` (dot) | `_` (underscore) |
| `-` (hyphen) | `_` (underscore) |
| ` ` (space) | `_` (underscore) |

**Examples:**
| Resource Key | Generated Property/Method Name |
|-------------|-------------------------------|
| `Welcome` | `Welcome` |
| `Error.Required` | `Error_Required` |
| `my-key` | `my_key` |
| `hello world` | `hello_world` |
| `Greeting` (with `{0}`) | `GreetingFormat(object parameter0)` |
| `MyFormat` (with `{0}`) | `MyFormat(object parameter0)` (no double suffix) |

## Format String Detection

The generator uses regex pattern `\{(\d+)(?::[^}]*)?\}` to detect format parameters:

- `{0}` → detected as parameter 0
- `{1:C2}` → detected as parameter 1 (format specifier ignored for detection)
- `{0:MMM dd yyyy}` → detected as parameter 0
- `{braces}` → NOT detected (non-numeric, treated as plain text)
- `{}` → NOT detected (empty braces)
- Duplicate parameter indices are deduplicated
- Parameters are sorted numerically (0, 1, 2...)

## Namespace Resolution

The generated class namespace is computed from:
1. The project's `RootNamespace` (or `ProjectName` if not set)
2. The relative folder path of the `.resx` file from the project root

**Examples:**
| RootNamespace | File Path | Generated Namespace |
|--------------|-----------|-------------------|
| `MyApp` | `MyClass.resx` | `MyApp` |
| `MyApp` | `Folder1/MyClass.resx` | `MyApp.Folder1` |
| `MyApp` | `Features/Auth/LoginPage.resx` | `MyApp.Features.Auth` |

## File Filtering

The generator only processes `.resx` files that:
- Have exactly one dot in the filename (e.g., `MyClass.resx`)
- Files with multiple dots are skipped (e.g., `MyClass.fr-ca.resx` is a locale variant, not processed)

This ensures only the default locale file generates the strongly-typed class, while locale variants are used at runtime by `IStringLocalizer`.

## Troubleshooting

### Compile error in generated code
- Ensure the associated C# class exists with the same name as the `.resx` file
- Example: `MyViewModel.resx` requires `MyViewModel.cs` in the same namespace

### AddStronglyTypedLocalizations not found
- Ensure `Shiny.Extensions.Localization.Generator` NuGet is installed
- Clean and rebuild the project
- The extension method is in the `RootNamespace` — add the correct `using` directive

### Generated class not appearing
- Verify the `.resx` file is included as `EmbeddedResource` in the project
- The `.props` file included with the package automatically configures `AdditionalFiles`
- Clean and rebuild the project
- Check that the `.resx` file has only one dot in its name (default locale only)

### Wrong namespace on generated class
- Check your `RootNamespace` in `.csproj`
- Namespace is computed from the file's relative path to the project root
- Move the `.resx` file to match your desired namespace structure

### Format method not generated
- Ensure the resource value contains format placeholders like `{0}`, `{1}`
- Non-numeric braces like `{name}` are not recognized as format parameters
- Empty braces `{}` are not recognized

### XML docs showing value instead of comment
- Add `<comment>` elements to your `.resx` data entries
- The generator uses the comment if present, otherwise falls back to the value
