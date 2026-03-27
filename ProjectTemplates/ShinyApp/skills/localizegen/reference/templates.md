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
