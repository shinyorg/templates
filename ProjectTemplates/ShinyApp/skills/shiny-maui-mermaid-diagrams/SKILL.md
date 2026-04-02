---
name: shiny-maui-mermaiddiagrams
description: Generate .NET MAUI pages with Mermaid flowchart diagram rendering using Shiny MAUI Diagrams
auto_invoke: true
triggers:
  - maui diagram
  - mermaid diagram
  - mermaid flowchart
  - diagram control
  - flowchart control
  - MermaidDiagramControl
  - DiagramText
  - DiagramTheme
  - Shiny.Maui.MermaidDiagrams
  - MermaidParser
  - SugiyamaLayoutEngine
  - DiagramModel
  - DiagramLayoutOptions
  - NodeShape
  - EdgeStyle
  - SubgraphModel
  - DefaultTheme
  - DarkTheme
  - ForestTheme
  - NeutralTheme
  - graph TD
  - graph LR
  - flowchart
---

# Shiny MAUI Diagrams Skill

You are an expert in Shiny MAUI Diagrams, a library that renders Mermaid JS flowchart diagrams natively in .NET MAUI using pure MAUI Graphics (IDrawable/ICanvas). No WebView, no SkiaSharp, no reflection — fully AOT-compliant.

## When to Use This Skill

Invoke this skill when the user wants to:
- Add a Mermaid flowchart diagram to a MAUI page
- Set up or configure Shiny MAUI Diagrams in their application
- Customize diagram theming (colors, styles)
- Parse Mermaid syntax programmatically
- Use the layout engine independently
- Create custom themes for diagrams
- Handle diagram parse errors
- Add pan/zoom interaction to diagrams
- Display flowcharts, graphs, or node-edge diagrams in MAUI

## Library Overview

**GitHub**: https://github.com/shinyorg/diagrams
**Namespace**: `Shiny.Maui.MermaidDiagrams`
**XAML Namespace**: `http://shiny.net/maui/diagrams`

Shiny MAUI Diagrams provides:
- A `MermaidDiagramControl` that renders Mermaid flowchart text as native MAUI graphics
- Hand-written lexer and recursive-descent parser for Mermaid syntax
- Sugiyama layered graph layout algorithm (same family as Graphviz, dagre)
- 4 built-in themes (Default, Dark, Forest, Neutral)
- Pan and pinch-to-zoom gesture support
- AOT-compliant — no reflection, no WebView, no SkiaSharp

## Setup

### 1. Install the NuGet Package

```bash
dotnet add package Shiny.Maui.MermaidDiagrams
```

### 2. Add XAML Namespace

```xml
xmlns:diagram="http://shiny.net/maui/diagrams"
```

No builder configuration is required — simply install the package, add the XAML namespace, and use the control.

## Code Generation Instructions

When generating code for Shiny MAUI Diagrams projects, follow these conventions:

### 1. Basic Diagram Page

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:diagram="http://shiny.net/maui/diagrams"
             x:Class="MyApp.DiagramPage"
             Title="Diagram">
    <diagram:MermaidDiagramControl
        DiagramText="graph TD; A[Start] --> B{Decision}; B -->|Yes| C[Do It]; B -->|No| D[Skip];" />
</ContentPage>
```

### 2. Data-Bound Diagram

Bind `DiagramText` to a ViewModel property for dynamic updates:

```csharp
public partial class DiagramViewModel : ObservableObject
{
    [ObservableProperty]
    string mermaidText = "graph TD; A-->B; B-->C;";
}
```

```xml
<diagram:MermaidDiagramControl
    DiagramText="{Binding MermaidText}" />
```

### 3. Themed Diagram

Apply built-in or custom themes:

```xml
<!-- Using a resource -->
<ContentPage.Resources>
    <diagram:DarkTheme x:Key="DarkTheme" />
</ContentPage.Resources>

<diagram:MermaidDiagramControl
    DiagramText="{Binding MermaidText}"
    Theme="{StaticResource DarkTheme}" />
```

Available themes:
- `DefaultTheme` — Blue/purple (Mermaid.js default)
- `DarkTheme` — Dark navy with red accents
- `ForestTheme` — Natural green tones
- `NeutralTheme` — Light gray, minimal

### 4. Error Handling

The `ParseError` property exposes any parse failure:

```xml
<VerticalStackLayout>
    <diagram:MermaidDiagramControl
        x:Name="diagram"
        DiagramText="{Binding MermaidText}" />

    <Label Text="{Binding Source={x:Reference diagram}, Path=ParseError}"
           TextColor="Red" />
</VerticalStackLayout>
```

### 5. Custom Layout Options

Adjust spacing and sizing:

```xml
<diagram:MermaidDiagramControl DiagramText="{Binding MermaidText}">
    <diagram:MermaidDiagramControl.LayoutOptions>
        <diagram:DiagramLayoutOptions
            NodeWidth="120"
            NodeHeight="40"
            HorizontalSpacing="50"
            VerticalSpacing="70"
            FontSize="12" />
    </diagram:MermaidDiagramControl.LayoutOptions>
</diagram:MermaidDiagramControl>
```

### 6. Programmatic Parsing

Use the parser and layout engine independently:

```csharp
using Shiny.Maui.MermaidDiagrams.Parsing;
using Shiny.Maui.MermaidDiagrams.Layout;

var model = MermaidParser.Parse("graph TD; A-->B; B-->C;");
var engine = new SugiyamaLayoutEngine();
var result = engine.Layout(model, new DiagramLayoutOptions());
// result.NodeLayouts, result.EdgeRoutes, result.SubgraphLayouts
```

### 7. Custom Themes

Subclass `DiagramTheme`:

```csharp
using Shiny.Maui.MermaidDiagrams.Theming;

public class MyTheme : DiagramTheme
{
    public override Color BackgroundColor => Colors.White;
    public override Color TextColor => Colors.Black;
    public override float FontSize => 14;

    public override NodeRenderStyle DefaultNodeStyle => new()
    {
        FillColor = Color.FromArgb("#E3F2FD"),
        StrokeColor = Color.FromArgb("#1565C0"),
        TextColor = Colors.Black
    };

    public override EdgeRenderStyle DefaultEdgeStyle => new()
    {
        StrokeColor = Color.FromArgb("#757575"),
        LabelBackgroundColor = Colors.White,
        LabelTextColor = Colors.Black
    };

    public override SubgraphRenderStyle DefaultSubgraphStyle => new()
    {
        FillColor = Color.FromArgb("#F5F5F5"),
        StrokeColor = Color.FromArgb("#BDBDBD"),
        TitleColor = Colors.Black
    };
}
```

## Mermaid Syntax Supported

### Directions
- `graph TD` / `graph TB` — Top to bottom (default)
- `graph BT` — Bottom to top
- `graph LR` — Left to right
- `graph RL` — Right to left
- `flowchart` keyword also supported

### Node Shapes
| Syntax | Shape |
|--------|-------|
| `A[text]` | Rectangle |
| `A(text)` | Rounded rectangle |
| `A([text])` | Stadium |
| `A((text))` | Circle |
| `A{text}` | Diamond |
| `A{{text}}` | Hexagon |

### Edge Types
| Syntax | Style |
|--------|-------|
| `-->` | Solid arrow |
| `---` | Solid line |
| `-.->` | Dotted arrow |
| `-.-` | Dotted line |
| `==>` | Thick arrow |
| `===` | Thick line |

### Edge Labels
```
A -->|label text| B
```

### Subgraphs
```
subgraph Title
    A --> B
end
```

### Comments
```
%% This is a comment
```

### Edge Chaining
```
A --> B --> C --> D
```

## Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DiagramText` | `string` | `""` | Mermaid flowchart text |
| `Theme` | `DiagramTheme` | `DefaultTheme` | Visual theme |
| `ZoomLevel` | `float` | `1.0` | Zoom scale |
| `ParseError` | `string?` | `null` | Parse error message |
| `LayoutOptions` | `DiagramLayoutOptions` | defaults | Layout configuration |

## Best Practices

1. **Bind DiagramText** — Use data binding for dynamic diagram updates
2. **Check ParseError** — Display parse errors to help users fix Mermaid syntax
3. **Use themes** — Apply built-in themes or create custom ones for consistent styling
4. **Adjust layout options** — Tune `NodeWidth`, `VerticalSpacing`, etc. for your content density
5. **Use subgraphs** — Group related nodes visually with `subgraph ... end`
6. **Edge chaining** — Use `A --> B --> C` instead of separate edge declarations for cleaner syntax
7. **Semicolons or newlines** — Separate statements with `;` for inline text or newlines for readability

## Reference Files

For detailed API surface and code templates, see:
- `reference/api-reference.md` — Full API surface, classes, and properties
- `reference/templates.md` — Page templates and code patterns
