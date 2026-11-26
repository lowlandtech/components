namespace LowlandTech.Components.Core.Transitions;

/// <summary>What an action did.</summary>
public enum TransitionOutcome
{
    /// <summary>The action ran and produced a new document state.</summary>
    Applied,

    /// <summary>The runtime refused the action; the document is unchanged.</summary>
    Refused,

    /// <summary>The action was not valid for the selected context, so nothing was run.</summary>
    Invalid,

    /// <summary>The document had moved since it was read; the carried document is the current one.</summary>
    Conflict,
}

/// <summary>How much attention a result case asks for.</summary>
public enum ResultSeverity
{
    /// <summary>The case records something that happened and needs no attention.</summary>
    Success,

    /// <summary>The case is informational.</summary>
    Info,

    /// <summary>The case warns about a consequence a person may want to weigh.</summary>
    Warning,

    /// <summary>The case is why the action did not apply.</summary>
    Error,
}

/// <summary>An action a result offers, identified by key so the host decides what it means.</summary>
/// <param name="Key">Stable key the host maps to a handler.</param>
/// <param name="LabelKey">Text key the host resolves to the visible label.</param>
/// <param name="Primary">Whether the action is the main one for the case.</param>
/// <param name="Disabled">Whether the action is offered but unavailable.</param>
public sealed record ResultAction(
    string Key,
    string LabelKey,
    bool Primary = false,
    bool Disabled = false);

/// <summary>
/// One inspectable case of an action's result — a success, a refusal, a problem or a
/// conflict — carried as data, never as markup.
/// </summary>
/// <param name="Code">Stable result code, the identity shared by the runtime, the client data and the test.</param>
/// <param name="Severity">How much attention the case asks for.</param>
/// <param name="TitleKey">Text key for the case title, resolved by the host.</param>
/// <param name="MessageKey">Text key for the case message, resolved by the host and formatted with <paramref name="Arguments"/>.</param>
/// <param name="Arguments">Values substituted into the message's <c>{name}</c> placeholders.</param>
/// <param name="Fields">Field-level messages: field name to the text key shown against that field.</param>
/// <param name="Observed">Evidence the runtime observed for this case; diagnostics, not user text.</param>
/// <param name="Actions">Actions this case offers, resolved and handled by the host.</param>
public sealed record ResultCase(
    string Code,
    ResultSeverity Severity,
    string? TitleKey = null,
    string? MessageKey = null,
    IReadOnlyDictionary<string, string?>? Arguments = null,
    IReadOnlyDictionary<string, string?>? Fields = null,
    IReadOnlyDictionary<string, string?>? Observed = null,
    IReadOnlyList<ResultAction>? Actions = null);

/// <summary>
/// The result of a document action: what happened, the cases that explain it, and — when the
/// runtime produced one — the authoritative document and its causal head.
/// </summary>
/// <param name="Outcome">What the action did.</param>
/// <param name="Cases">The inspectable cases; may be empty for a plain success.</param>
/// <param name="Document">The resulting document payload, kept opaque so the library never models a document.</param>
/// <param name="Head">The causal head the document was produced from, when the runtime reports one.</param>
/// <remarks>
/// A result that carries a document does so for every outcome, including a refusal or a conflict,
/// so the client can show the truth without a second read. A validation result carries cases and
/// observed evidence only: validation never writes and never produces a document.
/// </remarks>
public sealed record TransitionResult(
    TransitionOutcome Outcome,
    IReadOnlyList<ResultCase> Cases,
    string? Document = null,
    string? Head = null)
{
    /// <summary>Whether the result carries a document for the host to render.</summary>
    public bool HasDocument => !string.IsNullOrEmpty(Document);
}
