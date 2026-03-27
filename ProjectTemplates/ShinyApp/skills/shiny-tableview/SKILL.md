---
name: shiny-tableview
description: Generate settings-style TableView pages and cells for .NET MAUI using Shiny.Maui.TableView - a pure MAUI implementation with cascading styles, 14 cell types, sections, drag-sort, and full MVVM support
auto_invoke: true
triggers:
  - tableview
  - table view
  - settings page
  - settings view
  - settingsview
---

# Shiny.Maui.TableView Skill

You are an expert in Shiny.Maui.TableView, a pure .NET MAUI settings-style TableView control with 14 cell types, cascading styles, sections, drag-sort reordering, and full MVVM/binding support.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create a settings page or preferences UI in .NET MAUI
- Add or configure TableView cells (switch, checkbox, entry, picker, command, etc.)
- Style a TableView with global cascading styles or per-cell overrides
- Use sections with headers, footers, and dynamic ItemTemplate cells
- Enable drag-to-reorder within a section
- Bind cell properties to a ViewModel using MVVM
- Create radio button groups, date/time pickers, number pickers, or multi-select pickers
- Build any form-like or list-based settings UI

## Library Overview

**Repository**: https://github.com/shinyorg/tableview
**Namespace**: `Shiny.Maui.TableView`
**XAML Namespace**: `http://shiny.net/maui/tableview` (prefix: `tv`)

Shiny.Maui.TableView is a pure .NET MAUI implementation inspired by [AiForms.Maui.SettingsView](https://github.com/muak/AiForms.Maui.SettingsView). Unlike AiForms which uses native platform renderers (UITableView on iOS, RecyclerView on Android), this library uses only standard MAUI layouts and views with zero platform-specific code.

## Setup

### 1. Install NuGet Package
```bash
dotnet add package Shiny.Maui.TableView
```

### 2. Configure in MauiProgram.cs
```csharp
using Shiny.Maui.TableView;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShinyTableView();
```

### 3. Add XAML Namespace
```xml
xmlns:tv="http://shiny.net/maui/tableview"
```

## Architecture

The control hierarchy is:
```
tv:TableView (ContentView > ScrollView > VerticalStackLayout)
  tv:TableRoot
    tv:TableSection (Title, FooterText, cells)
      tv:LabelCell / tv:SwitchCell / tv:CommandCell / etc.
```

**Style cascade**: TableView globals -> Section overrides -> Cell overrides. All properties are `BindableProperty` with full MVVM support.

## Cell Types (14 Total)

### LabelCell
Displays title with optional value text on the right.

```xml
<tv:LabelCell Title="Version" ValueText="1.0.0" Description="Latest release" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ValueText` | `string` | `""` | Right-side text |
| `ValueTextColor` | `Color?` | `null` | Value color |
| `ValueTextFontSize` | `double` | `-1` | Value font size |
| `ValueTextFontFamily` | `string?` | `null` | Value font family |
| `ValueTextFontAttributes` | `FontAttributes?` | `null` | Value styling |

### SwitchCell
Toggle switch.

```xml
<tv:SwitchCell Title="Wi-Fi" On="{Binding WifiEnabled, Mode=TwoWay}" OnColor="#34C759" />
```

| Property | Type | Default |
|---|---|---|
| `On` | `bool` | `false` |
| `OnColor` | `Color?` | `null` |

### CheckboxCell
Native checkbox control.

```xml
<tv:CheckboxCell Title="Accept Terms" Checked="{Binding Accepted, Mode=TwoWay}" AccentColor="Green" />
```

| Property | Type | Default |
|---|---|---|
| `Checked` | `bool` | `false` |
| `AccentColor` | `Color?` | `null` |

### SimpleCheckCell
Checkmark toggle (no native checkbox, just a checkmark character).

```xml
<tv:SimpleCheckCell Title="Option A" Checked="{Binding OptionA, Mode=TwoWay}" />
```

| Property | Type | Default |
|---|---|---|
| `Checked` | `bool` | `false` |
| `Value` | `object?` | `null` |
| `AccentColor` | `Color?` | `null` |

### RadioCell
Radio button selection. Use `RadioCell.SelectedValue` attached property on the section.

```xml
<tv:TableSection Title="Theme" tv:RadioCell.SelectedValue="{Binding SelectedTheme, Mode=TwoWay}">
    <tv:RadioCell Title="Light" Value="Light" />
    <tv:RadioCell Title="Dark" Value="Dark" />
    <tv:RadioCell Title="System" Value="System" />
</tv:TableSection>
```

| Property | Type | Default |
|---|---|---|
| `Value` | `object?` | `null` |
| `AccentColor` | `Color?` | `null` |

### CommandCell
Tappable cell with disclosure arrow. Inherits from LabelCell.

```xml
<tv:CommandCell Title="About" ValueText="Learn more"
                 Command="{Binding AboutCommand}"
                 KeepSelectedUntilBack="True" />
```

| Property | Type | Default |
|---|---|---|
| `Command` | `ICommand?` | `null` |
| `CommandParameter` | `object?` | `null` |
| `ShowArrow` | `bool` | `true` |
| `KeepSelectedUntilBack` | `bool` | `false` |

### ButtonCell
Full-width button-style cell.

```xml
<tv:ButtonCell Title="Sign Out" Command="{Binding SignOutCommand}" ButtonTextColor="Red" />
```

| Property | Type | Default |
|---|---|---|
| `Command` | `ICommand?` | `null` |
| `CommandParameter` | `object?` | `null` |
| `ButtonTextColor` | `Color?` | `null` |
| `TitleAlignment` | `TextAlignment` | `Center` |

### EntryCell
Inline text input.

```xml
<tv:EntryCell Title="Email" ValueText="{Binding Email, Mode=TwoWay}"
               Placeholder="user@example.com" Keyboard="Email" />
```

| Property | Type | Default |
|---|---|---|
| `ValueText` | `string` | `""` |
| `Placeholder` | `string` | `""` |
| `PlaceholderColor` | `Color?` | `null` |
| `Keyboard` | `Keyboard` | `Default` |
| `IsPassword` | `bool` | `false` |
| `MaxLength` | `int` | `-1` |
| `TextAlignment` | `TextAlignment` | `End` |
| `CompletedCommand` | `ICommand?` | `null` |
| `ValueTextColor` | `Color?` | `null` |

### DatePickerCell
Opens native date picker dialog on tap.

```xml
<tv:DatePickerCell Title="Birthday" Date="{Binding BirthDate, Mode=TwoWay}" Format="D" />
```

| Property | Type | Default |
|---|---|---|
| `Date` | `DateTime?` | `null` |
| `InitialDate` | `DateTime` | `2000-01-01` |
| `MinimumDate` | `DateTime` | `1900-01-01` |
| `MaximumDate` | `DateTime` | `2100-12-31` |
| `Format` | `string` | `"d"` |
| `ValueTextColor` | `Color?` | `null` |

### TimePickerCell
Opens native time picker dialog on tap.

```xml
<tv:TimePickerCell Title="Alarm" Time="{Binding AlarmTime, Mode=TwoWay}" Format="T" />
```

| Property | Type | Default |
|---|---|---|
| `Time` | `TimeSpan` | `00:00:00` |
| `Format` | `string` | `"t"` |
| `ValueTextColor` | `Color?` | `null` |

### TextPickerCell
Opens native dropdown/spinner picker on tap.

```xml
<tv:TextPickerCell Title="Color" ItemsSource="{Binding Colors}"
                    SelectedIndex="{Binding SelectedColorIndex, Mode=TwoWay}" />
```

| Property | Type | Default |
|---|---|---|
| `ItemsSource` | `IList?` | `null` |
| `SelectedIndex` | `int` | `-1` |
| `SelectedItem` | `object?` | `null` |
| `DisplayMember` | `string?` | `null` |
| `PickerTitle` | `string?` | `null` |
| `SelectedCommand` | `ICommand?` | `null` |
| `ValueTextColor` | `Color?` | `null` |

### NumberPickerCell
Opens a prompt dialog for numeric input.

```xml
<tv:NumberPickerCell Title="Font Size" Number="{Binding FontSize, Mode=TwoWay}"
                      Min="8" Max="72" Unit="pt" />
```

| Property | Type | Default |
|---|---|---|
| `Number` | `int?` | `null` |
| `Min` | `int` | `0` |
| `Max` | `int` | `9999` |
| `Unit` | `string` | `""` |
| `PickerTitle` | `string` | `"Enter a number"` |
| `SelectedCommand` | `ICommand?` | `null` |
| `ValueTextColor` | `Color?` | `null` |

### PickerCell
Full-page picker for single or multi-select. Navigates to a selection page.

```xml
<!-- Single select -->
<tv:PickerCell Title="Country" ItemsSource="{Binding Countries}"
                SelectionMode="Single" SelectedItem="{Binding SelectedCountry, Mode=TwoWay}"
                PageTitle="Select Country" />

<!-- Multi select -->
<tv:PickerCell Title="Hobbies" ItemsSource="{Binding Hobbies}"
                SelectionMode="Multiple" MaxSelectedNumber="3"
                SelectedItems="{Binding SelectedHobbies, Mode=TwoWay}" />
```

| Property | Type | Default |
|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` |
| `SelectedItem` | `object?` | `null` |
| `SelectedItems` | `IList?` | `null` |
| `SelectionMode` | `SelectionMode` | `Single` |
| `MaxSelectedNumber` | `int` | `0` (unlimited) |
| `UsePickToClose` | `bool` | `false` |
| `UseAutoValueText` | `bool` | `true` |
| `DisplayMember` | `string?` | `null` |
| `SubDisplayMember` | `string?` | `null` |
| `PageTitle` | `string` | `"Select"` |
| `ShowArrow` | `bool` | `true` |
| `KeepSelectedUntilBack` | `bool` | `false` |
| `SelectedCommand` | `ICommand?` | `null` |
| `AccentColor` | `Color?` | `null` |

### CustomCell
Hosts any custom MAUI view.

```xml
<tv:CustomCell Title="Progress">
    <tv:CustomCell.CustomContent>
        <ProgressBar Progress="0.75" />
    </tv:CustomCell.CustomContent>
</tv:CustomCell>
```

| Property | Type | Default |
|---|---|---|
| `CustomContent` | `View?` | `null` |
| `UseFullSize` | `bool` | `false` |
| `Command` | `ICommand?` | `null` |
| `LongCommand` | `ICommand?` | `null` |
| `ShowArrow` | `bool` | `false` |
| `KeepSelectedUntilBack` | `bool` | `false` |

## Common Cell Properties (CellBase)

All cells inherit these properties:

| Property | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Primary text |
| `TitleColor` | `Color?` | `null` | Title color |
| `TitleFontSize` | `double` | `-1` | Title font size |
| `TitleFontFamily` | `string?` | `null` | Title font family |
| `TitleFontAttributes` | `FontAttributes?` | `null` | Bold, Italic, None |
| `Description` | `string` | `""` | Subtitle below title |
| `DescriptionColor` | `Color?` | `null` | Description color |
| `DescriptionFontSize` | `double` | `-1` | Description font size |
| `HintText` | `string` | `""` | Right of title hint |
| `HintTextColor` | `Color?` | `null` | Hint color |
| `IconSource` | `ImageSource?` | `null` | Left icon |
| `IconSize` | `double` | `-1` | Icon dimensions |
| `IconRadius` | `double` | `-1` | Icon corner radius |
| `CellBackgroundColor` | `Color?` | `null` | Background color |
| `SelectedColor` | `Color?` | `null` | Tap highlight color |
| `IsSelectable` | `bool` | `true` | Responds to taps |
| `CellHeight` | `double` | `-1` | Fixed height |
| `BorderColor` | `Color?` | `null` | Border color |
| `BorderWidth` | `double` | `-1` | Border width |
| `BorderRadius` | `double` | `-1` | Border corner radius |

## TableSection Properties

```xml
<tv:TableSection Title="GENERAL" FooterText="These settings apply globally"
                  HeaderBackgroundColor="#F2F2F7" HeaderTextColor="#666666"
                  UseDragSort="False">
    <!-- cells -->
</tv:TableSection>
```

| Property | Type | Default |
|---|---|---|
| `Title` | `string` | `""` |
| `FooterText` | `string` | `""` |
| `HeaderView` | `View?` | `null` |
| `FooterView` | `View?` | `null` |
| `IsVisible` | `bool` | `true` |
| `FooterVisible` | `bool` | `true` |
| `HeaderBackgroundColor` | `Color?` | `null` |
| `HeaderTextColor` | `Color?` | `null` |
| `HeaderFontSize` | `double` | `-1` |
| `HeaderFontFamily` | `string?` | `null` |
| `HeaderFontAttributes` | `FontAttributes?` | `null` |
| `HeaderHeight` | `double` | `-1` |
| `FooterTextColor` | `Color?` | `null` |
| `FooterFontSize` | `double` | `-1` |
| `FooterBackgroundColor` | `Color?` | `null` |
| `UseDragSort` | `bool` | `false` |
| `ItemsSource` | `IEnumerable?` | `null` |
| `ItemTemplate` | `DataTemplate?` | `null` |
| `TemplateStartIndex` | `int` | `0` |

## Dynamic Cells with ItemTemplate

Generate cells from a data source:

```xml
<tv:TableSection Title="Items" ItemsSource="{Binding Items}">
    <tv:TableSection.ItemTemplate>
        <DataTemplate>
            <tv:LabelCell Title="{Binding Name}" ValueText="{Binding Value}" />
        </DataTemplate>
    </tv:TableSection.ItemTemplate>
</tv:TableSection>
```

The `ItemsSource` supports `INotifyCollectionChanged` for live updates.

## Global Styling (TableView Properties)

Apply styles at the TableView level. Individual cell/section properties override globals.

```xml
<tv:TableView CellTitleColor="#333333"
              CellTitleFontSize="17"
              CellDescriptionColor="#888888"
              CellValueTextColor="#007AFF"
              CellBackgroundColor="White"
              CellSelectedColor="#EFEFEF"
              CellAccentColor="#007AFF"
              CellIconSize="28"
              HeaderTextColor="#666666"
              HeaderFontSize="13"
              HeaderBackgroundColor="#F2F2F7"
              FooterTextColor="#8E8E93"
              SeparatorColor="#C6C6C8"
              SeparatorPadding="16"
              SectionSeparatorHeight="12">
```

### Cell Global Styles

| Property | Type | Description |
|---|---|---|
| `CellTitleColor` | `Color?` | Title color for all cells |
| `CellTitleFontSize` | `double` | Title font size |
| `CellTitleFontFamily` | `string?` | Title font family |
| `CellTitleFontAttributes` | `FontAttributes?` | Title styling |
| `CellDescriptionColor` | `Color?` | Description color |
| `CellDescriptionFontSize` | `double` | Description font size |
| `CellHintTextColor` | `Color?` | Hint text color |
| `CellHintTextFontSize` | `double` | Hint font size |
| `CellValueTextColor` | `Color?` | Value text color |
| `CellValueTextFontSize` | `double` | Value font size |
| `CellBackgroundColor` | `Color?` | Cell background |
| `CellSelectedColor` | `Color?` | Tap highlight color |
| `CellAccentColor` | `Color?` | Switches, checkboxes, radios |
| `CellIconSize` | `double` | Icon dimensions |
| `CellIconRadius` | `double` | Icon corner radius |
| `CellPadding` | `Thickness?` | Cell content padding |
| `CellBorderColor` | `Color?` | Cell border color |
| `CellBorderWidth` | `double` | Cell border width |
| `CellBorderRadius` | `double` | Cell border corner radius |

### Header/Footer Global Styles

| Property | Type | Default |
|---|---|---|
| `HeaderBackgroundColor` | `Color?` | `null` |
| `HeaderTextColor` | `Color?` | `null` |
| `HeaderFontSize` | `double` | `-1` |
| `HeaderFontFamily` | `string?` | `null` |
| `HeaderFontAttributes` | `FontAttributes` | `Bold` |
| `HeaderPadding` | `Thickness` | `14,8,8,8` |
| `HeaderHeight` | `double` | `-1` |
| `HeaderTextVerticalAlign` | `LayoutAlignment` | `End` |
| `FooterTextColor` | `Color?` | `null` |
| `FooterFontSize` | `double` | `-1` |
| `FooterFontAttributes` | `FontAttributes` | `None` |
| `FooterPadding` | `Thickness` | `14,8,8,8` |
| `FooterBackgroundColor` | `Color?` | `null` |

### Separator/Section Styles

| Property | Type | Default |
|---|---|---|
| `SeparatorColor` | `Color?` | `null` |
| `SeparatorHeight` | `double` | `0.5` |
| `SeparatorPadding` | `double` | `16` |
| `ShowSectionSeparator` | `bool` | `true` |
| `SectionSeparatorHeight` | `double` | `8` |
| `SectionSeparatorColor` | `Color?` | `null` |

## Drag & Sort

Enable reorder controls (up/down arrows) on a section:

```xml
<tv:TableView ItemDroppedCommand="{Binding ItemDroppedCommand}">
    <tv:TableRoot>
        <tv:TableSection Title="Reorder" UseDragSort="True">
            <tv:LabelCell Title="First" ValueText="1" />
            <tv:LabelCell Title="Second" ValueText="2" />
            <tv:LabelCell Title="Third" ValueText="3" />
        </tv:TableSection>
    </tv:TableRoot>
</tv:TableView>
```

The `ItemDroppedCommand` receives `ItemDroppedEventArgs` with `Section`, `Cell`, `FromIndex`, `ToIndex`.

## Scroll Control

```xml
<tv:TableView ScrollToTop="{Binding ShouldScrollTop}" ScrollToBottom="{Binding ShouldScrollBottom}" />
```

```csharp
await tableView.ScrollToTopAsync();
await tableView.ScrollToBottomAsync();
```

## Events

| Event | Args | Description |
|---|---|---|
| `ItemDropped` | `ItemDroppedEventArgs` | Cell reordered via drag sort |
| `ModelChanged` | `EventArgs` | Root/sections/cells changed |
| `CellPropertyChanged` | `CellPropertyChangedEventArgs` | Cell property changed |

## Code Generation Instructions

When generating code with Shiny.Maui.TableView:

### 1. Page Structure
- Always add `xmlns:tv="http://shiny.net/maui/tableview"` to the page
- Wrap content in `tv:TableView > tv:TableRoot > tv:TableSection`
- Group related settings into sections with descriptive `Title` and `FooterText`

### 2. Cell Selection
- Use `SwitchCell` for on/off toggles
- Use `CheckboxCell` for accept/agree checkboxes
- Use `SimpleCheckCell` for selection lists (shows/hides checkmark)
- Use `RadioCell` for mutually exclusive choices within a section
- Use `EntryCell` for text input
- Use `CommandCell` for navigation/action items with disclosure arrow
- Use `ButtonCell` for destructive or primary actions
- Use `LabelCell` for read-only display
- Use `DatePickerCell` / `TimePickerCell` for date/time selection
- Use `TextPickerCell` for dropdown selection from a list
- Use `NumberPickerCell` for numeric input with min/max
- Use `PickerCell` for full-page single or multi-select
- Use `CustomCell` for any custom MAUI view

### 3. Binding Patterns
- Always use `Mode=TwoWay` for editable properties (`On`, `Checked`, `ValueText`, `Date`, `Time`, `Number`, `SelectedIndex`, `SelectedItem`, `SelectedItems`)
- Use `Mode=OneWay` (default) for display-only properties (`Title`, `Description`, `ValueText` on LabelCell)
- Commands use default `Mode=OneWay`
- RadioCell selection binds at section level: `tv:RadioCell.SelectedValue="{Binding Prop, Mode=TwoWay}"`

### 4. ViewModel Pattern
```csharp
public class SettingsViewModel : INotifyPropertyChanged
{
    private bool _wifiEnabled = true;
    public bool WifiEnabled
    {
        get => _wifiEnabled;
        set => SetProperty(ref _wifiEnabled, value);
    }

    public ICommand SaveCommand { get; }

    public SettingsViewModel()
    {
        SaveCommand = new Command(async () =>
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Saved", "Settings saved.", "OK"));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }
}
```

### 5. Dark Mode
- Do NOT hardcode colors. Leave color properties as `null` to inherit system defaults.
- Only set explicit colors when the design requires specific brand colors.
- The control respects `Application.Current.UserAppTheme` automatically.

### 6. Styling Strategy
- Set global styles on `tv:TableView` for consistent appearance
- Override at section level for section-specific header/footer styling
- Override at cell level only for individual cell emphasis
- Use `CellAccentColor` for switches, checkboxes, and radio buttons globally

## Complete Example

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:tv="http://shiny.net/maui/tableview"
             x:Class="MyApp.SettingsPage"
             Title="Settings">

    <tv:TableView CellSelectedColor="#E0E0E0" CellAccentColor="#007AFF">
        <tv:TableRoot>
            <tv:TableSection Title="General">
                <tv:SwitchCell Title="Notifications" On="{Binding NotificationsOn, Mode=TwoWay}" />
                <tv:SwitchCell Title="Sound" On="{Binding SoundOn, Mode=TwoWay}" />
                <tv:CheckboxCell Title="Accept Analytics" Checked="{Binding AnalyticsAccepted, Mode=TwoWay}" />
            </tv:TableSection>

            <tv:TableSection Title="Account">
                <tv:EntryCell Title="Name" ValueText="{Binding Name, Mode=TwoWay}" Placeholder="Your name" />
                <tv:EntryCell Title="Email" ValueText="{Binding Email, Mode=TwoWay}" Keyboard="Email" />
                <tv:CommandCell Title="Change Password" Command="{Binding ChangePasswordCommand}" />
            </tv:TableSection>

            <tv:TableSection Title="Theme" tv:RadioCell.SelectedValue="{Binding Theme, Mode=TwoWay}">
                <tv:RadioCell Title="Light" Value="Light" />
                <tv:RadioCell Title="Dark" Value="Dark" />
                <tv:RadioCell Title="System" Value="System" />
            </tv:TableSection>

            <tv:TableSection Title="Preferences">
                <tv:DatePickerCell Title="Birthday" Date="{Binding Birthday, Mode=TwoWay}" Format="D" />
                <tv:TimePickerCell Title="Daily Reminder" Time="{Binding ReminderTime, Mode=TwoWay}" />
                <tv:NumberPickerCell Title="Font Size" Number="{Binding FontSize, Mode=TwoWay}"
                                      Min="10" Max="36" Unit="pt" />
            </tv:TableSection>

            <tv:TableSection Title="About">
                <tv:LabelCell Title="Version" ValueText="1.0.0" />
                <tv:CommandCell Title="Privacy Policy" Command="{Binding PrivacyCommand}" />
                <tv:CommandCell Title="Terms of Service" Command="{Binding TermsCommand}" />
            </tv:TableSection>

            <tv:TableSection Title="Actions">
                <tv:ButtonCell Title="Sign Out" Command="{Binding SignOutCommand}" ButtonTextColor="Red" />
            </tv:TableSection>
        </tv:TableRoot>
    </tv:TableView>
</ContentPage>
```

## Best Practices

1. **Group logically** - Put related settings in the same section with clear headers
2. **Use FooterText** - Explain non-obvious settings in section footers
3. **Two-way bind editable values** - Always `Mode=TwoWay` for user-editable properties
4. **Leave colors null for dark mode** - Only set colors when brand-specific styling is needed
5. **Use CellAccentColor globally** - Set once on TableView instead of per-cell AccentColor
6. **Use CommandCell for navigation** - With `ShowArrow="True"` and `KeepSelectedUntilBack="True"`
7. **Use ButtonCell for destructive actions** - Red text, centered, at the bottom of the page
8. **Use RadioCell for exclusive choices** - Bind `SelectedValue` at the section level
9. **Use PickerCell for long lists** - Full-page picker is better than inline for more than 4-5 items
10. **Use ItemTemplate for dynamic content** - Bind `ItemsSource` on sections for data-driven cells

## Reference Files

For detailed templates and examples, see:
- `reference/api-reference.md` - Full API surface and property tables
- `reference/templates.md` - Page and cell code generation templates
