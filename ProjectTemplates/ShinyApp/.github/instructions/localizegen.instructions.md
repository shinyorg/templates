---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
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

# Localization Generator Code Templates

## Basic Resource + Class Template

When adding localization to a class, create both the `.resx` file and ensure the associated class exists.

### Resource File (.resx)
```xml
<!-- {Name}.resx -->
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msOccurs="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msOccurs="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
              <xsd:attribute name="xml:space" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <data name="{Key}" xml:space="preserve">
    <value>{Value}</value>
    <comment>{Optional comment for XML docs}</comment>
  </data>
</root>
```

### Associated Class
```csharp
// {Name}.cs
namespace {Namespace};

public class {Name}
{
    public {Name}({Name}Localized localizer)
    {
        this.Localizer = localizer;
    }

    public {Name}Localized Localizer { get; }
}
```

## MAUI ViewModel with Localization Template

### ViewModel
```csharp
// ViewModels/{Name}ViewModel.cs
namespace {Namespace}.ViewModels;

public class {Name}ViewModel : INotifyPropertyChanged
{
    public {Name}ViewModel({Name}ViewModelLocalized localizer)
    {
        this.Localizer = localizer;
    }

    public {Name}ViewModelLocalized Localizer { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
}
```

### Resource File
```xml
<!-- ViewModels/{Name}ViewModel.resx -->
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Title" xml:space="preserve">
    <value>Page Title</value>
    <comment>Title displayed at top of page</comment>
  </data>
  <data name="Description" xml:space="preserve">
    <value>Page description text</value>
  </data>
  <data name="WelcomeMessage" xml:space="preserve">
    <value>Welcome, {0}!</value>
    <comment>Welcome message with user name parameter</comment>
  </data>
</root>
```

### XAML Page
```xml
<!-- Views/{Name}Page.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="{Namespace}.Views.{Name}Page"
             Title="{Binding Localizer.Title}">
    <VerticalStackLayout Padding="16" Spacing="12">
        <Label Text="{Binding Localizer.Title}" FontSize="24" FontAttributes="Bold" />
        <Label Text="{Binding Localizer.Description}" />
    </VerticalStackLayout>
</ContentPage>
```

## Resource File with Format Parameters Template

### Resource File
```xml
<!-- {Name}.resx -->
<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- Simple string - generates property -->
  <data name="AppTitle" xml:space="preserve">
    <value>My Application</value>
    <comment>Application title</comment>
  </data>

  <!-- Single parameter - generates method with 1 arg -->
  <data name="Greeting" xml:space="preserve">
    <value>Hello, {0}!</value>
    <comment>Greeting with user name</comment>
  </data>

  <!-- Multiple parameters - generates method with N args -->
  <data name="OrderSummary" xml:space="preserve">
    <value>Order #{0} placed on {1:MMM dd yyyy} for {2:C2}</value>
    <comment>Order summary with order number, date, and amount</comment>
  </data>

  <!-- Key with dots - property name uses underscores -->
  <data name="Errors.Required" xml:space="preserve">
    <value>This field is required</value>
    <comment>Validation error for required fields</comment>
  </data>
</root>
```

### Generated Output
```csharp
// Generated: {Name}Localized.g.cs
namespace {Namespace};

public partial class {Name}Localized
{
    readonly IStringLocalizer localizer;

    public {Name}Localized(IStringLocalizer<{Name}> localizer)
    {
        this.localizer = localizer;
    }

    public IStringLocalizer Localizer => this.localizer;

    /// <summary>
    /// Application title
    /// </summary>
    public string AppTitle => this.localizer["AppTitle"];

    /// <summary>
    /// Greeting with user name
    /// </summary>
    public string GreetingFormat(object parameter0)
    {
        return string.Format(this.localizer["Greeting"], parameter0);
    }

    /// <summary>
    /// Order summary with order number, date, and amount
    /// </summary>
    public string OrderSummaryFormat(object parameter0, object parameter1, object parameter2)
    {
        return string.Format(this.localizer["OrderSummary"], parameter0, parameter1, parameter2);
    }

    /// <summary>
    /// This field is required
    /// </summary>
    public string Errors_Required => this.localizer["Errors.Required"];
}
```

## Locale Variant Template

When adding a new locale, create the locale-specific `.resx` alongside the default:

```xml
<!-- {Name}.fr-ca.resx (French-Canadian) -->
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="AppTitle" xml:space="preserve">
    <value>Mon Application</value>
  </data>
  <data name="Greeting" xml:space="preserve">
    <value>Bonjour, {0}!</value>
  </data>
</root>
```

Supported locale naming: `{Name}.{culture}.resx` (e.g., `MyClass.fr-ca.resx`, `MyClass.de.resx`, `MyClass.ja.resx`)

## ASP.NET Controller with Localization Template

### Controller
```csharp
// Controllers/{Name}Controller.cs
using Microsoft.AspNetCore.Mvc;

namespace {Namespace}.Controllers;

[ApiController]
[Route("api/[controller]")]
public class {Name}Controller : ControllerBase
{
    readonly {Name}ControllerLocalized _localizer;

    public {Name}Controller({Name}ControllerLocalized localizer)
    {
        _localizer = localizer;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = _localizer.Welcome });
    }
}
```

### Resource File
```xml
<!-- Controllers/{Name}Controller.resx -->
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Welcome" xml:space="preserve">
    <value>Welcome to the API</value>
    <comment>Welcome message for API root endpoint</comment>
  </data>
  <data name="NotFound" xml:space="preserve">
    <value>{0} with ID {1} was not found</value>
    <comment>Not found message with entity type and ID</comment>
  </data>
</root>
```

## Blazor Component with Localization Template

### Component
```razor
@* Pages/{Name}.razor *@
@page "/{route}"
@inject {Name}Localized Localizer

<h1>@Localizer.Title</h1>
<p>@Localizer.Description</p>
<p>@Localizer.GreetingFormat(userName)</p>

@code {
    string userName = "World";
}
```

### Code-Behind (if using)
```csharp
// Pages/{Name}.razor.cs
namespace {Namespace}.Pages;

public partial class {Name}
{
    [Inject]
    public {Name}Localized Localizer { get; set; } = default!;
}
```

## DI Setup Templates

### MAUI (MauiProgram.cs)
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace {Namespace};

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddLocalization();
        builder.Services.AddStronglyTypedLocalizations();

        return builder.Build();
    }
}
```

### ASP.NET (Program.cs)
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
builder.Services.AddStronglyTypedLocalizations();

var app = builder.Build();
app.Run();
```

### Console / Generic Host
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.Services.AddStronglyTypedLocalizations();

var app = builder.Build();
var localizer = app.Services.GetRequiredService<MyClassLocalized>();
Console.WriteLine(localizer.Welcome);
```
