namespace LowlandTech.Components.Core;

/// <summary>
/// The outcome of one feature activation.
/// </summary>
/// <param name="IsActivated">Whether the feature activated.</param>
/// <param name="ResolvedComponents">The components the activation resolved.</param>
/// <param name="Errors">The failures the activation reported.</param>
public sealed record ComponentsFeatureActivationResult(
    bool IsActivated,
    IReadOnlyList<Type> ResolvedComponents,
    IReadOnlyList<ComponentsFeatureActivationError> Errors);
