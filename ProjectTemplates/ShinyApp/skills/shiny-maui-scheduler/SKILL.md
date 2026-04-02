---
name: shiny-maui-scheduler
description: Generate .NET MAUI scheduler views, event providers, and calendar/agenda/list components using Shiny.Maui.Scheduler
auto_invoke: true
triggers:
  - SchedulerAgendaView
  - ISchedulerEventProvider
  - Shiny.Maui.Scheduler
  - SchedulerCalendarView
  - SchedulerCalendarListView
---

# Shiny MAUI Scheduler Skill

You are an expert in Shiny.Maui.Scheduler, a .NET MAUI component library providing calendar, agenda timeline, and event list views.

## When to Use This Skill

Invoke this skill when the user wants to:
- Create scheduler views (calendar, agenda, or event list) in a MAUI app
- Implement `ISchedulerEventProvider` to supply event data
- Customize event templates or day header templates for scheduler views
- Configure `SchedulerCalendarView`, `SchedulerAgendaView`, or `SchedulerCalendarListView`
- Set up infinite scrolling event lists grouped by day
- Create ViewModels that bind to scheduler views
- Understand the scheduler library's architecture or API surface

## Library Overview

**GitHub**: https://github.com/shinyorg/scheduler
**NuGet**: `Shiny.Maui.Scheduler`
**Namespace**: `Shiny.Maui.Scheduler`
**Targets**: net10.0-android, net10.0-ios, net10.0-maccatalyst, net10.0-windows

Shiny.Maui.Scheduler provides three views that share a common `ISchedulerEventProvider` interface:

1. **SchedulerCalendarView** — Monthly calendar grid with swipe navigation
2. **SchedulerAgendaView** — Day/multi-day timeline with overlap detection
3. **SchedulerCalendarListView** — Vertically scrolling event list grouped by day with infinite scroll

All views are built programmatically (no XAML internals), use AOT-safe lambda bindings, and support custom DataTemplates.

## Setup

### 1. Install NuGet Package
```bash
dotnet add package Shiny.Maui.Scheduler
```

### 2. Configure in MauiProgram.cs
```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShinyScheduler();
```

## Core Interfaces and Models

### ISchedulerEventProvider

```csharp
public interface ISchedulerEventProvider
{
    Task<IReadOnlyList<SchedulerEvent>> GetEvents(DateTimeOffset start, DateTimeOffset end);
    void OnEventSelected(SchedulerEvent selectedEvent);
    bool CanCalendarSelect(DateOnly selectedDate);
    void OnCalendarDateSelected(DateOnly selectedDate);
    void OnAgendaTimeSelected(DateTimeOffset selectedTime);
    bool CanSelectAgendaTime(DateTimeOffset selectedTime);
}
```

### SchedulerEvent

```csharp
public class SchedulerEvent
{
    public string Identifier { get; set; }   // Default: new Guid
    public string Title { get; set; }
    public string? Description { get; set; }
    public Color? Color { get; set; }
    public bool IsAllDay { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
```

### CalendarListDayGroup

Used for custom `DayHeaderTemplate` bindings in `SchedulerCalendarListView`:

```csharp
public class CalendarListDayGroup : List<SchedulerEvent>
{
    public DateOnly Date { get; }
    public string DateDisplay { get; }       // "dddd, MMMM d, yyyy"
    public bool IsToday { get; }
    public string EventCountDisplay { get; } // "3 events"
}
```

### DatePickerItemContext

Used for custom `DayPickerItemTemplate` bindings in `SchedulerAgendaView`:

```csharp
public class DatePickerItemContext
{
    public DateOnly Date { get; set; }
    public string DayNumber { get; set; }   // "30"
    public string DayName { get; set; }     // "MON"
    public string MonthName { get; set; }   // "MAR"
    public bool IsSelected { get; set; }
    public bool IsToday { get; set; }
}
```

### CalendarOverflowContext

Used for custom `OverflowItemTemplate` bindings in `SchedulerCalendarView`:

```csharp
public class CalendarOverflowContext
{
    public int EventCount { get; set; }
    public DateOnly Date { get; set; }
}
```

## Code Generation Instructions

When generating code for Shiny.Maui.Scheduler projects, follow these conventions:

### 1. AOT-Safe Bindings

**CRITICAL**: Never use string-based `SetBinding()`. Always use the static lambda overload:

```csharp
// Correct - AOT safe
label.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Title);

// WRONG - not AOT safe
label.SetBinding(Label.TextProperty, "Title");
```

### 2. Event Provider Implementation

Always implement all methods of `ISchedulerEventProvider`:

```csharp
public class MyEventProvider : ISchedulerEventProvider
{
    public async Task<IReadOnlyList<SchedulerEvent>> GetEvents(DateTimeOffset start, DateTimeOffset end)
    {
        // Fetch events from your data source for the given range
        // Multi-day events should be included if they overlap the range at all
        return events;
    }

    public void OnEventSelected(SchedulerEvent selectedEvent)
    {
        // Handle event taps — navigate to detail, show dialog, etc.
    }

    public bool CanCalendarSelect(DateOnly selectedDate) => true;
    public void OnCalendarDateSelected(DateOnly selectedDate) { }
    public void OnAgendaTimeSelected(DateTimeOffset selectedTime) { }
    public bool CanSelectAgendaTime(DateTimeOffset selectedTime) => true;
}
```

Pass the provider to views via the `Provider` bindable property — it does not need to be registered in DI.

### 3. ViewModels

Use `INotifyPropertyChanged` (not ObservableObject from CommunityToolkit — this library has no CommunityToolkit dependency):

```csharp
public class MySchedulerViewModel : INotifyPropertyChanged
{
    DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

    public MySchedulerViewModel(ISchedulerEventProvider provider)
    {
        Provider = provider;
    }

    public ISchedulerEventProvider Provider { get; }

    public DateOnly SelectedDate
    {
        get => _selectedDate;
        set { _selectedDate = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

Register ViewModels and Pages as transient:
```csharp
builder.Services.AddTransient<MyViewModel>();
builder.Services.AddTransient<MyPage>();
```

### 4. Pages

Use XAML with `x:DataType` for compile-time binding:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:scheduler="clr-namespace:Shiny.Maui.Scheduler;assembly=Shiny.Maui.Scheduler"
             xmlns:vm="clr-namespace:MyApp.ViewModels"
             x:Class="MyApp.Pages.MyPage"
             x:DataType="vm:MyViewModel"
             Title="My Page">

    <scheduler:SchedulerCalendarListView
        Provider="{Binding Provider}"
        SelectedDate="{Binding SelectedDate}" />

</ContentPage>
```

Code-behind with constructor injection:
```csharp
public partial class MyPage : ContentPage
{
    public MyPage(MyViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}
```

## View Properties Reference

### SchedulerCalendarView

| Property | Type | Default |
|----------|------|---------|
| Provider | ISchedulerEventProvider? | null |
| SelectedDate | DateOnly | Today (TwoWay) |
| DisplayMonth | DateOnly | Today (TwoWay) |
| MinDate | DateOnly? | null |
| MaxDate | DateOnly? | null |
| ShowCalendarCellEventCountOnly | bool | false |
| EventItemTemplate | DataTemplate? | null |
| OverflowItemTemplate | DataTemplate? | null |
| LoaderTemplate | DataTemplate? | null |
| MaxEventsPerCell | int | 3 |
| CalendarCellColor | Color | White |
| CalendarCellSelectedColor | Color | LightBlue |
| CurrentDayColor | Color | DodgerBlue |
| FirstDayOfWeek | DayOfWeek | Sunday |
| AllowPan | bool | true |
| AllowZoom | bool | false |

### SchedulerAgendaView

| Property | Type | Default |
|----------|------|---------|
| Provider | ISchedulerEventProvider? | null |
| SelectedDate | DateOnly | Today (TwoWay) |
| MinDate | DateOnly? | null |
| MaxDate | DateOnly? | null |
| DaysToShow | int | 1 (clamped 1-7) |
| ShowCarouselDatePicker | bool | true |
| ShowCurrentTimeMarker | bool | true |
| Use24HourTime | bool | true |
| EventItemTemplate | DataTemplate? | null |
| LoaderTemplate | DataTemplate? | null |
| DayPickerItemTemplate | DataTemplate? | null |
| CurrentTimeMarkerColor | Color | Red |
| TimezoneColor | Color | Gray |
| SeparatorColor | Color | LightGray |
| DefaultEventColor | Color | CornflowerBlue |
| TimeSlotHeight | double | 60.0 |
| AllowPan | bool | true |
| AllowZoom | bool | false |
| ShowAdditionalTimezones | bool | false |
| AdditionalTimezones | `IList<TimeZoneInfo>` | empty |

### SchedulerCalendarListView

| Property | Type | Default |
|----------|------|---------|
| Provider | ISchedulerEventProvider? | null |
| SelectedDate | DateOnly | Today (TwoWay) |
| MinDate | DateOnly? | null |
| MaxDate | DateOnly? | null |
| EventItemTemplate | DataTemplate? | null |
| DayHeaderTemplate | DataTemplate? | null |
| LoaderTemplate | DataTemplate? | null |
| DaysPerPage | int | 30 |
| DefaultEventColor | Color | CornflowerBlue |
| DayHeaderBackgroundColor | Color | Transparent |
| DayHeaderTextColor | Color | Black |
| AllowPan | bool | true |
| AllowZoom | bool | false |

## Default Templates

The `DefaultTemplates` static class provides reusable templates:

| Method | Binds To |
|--------|----------|
| `CreateEventItemTemplate()` | `SchedulerEvent` — Color bar + title |
| `CreateOverflowTemplate()` | `CalendarOverflowContext` — "+N more" |
| `CreateLoaderTemplate()` | None — ActivityIndicator + "Loading..." |
| `CreateCalendarListDayHeaderTemplate()` | `CalendarListDayGroup` — Accent bar + today dot + bold date + event count |
| `CreateCalendarListEventItemTemplate()` | `SchedulerEvent` — Card with color bar, title, description, start-end time range |
| `CreateAppleCalendarDayPickerTemplate()` | `DatePickerItemContext` — Apple Calendar-style day picker (default for Agenda) |

### Custom Template Example

```csharp
var myEventTemplate = new DataTemplate(() =>
{
    var grid = new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(new GridLength(4)),
            new ColumnDefinition(GridLength.Star)
        },
        Padding = new Thickness(8),
        ColumnSpacing = 8
    };

    var colorBar = new BoxView { CornerRadius = 2 };
    colorBar.SetBinding(BoxView.ColorProperty, static (SchedulerEvent e) => e.Color);

    var title = new Label { FontSize = 14 };
    title.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Title);

    grid.Add(colorBar, 0);
    grid.Add(title, 1);
    return grid;
});
```

## Important Notes

- All three views extend `ContentView` — they can be placed anywhere in your XAML layout
- `SelectedDate` is `TwoWay` on all views — changes propagate back to the ViewModel
- All views support `MinDate`/`MaxDate` to constrain navigation and selection
- All views support `AllowPan` and `AllowZoom` to control gesture interactions
- All views support `LoaderTemplate` for custom loading indicators (defaults to ActivityIndicator + "Loading...")
- The provider's `GetEvents()` may be called with any date range — implement it to handle arbitrary ranges
- Multi-day events should span the full range (the views handle per-day duplication internally)
- The `SchedulerCalendarListView` skips empty days (only days with events are shown)
- The `SchedulerAgendaView` uses an Apple Calendar-style day picker by default; override with `DayPickerItemTemplate` using `DatePickerItemContext`
- The `SchedulerAgendaView` supports 24-hour and 12-hour (AM/PM) time via `Use24HourTime`; this affects time labels, event time text, and the current time marker
- The `SchedulerAgendaView` supports multiple timezone columns via `AdditionalTimezones` + `ShowAdditionalTimezones`; in multi-day view, time labels appear once on the left with sticky per-day date headers above each column; timezone abbreviations only show in headers when multiple timezones are visible
- The `SchedulerAgendaView` separator lines are styleable via `SeparatorColor`
- All-day events always sort to top within their day group
- Custom templates must use AOT-safe static lambda bindings, never string-based bindings
- `AdditionalTimezones` is an `IList<TimeZoneInfo>` — add timezones in code-behind: `AgendaView.AdditionalTimezones.Add(TimeZoneInfo.FindSystemTimeZoneById("America/New_York"))`
