namespace LowlandTech.Components.Core;

/// <summary>
/// One failure of a feature activation. The subject is the name the failure is
/// reported by: the feature key, the missing logical dependency or the
/// identity that did not resolve.
/// </summary>
/// <param name="Code">The reason the activation failed.</param>
/// <param name="Subject">The name the failure names.</param>
/// <param name="Message">The human readable failure.</param>
public sealed record ComponentsFeatureActivationError(
    ComponentsFeatureActivationErrorCode Code,
    string Subject,
    string Message);
