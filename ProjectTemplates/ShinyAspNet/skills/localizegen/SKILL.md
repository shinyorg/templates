---
name: localizegen
description: Generate strongly-typed localization classes from .resx resource files using Shiny.Extensions.Localization.Generator for .NET applications
auto_invoke: true
triggers:
  - localization
  - localize
  - resx
  - resource file
  - IStringLocalizer
  - StringLocalizer
  - strongly typed localization
  - AddStronglyTypedLocalizations
  - Localized class
  - localizegen
  - Shiny.Extensions.Localization.Generator
  - i18n
  - internationalization
  - translate
  - translation
---

# Shiny Localization Generator Skill

You are an expert in Shiny.Extensions.Localization.Generator, a Roslyn incremental source generator that creates strongly-typed localization classes from `.resx` resource files for use with `Microsoft.Extensions.Localization`.

## When to Use This Skill

Invoke this skill when the user wants to:
- Add localization/internationalization to a .NET application using `.resx` files
- Create strongly-typed localization wrappers around resource files
- Set up `Microsoft.Extensions.Localization` with generated code
- Add new `.resx` resource files with matching classes
- Use format string parameters in localized strings
- Configure public vs internal accessor generation
- Bind localized strings in XAML or Razor views with IntelliSense support

## Library Overview

**NuGet**: `Shiny.Extensions.Localization.Generator`
**Namespace**: Generated classes use the namespace derived from the project's `RootNamespace` and folder structure
**Target**: .NET Standard 2.0 (works with any modern .NET application)

Shiny.Extensions.Localization.Generator is a Roslyn incremental source generator that:
- Reads `.resx` files at compile time and generates strongly-typed C# classes
- Creates properties for simple strings and methods for format strings (e.g., `"Hello {0}"`)
- Generates a `ServiceCollectionExtensions` class with `AddStronglyTypedLocalizations()` for DI registration
- Supports XML documentation comments from `.resx` comment fields
- Respects folder structure for namespace computation
- Supports public or internal accessor generation via MSBuild property

## Setup

### 1. Install NuGet Packages
```bash
dotnet add package Microsoft.Extensions.Localization
dotnet add package Shiny.Extensions.Localization.Generator
```

### 2. Register in DI
```csharp
builder.Services.AddLocalization();
builder.Services.AddStronglyTypedLocalizations();
```

### Important Notes
- Every `.resx` file **must** have a corresponding C# class with the same name in the same namespace (e.g., `MyViewModel.resx` requires `MyViewModel.cs`)
- If the associated class does not exist, the generated code will produce a compile error
- Only the default locale `.resx` file is processed (e.g., `MyViewModel.resx`), locale-specific files like `MyViewModel.fr-ca.resx` are handled by the localization framework at runtime
- The generator produces a `{ClassName}Localized` class (e.g., `MyViewModelLocalized`) for each `.resx` file

## How It Works

### Directory Structure
```
MyApp/
├── MyViewModel.cs
├── MyViewModel.resx              # Default locale (English)
├── MyViewModel.fr-ca.resx        # French-Canadian locale
├── Folder1/
│   ├── FolderViewModel.cs
│   ├── FolderViewModel.resx
│   └── FolderViewModel.fr-ca.resx
```

### Generated Output
```
MyApp/
├── MyViewModelLocalized.g.cs                    # RootNamespace.MyViewModelLocalized
├── Folder1.FolderViewModelLocalized.g.cs        # RootNamespace.Folder1.FolderViewModelLocalized
├── ServiceCollectionExtensions.g.cs             # DI registration extension method
```

### Resource Key Rules
- Dots (`.`), hyphens (`-`), and spaces in resource keys are converted to underscores (`_`)
- Format string parameters like `{0}`, `{1}` are detected automatically
- When format parameters are present, a method is generated instead of a property
- Method names get a `Format` suffix unless the key already ends with `Format`

## Code Generation Instructions

When working with this library:

### 1. Resource Files (.resx)

Create `.resx` files alongside their associated C# class:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Welcome" xml:space="preserve">
    <value>Welcome to our app!</value>
    <comment>Greeting shown on the home page</comment>
  </data>
  <data name="UserGreeting" xml:space="preserve">
    <value>Hello {0}, welcome to {1}!</value>
    <comment>Personalized greeting with user name and app name</comment>
  </data>
  <data name="ItemCount" xml:space="preserve">
    <value>You have {0} items in your cart</value>
  </data>
</root>
```

### 2. Associated Class

The class must exist with the same name and namespace as the `.resx` file:

```csharp
namespace MyApp;

public class MyViewModel
{
    public MyViewModel(MyViewModelLocalized localizer)
    {
        this.Localizer = localizer;
    }

    public MyViewModelLocalized Localizer { get; }
}
```

### 3. Generated Class Structure

The generator creates:

```csharp
namespace MyApp;

public partial class MyViewModelLocalized
{
    readonly IStringLocalizer localizer;

    public MyViewModelLocalized(IStringLocalizer<MyViewModel> localizer)
    {
        this.localizer = localizer;
    }

    public IStringLocalizer Localizer => this.localizer;

    /// <summary>
    /// Greeting shown on the home page
    /// </summary>
    public string Welcome => this.localizer["Welcome"];

    /// <summary>
    /// Personalized greeting with user name and app name
    /// </summary>
    public string UserGreetingFormat(object parameter0, object parameter1)
    {
        return string.Format(this.localizer["UserGreeting"], parameter0, parameter1);
    }

    /// <summary>
    /// You have {0} items in your cart
    /// </summary>
    public string ItemCountFormat(object parameter0)
    {
        return string.Format(this.localizer["ItemCount"], parameter0);
    }
}
```

### 4. XAML Binding

Bind in XAML with full IntelliSense:
```xml
<Label Text="{Binding Localizer.Welcome}" />
```

### 5. File Organization

Place files following standard .NET conventions:
- Resource files: alongside their associated class file
- Locale variants: same location with locale suffix (e.g., `MyClass.fr-ca.resx`)
- Folder structure determines namespace: `Folder1/MyClass.resx` → `RootNamespace.Folder1`

## Configuration

### Internal Accessor

To generate classes with `internal` instead of `public` access modifier:

```xml
<PropertyGroup>
    <GenerateLocalizersInternal>True</GenerateLocalizersInternal>
</PropertyGroup>
```

## Usage Examples

**Simple property access:**
```csharp
var localizer = services.GetRequiredService<MyViewModelLocalized>();
Console.WriteLine(localizer.Welcome);
```

**Format string method:**
```csharp
Console.WriteLine(localizer.UserGreetingFormat("Allan", "MyApp"));
// Output: Hello Allan, welcome to MyApp!
```

**XAML binding in MAUI:**
```xml
<Label Text="{Binding Localizer.Welcome}" />
```

**ASP.NET/Blazor usage:**
```csharp
@inject MyViewModelLocalized Localizer

<h1>@Localizer.Welcome</h1>
<p>@Localizer.UserGreetingFormat(User.Name, "MyApp")</p>
```

## Best Practices

1. **Always create the associated class** - Every `.resx` file must have a matching C# class or a compile error will occur
2. **Use comments in .resx** - Comments become XML documentation on generated properties/methods
3. **Use format parameters for dynamic content** - `{0}`, `{1}` etc. generate type-safe methods
4. **Use default locale for generation** - Only `ClassName.resx` is processed; `ClassName.fr-ca.resx` is for runtime localization
5. **Inject the Localized class** - Use DI to inject `{ClassName}Localized`, not `IStringLocalizer` directly
6. **Expose the raw localizer** - The generated class exposes `Localizer` property for edge cases needing `IStringLocalizer`
7. **Namespace follows folder structure** - Organize `.resx` files in folders to match your namespace conventions

## Reference Files

For detailed templates and examples, see:
- `reference/templates.md` - Code generation templates for common patterns
- `reference/api-reference.md` - Full API surface, MSBuild properties, and troubleshooting

## Common Packages

```bash
dotnet add package Shiny.Extensions.Localization.Generator    # Source generator
dotnet add package Microsoft.Extensions.Localization           # Localization framework
dotnet add package Microsoft.Extensions.DependencyInjection    # DI container
```
