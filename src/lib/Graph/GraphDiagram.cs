namespace LowlandTech.Components;

/// <summary>
/// Holds the complete mutable state of a graph: its nodes and edges.
/// Fires <see cref="Changed"/> whenever any model inside it mutates,
/// which the <see cref="GraphEditor.GraphEditor"/> component subscribes
/// to in order to call <c>StateHasChanged()</c>.
///
/// Usage:
/// <code>
/// var d = new GraphDiagram();
/// var a = d.AddNode(new GraphNodeModel(40,  88) { Title = "A", Variant = "primary" });
/// var b = d.AddNode(new GraphNodeModel(220, 88) { Title = "B" });
/// d.AddEdge(a.AddPort(GraphPortAlignment.Right), b.AddPort(GraphPortAlignment.Left));
/// // Pass `d` to &lt;GraphEditor Diagram="@d" /&gt;
/// </code>
/// </summary>
public class GraphDiagram
{
    private readonly List<GraphNodeModel> _nodes = new();
    private readonly List<GraphEdgeModel> _edges = new();

    /// <summary>Fires after any structural change or model mutation.</summary>
    public event Action? Changed;

    public IReadOnlyList<GraphNodeModel> Nodes => _nodes;
    public IReadOnlyList<GraphEdgeModel> Edges => _edges;

    // -------------------------------------------------------------------------
    // Mutations
    // -------------------------------------------------------------------------

    public GraphNodeModel AddNode(GraphNodeModel node)
    {
        _nodes.Add(node);
        node.Changed += OnModelChanged;
        Notify();
        return node;
    }

    /// <summary>
    /// Create an edge between <paramref name="source"/> and <paramref name="target"/>
    /// ports and register it in the diagram.
    /// </summary>
    public GraphEdgeModel AddEdge(GraphPortModel source, GraphPortModel target)
    {
        var edge = new GraphEdgeModel(source, target);
        source.AddEdge(edge);
        target.AddEdge(edge);
        _edges.Add(edge);
        edge.Changed += OnModelChanged;
        Notify();
        return edge;
    }

    public void RemoveNode(GraphNodeModel node)
    {
        // Remove all edges touching this node first.
        var nodeEdges = node.Ports.SelectMany(p => p.Edges).Distinct().ToList();
        foreach (var e in nodeEdges)
            RemoveEdge(e);

        node.Changed -= OnModelChanged;
        _nodes.Remove(node);
        Notify();
    }

    public void RemoveEdge(GraphEdgeModel edge)
    {
        edge.Source.RemoveEdge(edge);
        edge.Target.RemoveEdge(edge);
        edge.Changed -= OnModelChanged;
        _edges.Remove(edge);
        Notify();
    }

    // -------------------------------------------------------------------------
    // Private
    // -------------------------------------------------------------------------

    private void OnModelChanged(GraphModel _) => Notify();
    private void Notify() => Changed?.Invoke();
}
