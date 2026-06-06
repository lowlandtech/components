namespace LowlandTech.Core.Graph;

/// <summary>
/// One entry in a block's intent surface. Stored in state; recomputed by the reducer
/// after every action. <see cref="Key"/> is the dispatchable action type string the
/// agent or UI reads to decide what to dispatch next.
/// </summary>
public sealed record BlockIntent(
    string Key,
    BlockIntentClass Class,
    string Label,
    IReadOnlyDictionary<string, object?>? Payload = null);

/// <summary>
/// Back-stack frame pushed on every transition intent dispatch.
/// Popped by Cancel / Back to restore the prior view.
/// </summary>
public sealed record BlockFrame(
    string View,
    IReadOnlyDictionary<string, object?> Payload);

/// <summary>Structural class of a block intent.</summary>
public enum BlockIntentClass
{
    /// <summary>Always present; does not alter back-stack or sibling intents.</summary>
    Ambient,

    /// <summary>Narrows the sibling surface without pushing a new frame.</summary>
    Modifier,

    /// <summary>Pushes a new frame onto the back-stack.</summary>
    Transition
}
