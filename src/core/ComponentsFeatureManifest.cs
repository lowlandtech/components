namespace LowlandTech.Components.Core;

/// <summary>
/// The Components view of an installed feature manifest.
///
/// <para>
/// The manifest is package metadata: it names a feature, its target, its
/// logical dependencies and the symbolic identities it contributes. It never
/// carries a type; the trusted startup map resolves an identity to a type.
/// </para>
/// </summary>
/// <param name="Key">The stable logical plugin key of the feature.</param>
/// <param name="SchemaVersion">The version of the manifest schema.</param>
/// <param name="Target">The target the feature activates for, such as blazor.</param>
/// <param name="RequiredCapabilities">The capabilities the host must provide.</param>
/// <param name="LogicalDependencies">The logical keys the feature depends on.</param>
/// <param name="ComponentIdentities">The symbolic identities the feature contributes.</param>
public sealed record ComponentsFeatureManifest(
    string Key,
    int SchemaVersion,
    string Target,
    IReadOnlyList<string> RequiredCapabilities,
    IReadOnlyList<string> LogicalDependencies,
    IReadOnlyList<string> ComponentIdentities);
