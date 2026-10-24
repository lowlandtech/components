namespace LowlandTech.Components.Core;

/// <summary>
/// The reasons a feature activation can fail.
/// </summary>
public enum ComponentsFeatureActivationErrorCode
{
    /// <summary>The manifest targets a host the feature cannot activate for.</summary>
    UnsupportedTarget,

    /// <summary>A logical dependency of the feature is not registered by the host.</summary>
    MissingLogicalDependency,

    /// <summary>The feature is not part of the deployed host.</summary>
    MissingDeployment,

    /// <summary>A component the feature declares is not bound by the startup map.</summary>
    UnresolvedComponent,

    /// <summary>
    /// The host does not provide a capability the feature requires.
    /// </summary>
    /// <remarks>
    /// The name is the one the shared Plugins capability contract reports the
    /// same refusal with, so a host adapter maps one refusal onto the other
    /// without a second vocabulary.
    /// </remarks>
    TargetCapabilitiesRequired,
}
