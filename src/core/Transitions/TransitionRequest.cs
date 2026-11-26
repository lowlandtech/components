namespace LowlandTech.Components.Core.Transitions;

/// <summary>An edge the action asks the runtime to write beside the transition.</summary>
/// <param name="Key">Edge key.</param>
/// <param name="TargetId">The record the edge reaches.</param>
public sealed record TransitionEdgeWrite(string Key, string TargetId);

/// <summary>
/// What a trigger sends for one declared action: the causal head it was read at, the record it
/// belongs to, the transition key and the inputs the action declares.
/// </summary>
/// <param name="Head">Causal head the document was rendered from, always sent so a stale action is refused rather than lost.</param>
/// <param name="SourceId">Addressable identity of the record the action belongs to.</param>
/// <param name="EdgeKey">Transition key the action runs.</param>
/// <param name="TargetType">Target node type, when the transition creates one.</param>
/// <param name="Inputs">Values the action declared, keyed by input key.</param>
/// <param name="Properties">Property writes the action carries, keyed by property key.</param>
/// <param name="Edges">Edge writes the action carries.</param>
public sealed record TransitionRequest(
    string Head,
    string SourceId,
    string EdgeKey,
    string? TargetType = null,
    IReadOnlyDictionary<string, string?>? Inputs = null,
    IReadOnlyDictionary<string, string?>? Properties = null,
    IReadOnlyList<TransitionEdgeWrite>? Edges = null);

/// <summary>
/// The host's way of running a transition. Components ships no transport: the host supplies this
/// contract, so the same trigger works against a fake in a unit test and against the deployed
/// runtime in an app.
/// </summary>
public interface ITransitionClient
{
    /// <summary>Runs one action and answers with its outcome, cases and resulting document.</summary>
    Task<TransitionResult> ApplyAsync(TransitionRequest request, CancellationToken cancellationToken = default);
}
