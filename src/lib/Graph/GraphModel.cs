namespace LowlandTech.Components;

/// <summary>
/// Base class for all mutable graph objects (nodes, ports, edges).
/// Fires <see cref="Changed"/> whenever the model mutates so that
/// <see cref="GraphEditor.GraphEditor"/> can call <c>StateHasChanged()</c>.
/// </summary>
public abstract class GraphModel
{
    protected GraphModel() => Id = Guid.NewGuid().ToString("n")[..8];
    protected GraphModel(string id) => Id = id;

    public string Id { get; }

    /// <summary>When true the model ignores drag events.</summary>
    public bool Locked { get; set; }

    public event Action<GraphModel>? Changed;
    public virtual void Refresh() => Changed?.Invoke(this);
}
