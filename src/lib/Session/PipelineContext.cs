namespace LowlandTech.Components;

/// <summary>
/// Shared mutable context passed through every <see cref="IPipelineStage"/>
/// in a <see cref="SessionApp"/> pipeline.
/// </summary>
public sealed class PipelineContext
{
    /// <summary>
    /// Arbitrary key/value state shared across stages.
    /// Hydrated from the graph layer on mount when <see cref="IGraphSession"/>
    /// is available and a <see cref="SessionApp.SessionId"/> is provided.
    /// </summary>
    public Dictionary<string, object?> State { get; } = new(StringComparer.Ordinal);
}
