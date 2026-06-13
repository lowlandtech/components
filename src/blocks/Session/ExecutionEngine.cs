namespace LowlandTech.Components;

/// <summary>
/// Lightweight execution context passed to graph tool services. Real hosts can
/// extend this with the current user, a graph client, or a unit of work. The
/// components environment uses a no-op stub.
/// </summary>
public interface IExecutionEngine
{
    string? UserId { get; }
    IEdgeTypeService EdgeTypes { get; }
}

/// <summary>
/// Components-environment stub for <see cref="IExecutionEngine"/>. Real hosts
/// (shell/app/plugins) can replace this with an implementation that carries the
/// current user, graph client, and unit of work.
/// </summary>
public sealed class ComponentsExecutionEngine : IExecutionEngine
{
    public ComponentsExecutionEngine(IEdgeTypeService edgeTypes)
    {
        EdgeTypes = edgeTypes;
    }

    public string? UserId { get; init; }
    public IEdgeTypeService EdgeTypes { get; }
}
