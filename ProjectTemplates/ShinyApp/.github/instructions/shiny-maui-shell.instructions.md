---
applyTo: "**/*.cs,**/*.razor,**/*.xaml"
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
- No special AppShell subclass required

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

### Important Notes
- The default MAUI AppShell.xaml does not need modification to work with Shiny Shell
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

# API Reference

## Installation

```bash
dotnet add package Shiny.Maui.Shell
```

The NuGet package includes both the runtime library and the source generator. No additional analyzer package is needed.

## Namespace

All public types are in the `Shiny` namespace:
```csharp
using Shiny;
```

## INavigator Interface

The primary navigation service. Injected via DI as a singleton.

```csharp
public interface INavigator
{
    // Fires before navigation occurs - includes the source ViewModel instance
    event EventHandler<NavigationEventArgs>? Navigating;

    // Fires after navigation completes - includes the destination ViewModel instance
    event EventHandler<NavigatedEventArgs>? Navigated;

    // Navigate to a registered route with key-value arguments
    Task NavigateTo(string route, params IEnumerable<(string Key, object Value)> args);

    // Navigate to the page associated with a ViewModel type
    // configure: optional action to set ViewModel properties before navigation
    Task NavigateTo<TViewModel>(
        Action<TViewModel>? configure = null,
        params IEnumerable<(string Key, object Value)> args
    );

    // Replace the root page with one associated with the ViewModel type
    Task SetRoot<TViewModel>(
        Action<TViewModel>? configure = null,
        params IEnumerable<(string Key, object Value)> args
    );

    // Pop to the root page, optionally passing arguments
    Task PopToRoot(params IEnumerable<(string Key, object Value)> args);

    // Go back one page, optionally passing arguments
    Task GoBack(params IEnumerable<(string Key, object Value)> args);

    // Go back multiple pages
    Task GoBack(int backCount = 1, params IEnumerable<(string Key, object Value)> args);

    // Switch to a different Shell instance (replaces Application.MainPage)
    Task SwitchShell(Shell shell);

    // Switch to a Shell resolved from the DI container
    Task SwitchShell<TShell>() where TShell : Shell;
}
```

## IDialogs Interface

A testable dialog service. Injected via DI as a singleton. Use this instead of `Shell.Current.DisplayAlert`, `Shell.Current.DisplayPromptAsync`, or `Shell.Current.DisplayActionSheet`.

```csharp
public interface IDialogs
{
    // Display an alert dialog
    Task Alert(string? title, string message, string acceptText = "OK");

    // Display a confirmation dialog, returns true if accepted
    Task<bool> Confirm(string? title, string message, string acceptText = "Yes", string cancelText = "No");

    // Display a text input prompt, returns entered text or null if cancelled
    Task<string?> Prompt(
        string? title,
        string message,
        string acceptText = "OK",
        string cancelText = "Cancel",
        string? placeholder = null,
        string initialValue = "",
        int maxLength = -1,
        Keyboard? keyboard = null
    );

    // Display an action sheet with multiple options, returns selected button text
    Task<string> ActionSheet(string? title, string? cancel, string? destruction, params string[] buttons);
}
```

### Usage Examples

```csharp
public class MyViewModel(IDialogs dialogs)
{
    // Alert
    await dialogs.Alert("Error", "Something went wrong");

    // Confirm
    if (await dialogs.Confirm("Delete", "Are you sure?"))
    {
        // delete item
    }

    // Prompt for text input
    var name = await dialogs.Prompt("Name", "Enter your name", placeholder: "John Doe");
    if (name != null)
    {
        // user accepted with a value
    }

    // Prompt with numeric keyboard and max length
    var code = await dialogs.Prompt("Code", "Enter PIN", maxLength: 4, keyboard: Keyboard.Numeric);

    // Action sheet
    var action = await dialogs.ActionSheet("Photo", "Cancel", "Delete", "Take Photo", "Choose from Library");
}
```

## Navigation Events

### NavigationEventArgs (pre-navigation)

Fired via `INavigator.Navigating` before navigation occurs. Provides the source ViewModel instance.

```csharp
public record NavigationEventArgs(
    string? FromUri,                                  // Current location URI
    object? FromViewModel,                            // Source ViewModel instance (cast as needed)
    string ToUri,                                     // Destination route URI
    NavigationType NavigationType,                    // Push, SetRoot, GoBack, or PopToRoot
    IReadOnlyDictionary<string, object> Parameters    // Navigation parameters
);
```

### NavigatedEventArgs (post-navigation)

Fired via `INavigator.Navigated` after navigation completes and the destination page's ViewModel is resolved. Provides the destination ViewModel instance.

```csharp
public record NavigatedEventArgs(
    string ToUri,                                     // Destination route URI
    object? ToViewModel,                              // Destination ViewModel instance (cast as needed)
    NavigationType NavigationType,                    // Push, SetRoot, GoBack, or PopToRoot
    IReadOnlyDictionary<string, object> Parameters    // Navigation parameters
);
```

### NavigationType Enum

```csharp
public enum NavigationType
{
    Push,
    SetRoot,
    GoBack,
    PopToRoot,
    SwitchShell
}
```

### Usage

```csharp
navigator.Navigating += (sender, args) =>
{
    // Access the source ViewModel
    if (args.FromViewModel is MyViewModel vm)
        Console.WriteLine($"Leaving {vm.Title}");
};

navigator.Navigated += (sender, args) =>
{
    // Access the destination ViewModel
    if (args.ToViewModel is DetailViewModel detail)
        Console.WriteLine($"Arrived at {detail.ItemId}");
};
```

### Usage Examples

```csharp
public class MyViewModel(INavigator navigator)
{
    // Route-based navigation
    await navigator.NavigateTo("Detail", ("ItemId", "abc"), ("Mode", "edit"));

    // ViewModel-based navigation
    await navigator.NavigateTo<DetailViewModel>(vm => vm.ItemId = "abc");

    // Go back with result
    await navigator.GoBack(("Result", selectedValue));

    // Go back 2 pages
    await navigator.GoBack(2);

    // Pop entire stack to root
    await navigator.PopToRoot();

    // Replace root
    await navigator.SetRoot<DashboardViewModel>();

    // Switch to a different Shell instance
    await navigator.SwitchShell(new MainAppShell());

    // Switch to a Shell resolved from DI
    await navigator.SwitchShell<MainAppShell>();
}
```

## IPageLifecycleAware Interface

Provides page appearing/disappearing lifecycle hooks on ViewModels.

```csharp
public interface IPageLifecycleAware
{
    // Called when the page becomes visible (or re-appears after navigation back)
    void OnAppearing();

    // Called when the page is hidden or removed from the navigation stack
    void OnDisappearing();
}
```

## INavigationConfirmation Interface

Allows a ViewModel to block navigation away from its page.

```csharp
public interface INavigationConfirmation
{
    // Return true to allow navigation, false to block it
    Task<bool> CanNavigate();
}
```

### Usage
```csharp
public async Task<bool> CanNavigate()
{
    if (!hasUnsavedChanges)
        return true;

    return await dialogs.Confirm("Unsaved Changes", "Discard changes?");
}
```

## INavigationAware Interface

Allows a ViewModel to add or modify navigation parameters before the page navigates away.

```csharp
public interface INavigationAware
{
    // Called before navigation. Mutate the parameters dictionary to pass data back.
    void OnNavigatingFrom(IDictionary<string, object> parameters);
}
```

### Usage
```csharp
public void OnNavigatingFrom(IDictionary<string, object> parameters)
{
    parameters["LastViewed"] = CurrentItemId;
    parameters["Timestamp"] = DateTime.UtcNow;
}
```

## ShinyAppBuilder Class

Fluent builder for registering Page-to-ViewModel mappings. Used inside `UseShinyShell()`.

```csharp
public sealed class ShinyAppBuilder
{
    // Register a Page-ViewModel pair
    // route: optional route name (defaults to page class name)
    // registerRoute: set false for pages already in AppShell.xaml
    ShinyAppBuilder Add<TPage, TViewModel>(string? route = null, bool registerRoute = true)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged;
}
```

### Constraints
- `TPage` must inherit from `Microsoft.Maui.Controls.Page`
- `TViewModel` must implement `INotifyPropertyChanged`
- Both are registered as Transient in DI automatically

## Attributes

### ShellMapAttribute\<TPage\>

Marks a ViewModel class for source generation. Applied to the ViewModel class.

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ShellMapAttribute<TPage>(
    string? route = null,         // Route name — must be a valid C# identifier; used as generated constant and method name
    bool registerRoute = true     // Set false for AppShell.xaml pages
) : Attribute;
```

The `route` parameter drives naming:
- `[ShellMap<DetailPage>("Detail")]` → `Routes.Detail`, `NavigateToDetail(...)`
- `[ShellMap<HomePage>]` (no route) → `Routes.Home`, `NavigateToHome(...)`

Invalid route names (hyphens, spaces, leading digits) produce a **SHINY001** compiler error.

### ShellPropertyAttribute

Marks a ViewModel property as a navigation parameter for source generation.

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ShellPropertyAttribute(
    bool required = true          // Whether this parameter is required in generated methods
) : Attribute;
```

### Source Generation Output

Given this input:
```csharp
[ShellMap<DetailPage>("Detail")]
public partial class DetailViewModel : ObservableObject
{
    [ShellProperty] public string ItemId { get; set; }
    [ShellProperty(required: false)] public int Page { get; set; }
}
```

The source generator produces:

**Routes.g.cs:**
```csharp
public static class Routes
{
    public const string Detail = "Detail";
}
```

**NavigationExtensions.g.cs:**
```csharp
public static class NavigationExtensions
{
    public static Task NavigateToDetail(this INavigator navigator, string itemId, int page = default)
    {
        return navigator.NavigateTo<DetailViewModel>(x =>
        {
            x.ItemId = itemId;
            x.Page = page;
        });
    }
}
```

**NavigationBuilderExtensions.g.cs** (uses string literals, not `Routes.*`):
```csharp
public static class NavigationBuilderExtensions
{
    public static ShinyAppBuilder AddGeneratedMaps(this ShinyAppBuilder builder)
    {
        builder.Add<DetailPage, DetailViewModel>("Detail");
        return builder;
    }
}
```

### Configuring Source Generation

Disable individual generated files via MSBuild properties:

| Property | Default | Controls |
|---|---|---|
| `ShinyMauiShell_GenerateRouteConstants` | `true` | `Routes.g.cs` |
| `ShinyMauiShell_GenerateNavExtensions` | `true` | `NavigationExtensions.g.cs` |

`NavigationBuilderExtensions.g.cs` is always generated.

## Extension Method

### UseShinyShell

Configures Shiny MAUI Shell on the `MauiAppBuilder`.

```csharp
public static MauiAppBuilder UseShinyShell(
    this MauiAppBuilder builder,
    Action<ShinyAppBuilder> navBuilderAction
);
```

Registers:
- `INavigator` as singleton
- `IDialogs` as singleton
- `IMauiInitializeService` for lifecycle hooks
- `ShinyAppBuilder` as singleton
- All mapped Pages and ViewModels as transient

## IQueryAttributable (MAUI Built-in)

Standard MAUI interface for receiving navigation parameters. Must be implemented on ViewModels that receive arguments.

```csharp
// From Microsoft.Maui.Controls
public interface IQueryAttributable
{
    void ApplyQueryAttributes(IDictionary<string, object> query);
}
```

## IDisposable (System)

When implemented on a ViewModel, `Dispose()` is called when the page is permanently removed from the navigation stack.

## Troubleshooting

### ViewModel not bound to Page
- Ensure the Page-ViewModel pair is registered via `Add<TPage, TViewModel>()` or `[ShellMap]` + `AddGeneratedMaps()`
- Check that `UseShinyShell()` is called in MauiProgram.cs

### Navigation parameters not received
- ViewModel must implement `IQueryAttributable`
- Parameter keys are case-sensitive and must match property names
- When using `NavigateTo<TViewModel>(configure)`, properties set via `configure` are available immediately (no need for `IQueryAttributable`)

### Page not found during navigation
- Pages in AppShell.xaml should use `registerRoute: false`
- Pages not in AppShell.xaml need route registration (default behavior)
- Verify the route string matches exactly

### Source generator not producing output
- ViewModel class must be `partial`
- Ensure `Shiny.Maui.Shell` NuGet is installed (includes the generator)
- Check that `[ShellMap<TPage>]` attribute is applied to the class
- Route names must be valid C# identifiers — check for **SHINY001** errors
- Route constants and nav extensions can be disabled via `ShinyMauiShell_GenerateRouteConstants` and `ShinyMauiShell_GenerateNavExtensions` MSBuild properties
- Clean and rebuild the project

### OnAppearing/OnDisappearing not firing
- ViewModel must implement `IPageLifecycleAware`
- Verify the ViewModel is bound to the Page (check BindingContext)

### CanNavigate not called
- ViewModel must implement `INavigationConfirmation`
- Only fires when navigating away from the page (not when navigating to it)

# Shiny MAUI Shell Code Templates

## Page + ViewModel Template (with Source Generation)

When generating a new page and ViewModel pair, create both files:

### XAML Page
```xml
<!-- Views/{Name}Page.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="{Namespace}.Views.{Name}Page"
             Title="{Binding Title}">
    <VerticalStackLayout Padding="16" Spacing="12">
        <!-- Page content here -->
    </VerticalStackLayout>
</ContentPage>
```

### XAML Code-Behind
```csharp
// Views/{Name}Page.xaml.cs
namespace {Namespace}.Views;

public partial class {Name}Page : ContentPage
{
    public {Name}Page()
    {
        InitializeComponent();
    }
}
```

### ViewModel (Minimal)
```csharp
// ViewModels/{Name}ViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace {Namespace}.ViewModels;

[ShellMap<{Name}Page>("{route}")]
public partial class {Name}ViewModel(INavigator navigator) : ObservableObject
{
    [ObservableProperty]
    string title = "{Page Title}";
}
```

### ViewModel (Full Lifecycle)
```csharp
// ViewModels/{Name}ViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace {Namespace}.ViewModels;

[ShellMap<{Name}Page>("{route}")]
public partial class {Name}ViewModel(INavigator navigator) : ObservableObject,
    IQueryAttributable,
    IPageLifecycleAware,
    INavigationConfirmation,
    INavigationAware,
    IDisposable
{
    [ObservableProperty]
    string title = "{Page Title}";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Receive navigation parameters
    }

    public void OnAppearing()
    {
        // Page is visible - load data, subscribe to events
    }

    public void OnDisappearing()
    {
        // Page hidden - pause operations
    }

    public Task<bool> CanNavigate()
    {
        // Return true to allow navigation, false to block
        return Task.FromResult(true);
    }

    public void OnNavigatingFrom(IDictionary<string, object> parameters)
    {
        // Add/modify parameters before leaving
    }

    public void Dispose()
    {
        // Cleanup subscriptions, timers, etc.
    }
}
```

## Page with Navigation Parameters Template

### ViewModel with ShellProperty
```csharp
// ViewModels/{Name}ViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace {Namespace}.ViewModels;

[ShellMap<{Name}Page>("{route}")]
public partial class {Name}ViewModel(INavigator navigator) : ObservableObject,
    IQueryAttributable
{
    // Required parameter - source generator will make this a required method parameter
    [ShellProperty]
    [ObservableProperty]
    string {requiredParam};

    // Optional parameter - source generator will give this a default value
    [ShellProperty(required: false)]
    [ObservableProperty]
    int {optionalParam};

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof({RequiredParam}), out var param1))
            {RequiredParam} = param1?.ToString();

        if (query.TryGetValue(nameof({OptionalParam}), out var param2) && param2 is int intVal)
            {OptionalParam} = intVal;
    }
}
```

## Modal Page Template

### XAML
```xml
<!-- Views/{Name}Page.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Shell.PresentationMode="Modal"
             x:Class="{Namespace}.Views.{Name}Page"
             Title="{Binding Title}">
    <Grid RowDefinitions="Auto,*,Auto" Padding="16">
        <!-- Header -->
        <Label Text="{Binding Title}" FontSize="24" FontAttributes="Bold" />

        <!-- Content -->
        <ScrollView Grid.Row="1">
            <!-- Modal content here -->
        </ScrollView>

        <!-- Actions -->
        <HorizontalStackLayout Grid.Row="2" Spacing="12" HorizontalOptions="End">
            <Button Text="Cancel" Command="{Binding CloseCommand}" />
            <Button Text="Save" Command="{Binding SaveCommand}" />
        </HorizontalStackLayout>
    </Grid>
</ContentPage>
```

### ViewModel
```csharp
// ViewModels/{Name}ViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace {Namespace}.ViewModels;

[ShellMap<{Name}Page>("{route}")]
public partial class {Name}ViewModel(INavigator navigator) : ObservableObject,
    IPageLifecycleAware,
    IDisposable
{
    [ObservableProperty]
    string title = "{Modal Title}";

    public void OnAppearing() { }
    public void OnDisappearing() { }

    [RelayCommand]
    Task Close() => navigator.GoBack();

    [RelayCommand]
    async Task Save()
    {
        // Save logic here
        await navigator.GoBack(("Result", "saved"));
    }

    public void Dispose() { }
}
```

## Root/Home Page Template (No Route Registration)

For pages declared in AppShell.xaml, use `registerRoute: false`:

```csharp
// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace {Namespace}.ViewModels;

[ShellMap<MainPage>(registerRoute: false)]
public partial class MainViewModel(INavigator navigator) : ObservableObject,
    IQueryAttributable,
    IPageLifecycleAware
{
    [ObservableProperty]
    string title = "Home";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Receive results from pages that navigated back
        if (query.TryGetValue("Result", out var result))
        {
            // Handle result
        }
    }

    public void OnAppearing() { }
    public void OnDisappearing() { }

    [RelayCommand]
    Task NavigateToDetail() => navigator.NavigateTo<DetailViewModel>(vm => vm.ItemId = "123");

    [RelayCommand]
    Task NavigateByRoute() => navigator.NavigateTo("Detail", ("ItemId", "123"));
}
```

## List-Detail Navigation Template

### List ViewModel
```csharp
[ShellMap<ItemListPage>(registerRoute: false)]
public partial class ItemListViewModel(INavigator navigator) : ObservableObject,
    IQueryAttributable,
    IPageLifecycleAware
{
    [ObservableProperty]
    ObservableCollection<ItemModel> items = new();

    [ObservableProperty]
    ItemModel selectedItem;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Refresh", out _))
            LoadItems();
    }

    public void OnAppearing() => LoadItems();
    public void OnDisappearing() { }

    void LoadItems()
    {
        // Load items from service
    }

    [RelayCommand]
    async Task ItemSelected(ItemModel item)
    {
        if (item == null) return;
        await navigator.NavigateTo<ItemDetailViewModel>(vm => vm.ItemId = item.Id);
    }
}
```

### Detail ViewModel
```csharp
[ShellMap<ItemDetailPage>("ItemDetail")]
public partial class ItemDetailViewModel(INavigator navigator, IDialogs dialogs, IItemService itemService) : ObservableObject,
    IQueryAttributable,
    IPageLifecycleAware,
    INavigationConfirmation,
    IDisposable
{
    [ShellProperty]
    [ObservableProperty]
    string itemId;

    [ObservableProperty]
    ItemModel item;

    bool isDirty;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(ItemId), out var id))
            ItemId = id?.ToString();
    }

    public async void OnAppearing()
    {
        if (!string.IsNullOrEmpty(ItemId))
            Item = await itemService.GetItem(ItemId);
    }

    public void OnDisappearing() { }

    public async Task<bool> CanNavigate()
    {
        if (!isDirty) return true;
        return await dialogs.Confirm("Unsaved Changes", "Discard changes?");
    }

    [RelayCommand]
    async Task Save()
    {
        await itemService.SaveItem(Item);
        isDirty = false;
        await navigator.GoBack(("Refresh", true));
    }

    [RelayCommand]
    Task Cancel() => navigator.GoBack();

    public void Dispose() { }
}
```

## MauiProgram.cs Setup Template

### With Source Generation (Recommended)
```csharp
// MauiProgram.cs
using Shiny;

namespace {Namespace};

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShinyShell(x => x.AddGeneratedMaps())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register additional services
        builder.Services.AddSingleton<IItemService, ItemService>();

#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

### Manual Registration
```csharp
builder
    .UseMauiApp<App>()
    .UseShinyShell(x => x
        .Add<MainPage, MainViewModel>(registerRoute: false)
        .Add<DetailPage, DetailViewModel>("Detail")
        .Add<SettingsPage, SettingsViewModel>("Settings")
        .Add<ModalPage, ModalViewModel>("Modal")
    )
```
