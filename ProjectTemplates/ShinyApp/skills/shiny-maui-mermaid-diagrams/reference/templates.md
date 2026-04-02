# Shiny MAUI Diagrams Code Templates

## Basic Diagram Page

### XAML

```xml
<!-- Pages/{Name}Page.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             x:Class="{Namespace}.Pages.{Name}Page"
             Title="{Title}">
    <diagram:MermaidDiagramControl
        DiagramText="graph TD; A[Start] --> B{Decision}; B -->|Yes| C[Action]; B -->|No| D[End];" />
</ContentPage>
```

### Code-Behind

```csharp
// Pages/{Name}Page.xaml.cs
namespace {Namespace}.Pages;

public partial class {Name}Page : ContentPage
{
    public {Name}Page()
    {
        InitializeComponent();
    }
}
```

## Data-Bound Diagram Page

### XAML

```xml
<!-- Pages/{Name}Page.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             x:Class="{Namespace}.Pages.{Name}Page"
             Title="{Title}">
    <Grid RowDefinitions="*,Auto">
        <diagram:MermaidDiagramControl
            x:Name="diagram"
            DiagramText="{Binding MermaidText}" />

        <Label Grid.Row="1"
               Text="{Binding Source={x:Reference diagram}, Path=ParseError}"
               TextColor="Red"
               Padding="10">
            <Label.Triggers>
                <DataTrigger TargetType="Label"
                             Binding="{Binding Source={x:Reference diagram}, Path=ParseError}"
                             Value="{x:Null}">
                    <Setter Property="IsVisible" Value="False" />
                </DataTrigger>
                <DataTrigger TargetType="Label"
                             Binding="{Binding Source={x:Reference diagram}, Path=ParseError}"
                             Value="">
                    <Setter Property="IsVisible" Value="False" />
                </DataTrigger>
            </Label.Triggers>
        </Label>
    </Grid>
</ContentPage>
```

### ViewModel

```csharp
// ViewModels/{Name}ViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace {Namespace}.ViewModels;

public partial class {Name}ViewModel : ObservableObject
{
    [ObservableProperty]
    string mermaidText = @"graph TD
    A[Start] --> B{Is it working?}
    B -->|Yes| C[Great!]
    B -->|No| D[Debug]
    D --> B";
}
```

## Interactive Editor Page

### XAML

```xml
<!-- Pages/EditorPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             x:Class="{Namespace}.Pages.EditorPage"
             Title="Diagram Editor">
    <Grid RowDefinitions="*,Auto,200">
        <diagram:MermaidDiagramControl
            x:Name="diagram"
            DiagramText="{Binding Source={x:Reference editor}, Path=Text}" />

        <Label Grid.Row="1"
               Text="{Binding Source={x:Reference diagram}, Path=ParseError}"
               TextColor="Red"
               Padding="10,5" />

        <Editor Grid.Row="2"
                x:Name="editor"
                Placeholder="Enter Mermaid flowchart text..."
                FontFamily="Courier"
                Text="graph TD; A[Start] --> B[End];" />
    </Grid>
</ContentPage>
```

## Themed Diagram Page

### XAML

```xml
<!-- Pages/ThemedPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             xmlns:theming="clr-namespace:Shiny.Maui.MermaidDiagrams.Theming;assembly=Shiny.Maui.MermaidDiagrams"
             x:Class="{Namespace}.Pages.ThemedPage"
             Title="Themed Diagram">
    <ContentPage.Resources>
        <theming:DarkTheme x:Key="DarkTheme" />
        <theming:ForestTheme x:Key="ForestTheme" />
        <theming:NeutralTheme x:Key="NeutralTheme" />
    </ContentPage.Resources>

    <Grid RowDefinitions="Auto,*">
        <Picker x:Name="themePicker"
                Title="Select Theme"
                SelectedIndexChanged="OnThemeChanged">
            <Picker.ItemsSource>
                <x:Array Type="{x:Type x:String}">
                    <x:String>Default</x:String>
                    <x:String>Dark</x:String>
                    <x:String>Forest</x:String>
                    <x:String>Neutral</x:String>
                </x:Array>
            </Picker.ItemsSource>
        </Picker>

        <diagram:MermaidDiagramControl
            Grid.Row="1"
            x:Name="diagram"
            DiagramText="graph TD; A[Web App] --> B[API]; B --> C[(Database)]; B --> D[Cache];" />
    </Grid>
</ContentPage>
```

### Code-Behind

```csharp
// Pages/ThemedPage.xaml.cs
using Shiny.Maui.MermaidDiagrams.Theming;

namespace {Namespace}.Pages;

public partial class ThemedPage : ContentPage
{
    public ThemedPage()
    {
        InitializeComponent();
    }

    void OnThemeChanged(object sender, EventArgs e)
    {
        diagram.Theme = themePicker.SelectedIndex switch
        {
            1 => new DarkTheme(),
            2 => new ForestTheme(),
            3 => new NeutralTheme(),
            _ => new DefaultTheme()
        };
    }
}
```

## Custom Layout Options Page

### XAML

```xml
<!-- Pages/CustomLayoutPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             xmlns:layout="clr-namespace:Shiny.Maui.MermaidDiagrams.Layout;assembly=Shiny.Maui.MermaidDiagrams"
             x:Class="{Namespace}.Pages.CustomLayoutPage"
             Title="Custom Layout">
    <diagram:MermaidDiagramControl
        DiagramText="graph LR; A --> B --> C --> D;">
        <diagram:MermaidDiagramControl.LayoutOptions>
            <layout:DiagramLayoutOptions
                NodeWidth="120"
                NodeHeight="40"
                HorizontalSpacing="50"
                VerticalSpacing="60"
                FontSize="12"
                Margin="30" />
        </diagram:MermaidDiagramControl.LayoutOptions>
    </diagram:MermaidDiagramControl>
</ContentPage>
```

## Subgraph Diagram Page

### XAML

```xml
<!-- Pages/SubgraphPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             x:Class="{Namespace}.Pages.SubgraphPage"
             Title="Subgraphs">
    <diagram:MermaidDiagramControl>
        <diagram:MermaidDiagramControl.DiagramText>
graph TD
    subgraph Frontend
        A[React App]
        B[Mobile App]
    end
    subgraph Backend
        C[API Gateway]
        D[Auth Service]
        E[Data Service]
    end
    subgraph Storage
        F[(PostgreSQL)]
        G[(Redis)]
    end
    A --> C
    B --> C
    C --> D
    C --> E
    E --> F
    E --> G
        </diagram:MermaidDiagramControl.DiagramText>
    </diagram:MermaidDiagramControl>
</ContentPage>
```

## Programmatic Usage Template

### Parse and Inspect

```csharp
using Shiny.Maui.MermaidDiagrams.Parsing;
using Shiny.Maui.MermaidDiagrams.Models;

// Parse Mermaid text
var model = MermaidParser.Parse(@"
    graph TD
    A[Start] --> B{Decision}
    B -->|Yes| C[Action]
    B -->|No| D[Skip]
    C --> E[End]
    D --> E
");

// Inspect the model
Console.WriteLine($"Direction: {model.Direction}");
Console.WriteLine($"Nodes: {model.Nodes.Count}");
Console.WriteLine($"Edges: {model.Edges.Count}");

foreach (var node in model.Nodes)
    Console.WriteLine($"  {node.Id}: {node.Label} ({node.Shape})");

foreach (var edge in model.Edges)
    Console.WriteLine($"  {edge.SourceId} -> {edge.TargetId} ({edge.Style}, {edge.ArrowType})");
```

### Layout and Measure

```csharp
using Shiny.Maui.MermaidDiagrams.Layout;

var engine = new SugiyamaLayoutEngine();
var options = new DiagramLayoutOptions
{
    NodeWidth = 150,
    NodeHeight = 50,
    HorizontalSpacing = 60,
    VerticalSpacing = 80
};

var result = engine.Layout(model, options);

Console.WriteLine($"Total bounds: {result.TotalBounds}");

foreach (var nl in result.NodeLayouts)
    Console.WriteLine($"  {nl.NodeId}: ({nl.Bounds.X}, {nl.Bounds.Y}) {nl.Bounds.Width}x{nl.Bounds.Height}");

foreach (var er in result.EdgeRoutes)
    Console.WriteLine($"  {er.SourceId} -> {er.TargetId}: {er.Points.Count} points");
```

## Custom Theme Template

```csharp
using Shiny.Maui.MermaidDiagrams.Theming;

namespace {Namespace}.Themes;

public class {Name}Theme : DiagramTheme
{
    public override Color BackgroundColor => {backgroundColor};
    public override Color TextColor => {textColor};
    public override float FontSize => 14;

    public override NodeRenderStyle DefaultNodeStyle => new()
    {
        FillColor = {nodeFillColor},
        StrokeColor = {nodeStrokeColor},
        TextColor = {nodeTextColor},
        StrokeWidth = 2,
        CornerRadius = 4
    };

    public override EdgeRenderStyle DefaultEdgeStyle => new()
    {
        StrokeColor = {edgeStrokeColor},
        StrokeWidth = 2,
        LabelBackgroundColor = {labelBgColor},
        LabelTextColor = {labelTextColor}
    };

    public override SubgraphRenderStyle DefaultSubgraphStyle => new()
    {
        FillColor = {subgraphFillColor},
        StrokeColor = {subgraphStrokeColor},
        TitleColor = {subgraphTitleColor},
        StrokeWidth = 1.5f,
        CornerRadius = 8
    };

    // Optional: vary node colors by index
    public override NodeRenderStyle GetNodeStyle(int index) => DefaultNodeStyle;
}
```

## Common Mermaid Flowchart Patterns

### Decision Flow

```
graph TD
    Start[Start] --> Check{Condition?}
    Check -->|Yes| Action[Do Something]
    Check -->|No| Skip[Skip It]
    Action --> End[Done]
    Skip --> End
```

### Pipeline / CI-CD

```
graph LR
    Build[Build] --> Test[Test]
    Test --> Lint[Lint]
    Lint --> Deploy{Deploy?}
    Deploy -->|Staging| Staging[Staging]
    Deploy -->|Production| Prod[Production]
```

### Microservices Architecture

```
graph TD
    subgraph Client
        Web[Web App]
        Mobile[Mobile App]
    end
    subgraph API
        Gateway[API Gateway]
        Auth[Auth Service]
        Users[User Service]
        Orders[Order Service]
    end
    subgraph Data
        DB[(Database)]
        Cache[(Redis)]
        Queue[Message Queue]
    end
    Web --> Gateway
    Mobile --> Gateway
    Gateway --> Auth
    Gateway --> Users
    Gateway --> Orders
    Users --> DB
    Orders --> DB
    Orders --> Queue
    Gateway --> Cache
```

### Error Handling Flow

```
graph TD
    Request[Request] --> Validate{Valid?}
    Validate -->|Yes| Process[Process]
    Validate -->|No| Error[Return Error]
    Process --> Result{Success?}
    Result -->|Yes| Response[Return Response]
    Result -->|No| Retry{Retries Left?}
    Retry -->|Yes| Process
    Retry -->|No| Error
```
