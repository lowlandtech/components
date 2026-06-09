namespace LowlandTech.Components;

/// <summary>
/// A single stage in a <see cref="SessionBlock"/> runtime pipeline.
/// Stages are executed sequentially during component initialization
/// and disposed when the Blazor circuit drops.
/// </summary>
public interface IPipelineStage : IAsyncDisposable
{
    /// <summary>Unique name of this stage.</summary>
    string Name { get; }

    /// <summary>
    /// Execute the stage against the shared <see cref="PipelineContext"/>.
    /// </summary>
    Task ExecuteAsync(PipelineContext context, CancellationToken ct = default);
}
