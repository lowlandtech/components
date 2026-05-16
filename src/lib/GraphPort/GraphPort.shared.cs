using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.GraphPort;

/// <summary>
/// A connection port (socket) rendered as a small circle, typically on the
/// perimeter of a <see cref="GraphNode.GraphNode"/>. Coordinates are absolute
/// within the parent SVG canvas.
/// </summary>
[Properties(Group = "Graph", Icon = "circle",
    Description = "A connection port on the perimeter of a graph node")]
public partial class GraphPort
{
    [Parameter, PropertyGrid(Group = "Position", Description = "Port centre X in SVG canvas pixels")]
    public int X { get; set; }

    [Parameter, PropertyGrid(Group = "Position", Description = "Port centre Y in SVG canvas pixels")]
    public int Y { get; set; }

    [Parameter, PropertyGrid(Group = "Layout", Description = "Port circle radius")]
    public int Radius { get; set; } = 5;

    [Parameter, PropertyGrid(Group = "Content", Description = "Optional label next to the port")]
    public string? Label { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "input", "output" })]
    public string Type { get; set; } = "output";

    private string GetClass()
    {
        var typeClass = Type == "input" ? "cn-graph-port-input" : "cn-graph-port-output";
        return string.IsNullOrEmpty(Class) ? typeClass : $"{typeClass} {Class}";
    }

    private int GetLabelX() => Type == "output"
        ? X + Radius + 4
        : X - Radius - 4;
}
