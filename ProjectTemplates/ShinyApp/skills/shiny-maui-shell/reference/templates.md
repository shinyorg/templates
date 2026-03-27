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
