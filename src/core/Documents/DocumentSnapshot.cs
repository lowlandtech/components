namespace LowlandTech.Components.Core.Documents;

/// <summary>The closed set of value kinds a declared document property may have.</summary>
public enum DocumentValueKind
{
    Text,
    Number,
    Decimal,
    Boolean,
    Date,
    DateTime,
    Guid,
    Json,
    Selection,
}

/// <summary>One property a document type declares: its identity, its label and how it is shown.</summary>
/// <param name="Key">Stable key the runtime uses; never shown to a person.</param>
/// <param name="Label">The words a person reads for this property.</param>
/// <param name="ValueKind">The declared kind, which selects the control a writer would get.</param>
/// <param name="Required">Whether the document is incomplete without a value.</param>
/// <param name="ReadOnly">Whether the runtime refuses a change to this property.</param>
/// <param name="Options">The allowed values when <paramref name="ValueKind"/> is a selection.</param>
/// <param name="Order">Display order; ties fall back to declaration order.</param>
public sealed record DocumentPropertyDefinition(
    string Key,
    string Label,
    DocumentValueKind ValueKind = DocumentValueKind.Text,
    bool Required = false,
    bool ReadOnly = false,
    IReadOnlyList<string>? Options = null,
    int Order = 0);

/// <summary>The shape a document declares: its type identity and the properties it carries.</summary>
/// <param name="Type">Machine identity of the document type; not shown as a label.</param>
/// <param name="Properties">The declared properties, in declaration order.</param>
public sealed record DocumentDefinition(
    string Type,
    IReadOnlyList<DocumentPropertyDefinition> Properties);

/// <summary>A relation shown beside a document: one edge and the record it reaches.</summary>
/// <param name="Key">Edge key, kept for the trigger; shown as an attribute, not as text.</param>
/// <param name="Label">The words a person reads for the relation.</param>
/// <param name="Counterpart">The record at the other end, already labelled by the host.</param>
/// <param name="Direction">Which way the relation points, in the host's words.</param>
public sealed record DocumentRelation(
    string Key,
    string Label,
    string Counterpart,
    string Direction = "");

/// <summary>An input an action declares, which the trigger renders when the action is chosen.</summary>
/// <param name="Key">Stable input key.</param>
/// <param name="Label">The words a person reads for the input.</param>
/// <param name="ValueKind">The declared kind, mapped to a control by the trigger.</param>
/// <param name="Required">Whether the action may run without a value.</param>
/// <param name="Options">The allowed values when the kind is a selection.</param>
public sealed record DocumentInput(
    string Key,
    string Label,
    DocumentValueKind ValueKind = DocumentValueKind.Text,
    bool Required = false,
    IReadOnlyList<string>? Options = null);

/// <summary>An action a document offers: the transition it runs and the inputs it needs.</summary>
/// <param name="Key">Stable transition key the host maps to its runner.</param>
/// <param name="Label">The words a person reads on the action.</param>
/// <param name="Primary">Whether this is the main action of the document.</param>
/// <param name="Inputs">The inputs the transition declares; the trigger renders them.</param>
/// <param name="TargetType">Target node type, when the transition creates one.</param>
public sealed record DocumentAction(
    string Key,
    string Label,
    bool Primary = false,
    IReadOnlyList<DocumentInput>? Inputs = null,
    string? TargetType = null);

/// <summary>
/// One document as the host presents it: what it is, its declared shape, its values, its
/// relations, its causal head and the actions it offers. The library renders this and never
/// models the document's storage.
/// </summary>
/// <param name="Type">Machine identity of the document type.</param>
/// <param name="Id">The addressable identity the runtime uses for the record.</param>
/// <param name="Key">The person's name for the record.</param>
/// <param name="Head">Causal head the values were read at, surfaced so an action can be refused rather than lost.</param>
/// <param name="Definition">The declared shape.</param>
/// <param name="Values">Current values, keyed by property key.</param>
/// <param name="Relations">Relations to show, already labelled by the host.</param>
/// <param name="Actions">Actions the document offers.</param>
public sealed record DocumentSnapshot(
    string Type,
    string Id,
    string Key,
    string Head,
    DocumentDefinition Definition,
    IReadOnlyDictionary<string, string?> Values,
    IReadOnlyList<DocumentRelation>? Relations = null,
    IReadOnlyList<DocumentAction>? Actions = null)
{
    /// <summary>The declared relations, never null.</summary>
    public IReadOnlyList<DocumentRelation> RelationsOrEmpty => Relations ?? [];

    /// <summary>The offered actions, never null.</summary>
    public IReadOnlyList<DocumentAction> ActionsOrEmpty => Actions ?? [];
}
