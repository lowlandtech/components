using System.Text;

namespace LowlandTech.Components.GraphEdge;

/// <summary>
/// A directed or undirected edge connecting two points in a graph.
/// Optional <see cref="Waypoints"/> define intermediate anchor points that route
/// the path around obstacles; segments between consecutive points follow
/// the selected <see cref="Routing"/> style.
/// Must be placed inside a <see cref="GraphCanvas.GraphCanvas"/> component so it
/// can reference the arrowhead marker via the cascading canvas ID.
/// </summary>
[Properties(Group = "Graph", Icon = "arrow-right",
    Description = "A path connecting two points in a graph visualization")]
public partial class GraphEdge
{
    [Parameter, PropertyGrid(Group = "Position", Description = "Start point X")]
    public int X1 { get; set; }

    [Parameter, PropertyGrid(Group = "Position", Description = "Start point Y")]
    public int Y1 { get; set; }

    [Parameter, PropertyGrid(Group = "Position", Description = "End point X")]
    public int X2 { get; set; }

    [Parameter, PropertyGrid(Group = "Position", Description = "End point Y")]
    public int Y2 { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Description = "Show arrowhead at the end of the path")]
    public bool Directed { get; set; } = true;

    [Parameter, PropertyGrid(Group = "Content", Description = "Optional label drawn near the midpoint")]
    public string? Label { get; set; }

    /// <summary>
    /// Intermediate anchor points that the path must pass through, in order.
    /// Each waypoint is an <see cref="GraphPoint"/> (X, Y).
    /// When provided the edge routes through every waypoint using the chosen
    /// <see cref="Routing"/> style for each individual segment.
    /// </summary>
    [Parameter]
    public IReadOnlyList<GraphPoint>? Waypoints { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "bezier", "straight", "step" })]
    public string Routing { get; set; } = "bezier";

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "dashed", "highlighted" })]
    public string Variant { get; set; } = "default";

    // -------------------------------------------------------------------------
    // Path building — shared with the razor template
    // -------------------------------------------------------------------------

    internal string GetPath()
    {
        var pts = GetAllPoints();
        var sb = new StringBuilder();
        sb.Append($"M {pts[0].X} {pts[0].Y}");

        for (var i = 1; i < pts.Count; i++)
        {
            var p = pts[i - 1];
            var c = pts[i];
            switch (Routing)
            {
                case "straight":
                    sb.Append($" L {c.X} {c.Y}");
                    break;
                case "step":
                    var midX = (p.X + c.X) / 2;
                    sb.Append($" H {midX} V {c.Y} H {c.X}");
                    break;
                default: // bezier
                    var dx = (int)(Math.Abs(c.X - p.X) * 0.5);
                    sb.Append($" C {p.X + dx} {p.Y} {c.X - dx} {c.Y} {c.X} {c.Y}");
                    break;
            }
        }

        return sb.ToString();
    }

    internal string GetPathClass() =>
        $"cn-graph-edge-path{(Variant != "default" ? $" cn-graph-edge-path-{Variant}" : "")}";

    // Label sits at the midpoint of the straight-line span start→end.
    internal int GetLabelX() => (X1 + X2) / 2;
    internal int GetLabelY() => (Y1 + Y2) / 2 - 8;

    private List<GraphPoint> GetAllPoints()
    {
        var pts = new List<GraphPoint> { new(X1, Y1) };
        if (Waypoints is not null)
            pts.AddRange(Waypoints);
        pts.Add(new(X2, Y2));
        return pts;
    }
}
