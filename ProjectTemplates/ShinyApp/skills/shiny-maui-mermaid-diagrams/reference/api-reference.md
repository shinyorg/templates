# API Reference

## XAML Namespace

```xml
xmlns:diagram="http://shiny.net/maui/diagrams"
```

All public types are available under this XAML namespace.

## C# Namespaces

```csharp
using Shiny.Maui.MermaidDiagrams.Controls;    // MermaidDiagramControl
using Shiny.Maui.MermaidDiagrams.Models;      // DiagramModel, DiagramNode, DiagramEdge, etc.
using Shiny.Maui.MermaidDiagrams.Parsing;     // MermaidParser, MermaidLexer
using Shiny.Maui.MermaidDiagrams.Layout;      // ILayoutEngine, SugiyamaLayoutEngine, DiagramLayoutOptions
using Shiny.Maui.MermaidDiagrams.Rendering;   // DiagramDrawable, NodeRenderer, EdgeRenderer
using Shiny.Maui.MermaidDiagrams.Theming;     // DiagramTheme, DefaultTheme, DarkTheme, etc.
```

## MermaidDiagramControl

The primary control for rendering Mermaid diagrams. Inherits from `ContentView`.

```csharp
namespace Shiny.Maui.MermaidDiagrams.Controls;

public partial class MermaidDiagramControl : ContentView
{
    // The Mermaid flowchart text to parse and render
    public string DiagramText { get; set; }

    // The visual theme applied to the diagram
    public DiagramTheme Theme { get; set; }

    // The zoom scale factor (1.0 = 100%)
    public float ZoomLevel { get; set; }

    // Parse error message if the Mermaid text is invalid (null when valid)
    public string? ParseError { get; set; }

    // Layout spacing and sizing configuration
    public DiagramLayoutOptions LayoutOptions { get; set; }
}
```

### Bindable Properties

| Property | Type | Default | BindingMode |
|----------|------|---------|-------------|
| `DiagramTextProperty` | `string` | `""` | OneWay |
| `ThemeProperty` | `DiagramTheme` | `DefaultTheme` | OneWay |
| `ZoomLevelProperty` | `float` | `1.0` | OneWay |
| `ParseErrorProperty` | `string?` | `null` | OneWay |
| `LayoutOptionsProperty` | `DiagramLayoutOptions` | defaults | OneWay |

### Behavior

- Setting `DiagramText` triggers parsing → layout → rendering automatically
- Invalid Mermaid text sets `ParseError` and clears the diagram
- Setting `Theme` or `LayoutOptions` re-renders the diagram
- Pan and pinch-to-zoom gestures are built in

## MermaidParser

Static parser that converts Mermaid flowchart text into a `DiagramModel`.

```csharp
namespace Shiny.Maui.MermaidDiagrams.Parsing;

public sealed class MermaidParser
{
    // Parse Mermaid flowchart text into a DiagramModel
    // Throws ParseException on invalid syntax
    public static DiagramModel Parse(string mermaidText);
}
```

### Usage

```csharp
var model = MermaidParser.Parse("graph TD; A-->B; B-->C;");
// model.Direction == DiagramDirection.TopToBottom
// model.Nodes: [A, B, C]
// model.Edges: [A→B, B→C]
```

## MermaidLexer

Character-by-character scanner that tokenizes Mermaid text.

```csharp
namespace Shiny.Maui.MermaidDiagrams.Parsing;

public sealed class MermaidLexer
{
    public MermaidLexer(string source);
    public List<MermaidToken> Tokenize();
}
```

## MermaidToken

```csharp
namespace Shiny.Maui.MermaidDiagrams.Parsing;

public readonly record struct MermaidToken(
    MermaidTokenType Type,
    string Value,
    int Line,
    int Column
);
```

## MermaidTokenType

```csharp
public enum MermaidTokenType
{
    // Keywords
    Graph, Flowchart, Subgraph, End, Direction,

    // Delimiters
    OpenBracket, CloseBracket, OpenParen, CloseParen,
    OpenBrace, CloseBrace, Pipe, Semicolon, Newline,

    // Edges
    Arrow,        // -->
    Line,         // ---
    DottedArrow,  // -.->
    DottedLine,   // -.-
    ThickArrow,   // ==>
    ThickLine,    // ===

    // Values
    Identifier, QuotedString,

    // End
    Eof
}
```

## ParseException

```csharp
namespace Shiny.Maui.MermaidDiagrams.Parsing;

public sealed class ParseException : Exception
{
    public int Line { get; }
    public int Column { get; }
}
```

## DiagramModel

The root model produced by parsing.

```csharp
namespace Shiny.Maui.MermaidDiagrams.Models;

public sealed class DiagramModel
{
    public DiagramDirection Direction { get; set; }
    public List<DiagramNode> Nodes { get; }
    public List<DiagramEdge> Edges { get; }
    public List<SubgraphModel> Subgraphs { get; }

    // Get an existing node by ID, or create a new one
    public DiagramNode GetOrAddNode(string id, string? label = null, NodeShape shape = NodeShape.Rectangle);
}
```

## DiagramNode

```csharp
namespace Shiny.Maui.MermaidDiagrams.Models;

public sealed class DiagramNode
{
    public required string Id { get; init; }
    public string Label { get; set; }
    public NodeShape Shape { get; set; }
    public string? SubgraphId { get; set; }
}
```

## DiagramEdge

```csharp
namespace Shiny.Maui.MermaidDiagrams.Models;

public sealed class DiagramEdge
{
    public required string SourceId { get; init; }
    public required string TargetId { get; init; }
    public string? Label { get; set; }
    public EdgeStyle Style { get; set; }
    public ArrowType ArrowType { get; set; }
}
```

## SubgraphModel

```csharp
namespace Shiny.Maui.MermaidDiagrams.Models;

public sealed class SubgraphModel
{
    public required string Id { get; init; }
    public string Title { get; set; }
    public List<string> NodeIds { get; }
    public string? ParentSubgraphId { get; set; }
}
```

## Enums

### DiagramDirection

```csharp
public enum DiagramDirection
{
    TopToBottom,  // TD / TB
    BottomToTop,  // BT
    LeftToRight,  // LR
    RightToLeft   // RL
}
```

### NodeShape

```csharp
public enum NodeShape
{
    Rectangle,    // [text]
    RoundedRect,  // (text)
    Stadium,      // ([text])
    Circle,       // ((text))
    Diamond,      // {text}
    Hexagon       // {{text}}
}
```

### EdgeStyle

```csharp
public enum EdgeStyle
{
    Solid,   // --> or ---
    Dotted,  // -.-> or -.-
    Thick    // ==> or ===
}
```

### ArrowType

```csharp
public enum ArrowType
{
    Arrow,  // --> -.-> ==>
    Open    // --- -.- ===
}
```

## ILayoutEngine

```csharp
namespace Shiny.Maui.MermaidDiagrams.Layout;

public interface ILayoutEngine
{
    LayoutResult Layout(DiagramModel model, DiagramLayoutOptions options);
}
```

## SugiyamaLayoutEngine

Default implementation of `ILayoutEngine` using the Sugiyama layered graph algorithm.

```csharp
namespace Shiny.Maui.MermaidDiagrams.Layout;

public sealed class SugiyamaLayoutEngine : ILayoutEngine
{
    public LayoutResult Layout(DiagramModel model, DiagramLayoutOptions options);
}
```

### Algorithm Phases

1. **Cycle Removal** — DFS-based back-edge reversal
2. **Layer Assignment** — Longest-path with dummy nodes for multi-layer edges
3. **Crossing Minimization** — Barycenter heuristic (4 iterations)
4. **Node Positioning** — Coordinate assignment per direction
5. **Edge Routing** — Polyline routing with shape-aware connection points

## DiagramLayoutOptions

```csharp
namespace Shiny.Maui.MermaidDiagrams.Layout;

public sealed class DiagramLayoutOptions
{
    public float NodeWidth { get; init; } = 150;
    public float NodeHeight { get; init; } = 50;
    public float HorizontalSpacing { get; init; } = 60;
    public float VerticalSpacing { get; init; } = 80;
    public float SubgraphPadding { get; init; } = 30;
    public float FontSize { get; init; } = 14;
    public float Margin { get; init; } = 40;
}
```

## LayoutResult

```csharp
namespace Shiny.Maui.MermaidDiagrams.Layout;

public sealed class LayoutResult
{
    public List<NodeLayout> NodeLayouts { get; init; }
    public List<EdgeRoute> EdgeRoutes { get; init; }
    public List<SubgraphLayout> SubgraphLayouts { get; init; }
    public RectF TotalBounds { get; init; }
}

public sealed class NodeLayout
{
    public required string NodeId { get; init; }
    public required RectF Bounds { get; init; }
    public required NodeShape Shape { get; init; }
    public required string Label { get; init; }
}

public sealed class EdgeRoute
{
    public required string SourceId { get; init; }
    public required string TargetId { get; init; }
    public required List<PointF> Points { get; init; }
    public string? Label { get; init; }
    public required EdgeStyle Style { get; init; }
    public required ArrowType ArrowType { get; init; }
}

public sealed class SubgraphLayout
{
    public required string SubgraphId { get; init; }
    public required string Title { get; init; }
    public required RectF Bounds { get; init; }
}
```

## DiagramTheme (Abstract Base)

```csharp
namespace Shiny.Maui.MermaidDiagrams.Theming;

public abstract class DiagramTheme
{
    public abstract Color BackgroundColor { get; }
    public abstract NodeRenderStyle DefaultNodeStyle { get; }
    public abstract EdgeRenderStyle DefaultEdgeStyle { get; }
    public abstract SubgraphRenderStyle DefaultSubgraphStyle { get; }
    public abstract Color TextColor { get; }
    public abstract float FontSize { get; }

    // Override to provide different styles per node index
    public virtual NodeRenderStyle GetNodeStyle(int index) => DefaultNodeStyle;
}
```

## Theme Style Classes

### NodeRenderStyle

```csharp
public sealed class NodeRenderStyle
{
    public required Color FillColor { get; init; }
    public required Color StrokeColor { get; init; }
    public required Color TextColor { get; init; }
    public float StrokeWidth { get; init; } = 2;
    public float CornerRadius { get; init; } = 4;
}
```

### EdgeRenderStyle

```csharp
public sealed class EdgeRenderStyle
{
    public required Color StrokeColor { get; init; }
    public float StrokeWidth { get; init; } = 2;
    public required Color LabelBackgroundColor { get; init; }
    public required Color LabelTextColor { get; init; }
}
```

### SubgraphRenderStyle

```csharp
public sealed class SubgraphRenderStyle
{
    public required Color FillColor { get; init; }
    public required Color StrokeColor { get; init; }
    public required Color TitleColor { get; init; }
    public float StrokeWidth { get; init; } = 1.5f;
    public float CornerRadius { get; init; } = 8;
}
```

## Built-in Themes

| Class | Background | Node Fill | Stroke | Description |
|-------|-----------|-----------|--------|-------------|
| `DefaultTheme` | White | `#ECECFF` | `#9370DB` | Blue/purple — Mermaid.js default |
| `DarkTheme` | `#1A1A2E` | `#16213E` | `#E94560` | Dark navy with red accents |
| `ForestTheme` | `#F0FFF0` | `#C8E6C9` | `#2E7D32` | Natural green tones |
| `NeutralTheme` | `#FAFAFA` | `#F5F5F5` | `#9E9E9E` | Light gray, minimal |

## Troubleshooting

### Diagram not rendering
- Verify `DiagramText` is valid Mermaid flowchart syntax
- Check `ParseError` property for error details
- Ensure the NuGet package `Shiny.Maui.MermaidDiagrams` is installed

### Text must start with "graph" or "flowchart"
- Mermaid text must begin with `graph` or `flowchart` keyword
- Example: `graph TD; A-->B;`

### Nodes overlapping
- Increase `HorizontalSpacing` or `VerticalSpacing` in `DiagramLayoutOptions`
- Increase `NodeWidth` for nodes with long labels

### Subgraph not visible
- Ensure `subgraph ... end` syntax is correct
- Nodes inside a subgraph must be declared between `subgraph` and `end`

### Zoom/pan not working
- Gestures are built into `MermaidDiagramControl` automatically
- Ensure the control has sufficient size (not zero-width/height)
