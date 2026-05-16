namespace LowlandTech.Components.GraphNode;

/// <summary>
/// A node in a graph — renders as an SVG rect with a title label.
/// Must be placed inside a <see cref="GraphCanvas.GraphCanvas"/> component.
/// </summary>
[Properties(Group = "Graph", Icon = "square",
    Description = "A labelled node in a graph visualization")]
public partial class GraphNode
{
    [Parameter, PropertyGrid(Group = "Position", Description = "Left offset in SVG canvas pixels")]
    public int X { get; set; }

    [Parameter, PropertyGrid(Group = "Position", Description = "Top offset in SVG canvas pixels")]
    public int Y { get; set; }

    [Parameter, PropertyGrid(Group = "Layout", Description = "Node width in pixels")]
    public int Width { get; set; } = 120;

    [Parameter, PropertyGrid(Group = "Layout", Description = "Node height in pixels")]
    public int Height { get; set; } = 44;

    [Parameter, PropertyGrid(Group = "Content", Description = "Primary label")]
    public string Title { get; set; } = "Node";

    [Parameter, PropertyGrid(Group = "Content", Description = "Optional secondary label shown below the title")]
    public string? Subtitle { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Description = "Highlight node with selected-state ring")]
    public bool Selected { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "primary", "success", "warning", "danger" })]
    public string Variant { get; set; } = "default";

    private string GetRectClass()
    {
        var variant = $"cn-graph-node-rect cn-graph-node-rect-{Variant}";
        return Selected ? $"{variant} cn-graph-node-selected" : variant;
    }
}
