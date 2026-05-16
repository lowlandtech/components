using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.GraphEditor;

/// <summary>
/// Interactive SVG graph editor.  Pass a <see cref="GraphDiagram"/> built from
/// <see cref="GraphNodeModel"/>, <see cref="GraphPortModel"/>, and
/// <see cref="GraphEdgeModel"/> instances.  Nodes are draggable — edges and
/// ports follow automatically because they derive their coordinates from the
/// node model rather than storing them independently.
///
/// <b>Blazor Server note:</b> every pointer-move fires a SignalR round-trip,
/// which causes visible drag lag on high-latency connections.  For Server-side
/// hosting, wrap the pointer-move handler with a JS interop debounce; for WASM
/// the pure-Blazor approach here is smooth.
/// </summary>
[Properties(Group = "Graph", Icon = "network",
    Description = "Interactive diagram editor with draggable nodes and live-routed edges")]
public partial class GraphEditor
{
    /// <summary>The graph model that drives the editor.</summary>
    [Parameter, EditorRequired]
    public GraphDiagram Diagram { get; set; } = default!;

    [Parameter, PropertyGrid(Group = "Layout", Description = "Canvas width in pixels")]
    public int Width  { get; set; } = 640;

    [Parameter, PropertyGrid(Group = "Layout", Description = "Canvas height in pixels")]
    public int Height { get; set; } = 400;

    [Parameter, PropertyGrid(Group = "Appearance")]
    public bool ShowGrid { get; set; } = true;
}
