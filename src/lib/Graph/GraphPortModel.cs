namespace LowlandTech.Components;

/// <summary>
/// A connection point on a <see cref="GraphNodeModel"/>.
/// Its absolute SVG position is computed from the parent node's
/// position + dimensions + <see cref="Alignment"/> and is updated
/// automatically whenever the parent node moves.
/// </summary>
public class GraphPortModel : GraphModel
{
    private readonly List<GraphEdgeModel> _edges = new(4);

    internal GraphPortModel(GraphNodeModel node, GraphPortAlignment alignment)
    {
        Node      = node;
        Alignment = alignment;
    }

    public GraphNodeModel    Node      { get; }
    public GraphPortAlignment Alignment { get; }

    /// <summary>Absolute X position in SVG canvas coordinates.</summary>
    public double X { get; private set; }

    /// <summary>Absolute Y position in SVG canvas coordinates.</summary>
    public double Y { get; private set; }

    public IReadOnlyList<GraphEdgeModel> Edges => _edges;

    /// <summary>Recompute X/Y from the parent node's current position.</summary>
    internal void SyncPosition()
    {
        X = Alignment switch
        {
            GraphPortAlignment.Left   => Node.X,
            GraphPortAlignment.Right  => Node.X + Node.Width,
            _                         => Node.X + Node.Width  / 2
        };
        Y = Alignment switch
        {
            GraphPortAlignment.Top    => Node.Y,
            GraphPortAlignment.Bottom => Node.Y + Node.Height,
            _                         => Node.Y + Node.Height / 2
        };
    }

    /// <summary>Signal all connected edges to re-render.</summary>
    public void RefreshEdges()
    {
        foreach (var edge in _edges)
            edge.Refresh();
    }

    internal void AddEdge(GraphEdgeModel edge)    => _edges.Add(edge);
    internal void RemoveEdge(GraphEdgeModel edge) => _edges.Remove(edge);
}
