namespace LowlandTech.Components;

/// <summary>
/// The candidate a plugin owner contributes to the session block surface,
/// before it is validated and published.
/// </summary>
/// <param name="OwnerKey">The logical key of the owner that contributes the block.</param>
/// <param name="NodeTypeKey">The node type key the block handles.</param>
/// <param name="SymbolicIdentity">
/// The symbolic identity of the component, never a type. The identity resolves
/// through the trusted startup map of the host, so a plugin cannot contribute
/// an arbitrary type.
/// </param>
/// <param name="Priority">
/// The priority of the block. A higher value wins over a host default that
/// handles the same node type key.
/// </param>
/// <param name="Label">The optional label of the block.</param>
/// <param name="Icon">The optional icon of the block.</param>
public sealed record SessionBlockCandidate(
    string OwnerKey,
    string NodeTypeKey,
    string SymbolicIdentity,
    int Priority = 0,
    string? Label = null,
    string? Icon = null);

/// <summary>
/// The host authority over the plugin owners at one revision.
/// </summary>
/// <param name="Revision">The revision of the authority.</param>
/// <param name="ActiveOwnerKeys">The owners that are active in this revision.</param>
public sealed record SessionBlockOwnerSnapshot(long Revision, IReadOnlyCollection<string> ActiveOwnerKeys);

/// <summary>
/// The named reason a candidate was refused.
/// </summary>
public enum SessionBlockCandidateErrorCode
{
    /// <summary>The owner already claimed that node type key.</summary>
    DuplicateNodeTypeKey,

    /// <summary>Another owner already claimed that node type key.</summary>
    ConflictingNodeTypeKey,

    /// <summary>The symbolic identity does not resolve through the trusted startup map.</summary>
    UnresolvedComponent,

    /// <summary>The resolved type is not a renderable component.</summary>
    WrongComponentType,
}

/// <summary>
/// One refusal of a candidate.
/// </summary>
/// <param name="Code">The named reason of the refusal.</param>
/// <param name="Subject">The node type key, the symbolic identity or the owner the refusal names.</param>
/// <param name="Message">The message of the refusal.</param>
public sealed record SessionBlockCandidateError(
    SessionBlockCandidateErrorCode Code,
    string Subject,
    string Message);

/// <summary>
/// The outcome of validating a candidate.
/// </summary>
/// <param name="Candidate">The candidate that was validated.</param>
/// <param name="IsValid">Whether the candidate was accepted.</param>
/// <param name="Errors">The refusals of the candidate.</param>
public sealed record SessionBlockCandidateValidation(
    SessionBlockCandidate Candidate,
    bool IsValid,
    IReadOnlyList<SessionBlockCandidateError> Errors);
