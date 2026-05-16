namespace LowlandTech.Components;

/// <summary>
/// A movable, titled node in a <see cref="GraphDiagram"/>.
/// Call <see cref="SetPosition"/> to move the node — it automatically
/// resyncs all child <see cref="GraphPortModel"/> positions and fires
/// <see cref="GraphModel.Changed"/> so the canvas re-renders.
/// </summary>
public class GraphNodeModel : GraphModel
{
    private readonly List<GraphPortModel> _ports = new();
    private double _x, _y;

    public GraphNodeModel(double x = 0, double y = 0)
    {
        _x = x;
        _y = y;
    }

    public double X => _x;
    public double Y => _y;
    public double Width  { get; set; } = 120;
    public double Height { get; set; } = 44;

    public string  Title    { get; set; } = "";
    public string? Subtitle { get; set; }

    /// <summary>Visual colour variant: default | primary | success | warning | danger.</summary>
    public string Variant  { get; set; } = "default";
    public bool   Selected { get; set; }

    public IReadOnlyList<GraphPortModel> Ports => _ports;

    /// <summary>
    /// Move the node to (x, y).  All attached ports are repositioned and
    /// their connected edges are refreshed so the canvas re-renders.
    /// </summary>
    public void SetPosition(double x, double y)
    {
        _x = x;
        _y = y;
        foreach (var p in _ports)
        {
            p.SyncPosition();
            p.RefreshEdges();
        }
        Refresh();
    }

    /// <summary>Add a port on the given side and return it.</summary>
    public GraphPortModel AddPort(GraphPortAlignment alignment = GraphPortAlignment.Right)
    {
        var port = new GraphPortModel(this, alignment);
        _ports.Add(port);
        port.SyncPosition();
        return port;
    }
}
