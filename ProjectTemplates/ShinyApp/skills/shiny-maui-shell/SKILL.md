---
name: shiny-maui-shell
description: Generate .NET MAUI Shell pages, ViewModels, navigation, and source-generated routes using Shiny MAUI Shell
auto_invoke: true
triggers:
  - maui shell
  - shell navigation
  - shell switch
  - switch shell
  - maui navigation
  - maui page
  - maui viewmodel
  - INavigator
  - IDialogs
  - ShellMap
  - ShellProperty
  - UseShinyShell
  - ShinyShell
  - ShinyAppBuilder
  - Shiny.Maui.Shell
  - IPageLifecycleAware
  - INavigationConfirmation
  - INavigationAware
  - NavigateTo
  - GoBack
  - PopToRoot
  - SetRoot
  - SwitchShell
  - Navigating
  - Navigated
  - IDialogs
  - Alert
  - Confirm
  - Prompt
  - ActionSheet
  - NavigationEventArgs
  - NavigatedEventArgs
---

# Shiny MAUI Shell Skill

You are an expert in Shiny MAUI Shell, a library that enhances .NET MAUI Shell with ViewModel lifecycle management, navigation services, and source generation.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create new MAUI pages with ViewModels using Shiny Shell conventions
- Set up or configure Shiny MAUI Shell in their application
- Switch between different Shell instances at runtime (e.g., login shell vs main app shell)
- Implement navigation between pages using `INavigator`
- Show dialogs (alert, confirm, prompt, action sheet) using `IDialogs`
- Add ViewModel lifecycle hooks (appearing, disappearing, navigation confirmation)
- Use source generation with `[ShellMap]` and `[ShellProperty]` attributes
- Pass parameters between pages during navigation
- Create modal pages or tab navigation
- Migrate from vanilla MAUI Shell or Prism navigation to Shiny MAUI Shell

## Library Overview

**Documentation**: https://shinylib.net/maui
**GitHub**: https://github.com/shinyorg/mauishell
**NuGet**: `Shiny.Maui.Shell`
**Namespace**: `Shiny`

Shiny MAUI Shell wraps .NET MAUI Shell to provide:
- Page-to-ViewModel registration and automatic BindingContext assignment
- A testable `INavigator` service for all navigation operations
- A testable `IDialogs` service for alert, confirm, prompt, and action sheet dialogs
- Shell switching — swap the entire Shell at runtime (e.g., login → main app)
- ViewModel lifecycle interfaces (appearing, disappearing, dispose, navigation confirmation)
- Source generators that eliminate boilerplate route registration and produce strongly-typed navigation methods
- `ShinyShell` base class for deterministic initial-page BindingContext assignment

Inspired by [Prism Library](https://prismlibrary.com) by Dan Siegel and Brian Lagunas.

## Setup

### 1. Install NuGet Package
```bash
dotnet add package Shiny.Maui.Shell
```

### 2. Configure in MauiProgram.cs

**Manual registration:**
```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseShinyShell(x => x
            .Add<MainPage, MainViewModel>(registerRoute: false)
            .Add<DetailPage, DetailViewModel>("Detail")
            .Add<SettingsPage, SettingsViewModel>("Settings")
        )
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });

    return builder.Build();
}
```

**With source generation (preferred):**
```csharp
builder
    .UseMauiApp<App>()
    .UseShinyShell(x => x.AddGeneratedMaps())
```

### 3. AppShell must inherit from `ShinyShell`

Your `AppShell` (or any Shell subclass) must inherit from `Shiny.ShinyShell` instead of `Shell`. This ensures the initial page's BindingContext is set deterministically via Shell's own `OnNavigated` lifecycle.

**AppShell.xaml:**
```xml
<shiny:ShinyShell
    x:Class="MyApp.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:shiny="clr-namespace:Shiny;assembly=Shiny.Maui.Shell"
    xmlns:local="clr-namespace:MyApp"
    Title="MyApp">

    <ShellContent
        Title="Home"
        ContentTemplate="{DataTemplate local:MainPage}"
        Route="MainPage" />

</shiny:ShinyShell>
```

**AppShell.xaml.cs:**
```csharp
using Shiny;

namespace MyApp;

public partial class AppShell : ShinyShell
{
    public AppShell()
    {
        InitializeComponent();
    }
}
```

### Important Notes
- Pages defined in AppShell.xaml should use `registerRoute: false` since Shell already registers them
- Pages navigated to programmatically need route registration (the default behavior)
- All Pages and ViewModels are registered as Transient in DI automatically

## Code Generation Instructions

When generating code for Shiny MAUI Shell projects, follow these conventions:

### 1. ViewModels

All ViewModels must implement `INotifyPropertyChanged`. Use `CommunityToolkit.Mvvm` `ObservableObject` as the base:

```csharp
[ShellMap<MyPage>("MyRoute")]
public partial class MyViewModel : ObservableObject
{
}
```

- Use `[ShellMap<TPage>("Route")]` on every ViewModel class
- The `route` parameter must be a valid C# identifier — it is used as the generated constant name and method name
- Invalid route names (hyphens, spaces, leading digits) produce a **SHINY001** compiler error
- When no route is specified, the page type name without the `Page` suffix is used as the generated name
- Set `registerRoute: false` only for pages already declared in AppShell.xaml
- ViewModel classes using source generation should be `partial`
- Use primary constructors to inject `INavigator` and other dependencies

### 2. Navigation Properties

Use `[ShellProperty]` on ViewModel properties that should be passed as navigation parameters:

```csharp
[ShellMap<DetailPage>("Detail")]
public partial class DetailViewModel : ObservableObject, IQueryAttributable
{
    [ShellProperty]
    public string ItemId { get; set; }

    [ShellProperty(required: false)]
    public int PageIndex { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(ItemId), out var id))
            ItemId = id?.ToString();
    }
}
```

- Properties marked `[ShellProperty]` are required by default
- Use `[ShellProperty(required: false)]` for optional parameters
- The ViewModel must implement `IQueryAttributable` to receive the parameters
- Source generator creates strongly-typed extension methods on `INavigator`

### 3. Lifecycle Interfaces

Implement these interfaces on ViewModels as needed:

| Interface | Purpose |
|-----------|---------|
| `IPageLifecycleAware` | `OnAppearing()` / `OnDisappearing()` hooks |
| `INavigationConfirmation` | `Task<bool> CanNavigate()` - confirm before leaving |
| `INavigationAware` | `OnNavigatingFrom(IDictionary<string, object>)` - mutate args before leaving |
| `IQueryAttributable` | `ApplyQueryAttributes(IDictionary<string, object>)` - receive navigation args |
| `IDisposable` | Cleanup when page is removed from navigation stack |

### 4. Navigation Events

`INavigator` exposes two events for observing navigation:

- `Navigating` — fires **before** navigation with the source ViewModel instance
- `Navigated` — fires **after** navigation with the destination ViewModel instance

```csharp
navigator.Navigating += (sender, args) =>
{
    // args.FromUri, args.FromViewModel, args.ToUri, args.NavigationType, args.Parameters
};

navigator.Navigated += (sender, args) =>
{
    // args.ToUri, args.ToViewModel, args.NavigationType, args.Parameters
};
```

Hook these events in an `IMauiInitializeService` for cross-cutting concerns like logging or analytics.

### 5. Navigation

Always use `INavigator` for navigation, never `Shell.Current.GoToAsync` directly:

```csharp
// Route-based navigation with args
await navigator.NavigateTo("Detail", ("ItemId", "123"), ("PageIndex", 0));

// ViewModel-based navigation with strongly-typed configuration
await navigator.NavigateTo<DetailViewModel>(vm => vm.ItemId = "123");

// Source-generated strongly-typed method (preferred)
await navigator.NavigateToDetail("123", pageIndex: 0);

// Go back with result parameters
await navigator.GoBack(("Result", selectedItem));

// Go back multiple pages
await navigator.GoBack(backCount: 2);

// Pop to root
await navigator.PopToRoot();

// Set new root page
await navigator.SetRoot<MainViewModel>();

// Switch to a different Shell instance
await navigator.SwitchShell(new MainAppShell());

// Switch to a Shell resolved from DI
await navigator.SwitchShell<MainAppShell>();
```

### 6. Dialogs

Always use `IDialogs` for user-facing dialogs. Inject it via the primary constructor:

```csharp
public class MyViewModel(INavigator navigator, IDialogs dialogs)
{
    // Alert - informational message
    await dialogs.Alert("Title", "Something happened");

    // Confirm - yes/no question, returns bool
    bool confirmed = await dialogs.Confirm("Delete?", "Are you sure?");

    // Prompt - text input, returns string? (null if cancelled)
    var name = await dialogs.Prompt("Name", "Enter your name", placeholder: "John Doe");

    // Prompt with numeric keyboard
    var age = await dialogs.Prompt("Age", "Enter your age", keyboard: Keyboard.Numeric);

    // Action sheet - choose from options
    var choice = await dialogs.ActionSheet("Options", "Cancel", "Delete", "Edit", "Share");
}
```

### 7. Modal Pages

Set `Shell.PresentationMode="Modal"` on the page XAML:
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Shell.PresentationMode="Modal"
             x:Class="MyApp.ModalPage">
```

Navigate to it like any other page. Close with `GoBack()`.

### 8. File Organization

Place files following standard MAUI conventions:
- Pages: `Views/{Name}Page.xaml` + `Views/{Name}Page.xaml.cs`
- ViewModels: `ViewModels/{Name}ViewModel.cs`
- Or co-locate: `Features/{Feature}/{Name}Page.xaml` + `{Name}ViewModel.cs`

## Source Generation Output

The source generator produces up to three files from `[ShellMap]` and `[ShellProperty]` attributes. Each can be individually disabled via MSBuild properties.

### Routes.g.cs
The constant name is derived from the `route` parameter (or page type name without `Page` suffix when no route is specified):
```csharp
public static class Routes
{
    public const string Detail = "Detail";
    public const string Settings = "Settings";
}
```

### NavigationExtensions.g.cs
Method names are also derived from the route parameter:
```csharp
public static class NavigationExtensions
{
    public static Task NavigateToDetail(this INavigator navigator, string itemId, int pageIndex = default)
    {
        return navigator.NavigateTo<DetailViewModel>(x =>
        {
            x.ItemId = itemId;
            x.PageIndex = pageIndex;
        });
    }
}
```

### NavigationBuilderExtensions.g.cs
Uses inline string literals (not `Routes.*` constants), so it works regardless of whether route constants are enabled:
```csharp
public static class NavigationBuilderExtensions
{
    public static ShinyAppBuilder AddGeneratedMaps(this ShinyAppBuilder builder)
    {
        builder.Add<DetailPage, DetailViewModel>("Detail");
        builder.Add<SettingsPage, SettingsViewModel>("Settings");
        return builder;
    }
}
```

### Configuring Source Generation

Disable individual generated files via MSBuild properties in `.csproj`:

```xml
<PropertyGroup>
    <!-- Disable Routes.g.cs -->
    <ShinyMauiShell_GenerateRouteConstants>false</ShinyMauiShell_GenerateRouteConstants>

    <!-- Disable NavigationExtensions.g.cs -->
    <ShinyMauiShell_GenerateNavExtensions>false</ShinyMauiShell_GenerateNavExtensions>
</PropertyGroup>
```

`NavigationBuilderExtensions.g.cs` (`AddGeneratedMaps()`) is always generated — even when no `[ShellMap]` attributes exist yet — so you can wire up `MauiProgram.cs` immediately.

## Complete ViewModel Example

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace MyApp.ViewModels;

[ShellMap<DetailPage>("Detail")]
public partial class DetailViewModel(INavigator navigator, IDialogs dialogs) : ObservableObject,
    IQueryAttributable,
    IPageLifecycleAware,
    INavigationConfirmation,
    INavigationAware,
    IDisposable
{
    [ShellProperty]
    [ObservableProperty]
    string itemId;

    [ObservableProperty]
    string title;

    bool hasUnsavedChanges;

    // Receive navigation parameters
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(ItemId), out var id))
            ItemId = id?.ToString();
    }

    // Page appeared
    public void OnAppearing()
    {
        // Load data, start listening, etc.
    }

    // Page disappearing
    public void OnDisappearing()
    {
        // Pause operations
    }

    // Confirm before leaving
    public async Task<bool> CanNavigate()
    {
        if (!hasUnsavedChanges)
            return true;

        return await dialogs.Confirm(
            "Unsaved Changes",
            "You have unsaved changes. Discard them?"
        );
    }

    // Mutate parameters before leaving
    public void OnNavigatingFrom(IDictionary<string, object> parameters)
    {
        parameters["LastViewedItem"] = ItemId;
    }

    [RelayCommand]
    async Task Save()
    {
        // Save logic
        hasUnsavedChanges = false;
        await navigator.GoBack(("Saved", true));
    }

    [RelayCommand]
    Task GoBack() => navigator.GoBack();

    public void Dispose()
    {
        // Cleanup subscriptions, timers, etc.
    }
}
```

## Best Practices

1. **Use source generation** - Always prefer `[ShellMap]` + `[ShellProperty]` + `AddGeneratedMaps()` over manual registration
2. **Inject INavigator** - Never use `Shell.Current.GoToAsync` directly; use `INavigator` for testability
3. **Inject IDialogs** - Never use `Shell.Current.DisplayAlert` directly; use `IDialogs` for testability
4. **Use primary constructors** - Inject dependencies via primary constructor parameters
5. **Implement IQueryAttributable** - Required to receive navigation parameters on the target ViewModel
6. **Use ObservableObject** - From CommunityToolkit.Mvvm as the ViewModel base class
7. **Implement IDisposable** - Clean up event handlers and subscriptions to prevent memory leaks
8. **Use CanNavigate for guards** - Protect unsaved changes with `INavigationConfirmation`
9. **Mark ViewModel partial** - Required when using `[ShellMap]` source generation and CommunityToolkit attributes
10. **Pass results via GoBack args** - Return data to the previous page through navigation parameters

## Reference Files

For detailed templates and examples, see:
- `reference/templates.md` - Page and ViewModel code generation templates
- `reference/api-reference.md` - Full API surface, interfaces, and attributes

## Common Packages

```bash
dotnet add package Shiny.Maui.Shell           # Core library with source generators
dotnet add package CommunityToolkit.Mvvm      # ObservableObject, RelayCommand, etc.
```
