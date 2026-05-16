namespace LowlandTech.Components;

/// <summary>
/// A directed or undirected connection between two <see cref="GraphPortModel"/> instances.
/// The edge's actual coordinates are always read from <see cref="Source"/>
/// and <see cref="Target"/> — no coordinates are stored on the edge itself,
/// so moving a node automatically moves all of its edges.
/// </summary>
public class GraphEdgeModel : GraphModel
{
    public GraphEdgeModel(GraphPortModel source, GraphPortModel target)
    {
        Source = source;
        Target = target;
    }

    public GraphPortModel Source { get; }
    public GraphPortModel Target { get; }

    public bool    Directed { get; set; } = true;
    public string? Label    { get; set; }

    /// <summary>Path style: bezier | straight | step.</summary>
    public string Routing { get; set; } = "bezier";

    /// <summary>Visual variant: default | dashed | highlighted.</summary>
    public string Variant { get; set; } = "default";
}
