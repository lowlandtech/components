namespace LowlandTech.Components;

/// <summary>
/// Holds the ordered list of <see cref="IPipelineStage"/> instances
/// that a <see cref="SessionBlock"/> will execute on mount.
/// </summary>
public sealed class PipelineRegistry
{
    private readonly List<IPipelineStage> _stages = [];

    /// <summary>Register a stage. Stages execute in registration order.</summary>
    public void Register(IPipelineStage stage)
    {
        ArgumentNullException.ThrowIfNull(stage);
        _stages.Add(stage);
    }

    /// <summary>Registered stages in execution order.</summary>
    public IReadOnlyList<IPipelineStage> Stages => _stages.AsReadOnly();
}
