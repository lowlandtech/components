using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.GraphCanvas;

/// <summary>
/// SVG canvas container for graph visualization. Provides a dot-grid background,
/// arrowhead marker definitions, and a cascading canvas ID for child edges.
/// </summary>
[Properties(Group = "Graph", Icon = "network",
    Description = "SVG canvas container for graph visualizations")]
public partial class GraphCanvas
{
    [Parameter, PropertyGrid(Group = "Layout", Description = "Canvas width in pixels")]
    public int Width { get; set; } = 640;

    [Parameter, PropertyGrid(Group = "Layout", Description = "Canvas height in pixels")]
    public int Height { get; set; } = 280;

    [Parameter, PropertyGrid(Group = "Appearance", Description = "Show dot-grid background")]
    public bool ShowGrid { get; set; } = true;

    private string GetClass()
    {
        const string baseClass = "cn-graph-canvas";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
