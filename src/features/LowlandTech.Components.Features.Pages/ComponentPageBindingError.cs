namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// The named reasons a page candidate is refused.
/// </summary>
public enum ComponentPageBindingErrorCode
{
    /// <summary>The owner already published a page for this symbolic identity.</summary>
    DuplicateBinding,

    /// <summary>The host's trusted startup map cannot resolve the symbolic identity.</summary>
    UnresolvedComponent,

    /// <summary>The resolved type is not a renderable Blazor component.</summary>
    WrongComponentType,

    /// <summary>The resolved component carries no page metadata.</summary>
    MissingPageMetadata,

    /// <summary>Another owner already published a page under this route.</summary>
    ConflictingRoutePath,

    /// <summary>The route belongs to the host and is not available to a plugin page.</summary>
    ReservedHostRoute,
}

/// <summary>
/// A single refusal, named by code and subject so a host can report it without
/// parsing a message.
/// </summary>
/// <param name="Code">The named reason the candidate was refused.</param>
/// <param name="Subject">
/// The value the refusal is about: the symbolic identity of a component or the
/// route path of a conflicting or reserved route.
/// </param>
/// <param name="Message">The human readable explanation of the refusal.</param>
public sealed record ComponentPageBindingError(
    ComponentPageBindingErrorCode Code,
    string Subject,
    string Message);
