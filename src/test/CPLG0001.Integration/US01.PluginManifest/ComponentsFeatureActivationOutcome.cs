namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// The outcome of one Components feature activation, read back from the
/// production activation contract.
/// </summary>
/// <param name="IsActivated">Whether the feature activated.</param>
/// <param name="ResolvedComponents">The components resolved through the trusted startup map.</param>
/// <param name="Errors">The failures the activation reported.</param>
public sealed record ComponentsFeatureActivationOutcome(
    bool IsActivated,
    IReadOnlyList<Type> ResolvedComponents,
    IReadOnlyList<ComponentsFeatureActivationErrorView> Errors);
