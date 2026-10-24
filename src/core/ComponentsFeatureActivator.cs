namespace LowlandTech.Components.Core;

/// <summary>
/// Activates a feature manifest against the trusted startup map of a host.
///
/// <para>
/// The activation is a pure decision: it reads the manifest, the startup map
/// and the deployed feature keys, and it reports what it found. It never
/// changes the host project, the deployment snapshot or the startup map, so a
/// failed activation leaves the host exactly as it was.
/// </para>
/// </summary>
public sealed class ComponentsFeatureActivator
{
    /// <summary>The target a Components feature activates for.</summary>
    public const string BlazorTarget = "blazor";

    private readonly ComponentsStartupMap _startupMap;

    /// <summary>
    /// Creates an activator over the trusted startup map of a host.
    /// </summary>
    /// <param name="startupMap">The trusted startup map of the host.</param>
    public ComponentsFeatureActivator(ComponentsStartupMap startupMap) =>
        _startupMap = startupMap ?? throw new ArgumentNullException(nameof(startupMap));

    /// <summary>
    /// Activates a feature manifest in a deployed host.
    /// </summary>
    /// <param name="manifest">The manifest of the installed feature.</param>
    /// <param name="deployedFeatureKeys">
    /// The logical keys of the features the deployed host carries.
    /// </param>
    public ComponentsFeatureActivationResult Activate(
        ComponentsFeatureManifest manifest,
        IReadOnlyCollection<string> deployedFeatureKeys)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(deployedFeatureKeys);

        var errors = new List<ComponentsFeatureActivationError>();

        if (!string.Equals(manifest.Target, BlazorTarget, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new ComponentsFeatureActivationError(
                ComponentsFeatureActivationErrorCode.UnsupportedTarget,
                manifest.Key,
                $"The feature '{manifest.Key}' targets '{manifest.Target}' and cannot activate in a {BlazorTarget} host."));
        }

        foreach (var dependency in manifest.LogicalDependencies ?? [])
        {
            if (!_startupMap.ContainsLogicalKey(dependency))
            {
                errors.Add(new ComponentsFeatureActivationError(
                    ComponentsFeatureActivationErrorCode.MissingLogicalDependency,
                    dependency,
                    $"The logical dependency '{dependency}' of the feature '{manifest.Key}' is not registered by the host."));
            }
        }

        if (!deployedFeatureKeys.Contains(manifest.Key, StringComparer.Ordinal))
        {
            errors.Add(new ComponentsFeatureActivationError(
                ComponentsFeatureActivationErrorCode.MissingDeployment,
                manifest.Key,
                $"The feature '{manifest.Key}' is not part of the deployed host. Rebuild the host and restart it to activate the feature."));
        }

        if (errors.Count > 0)
        {
            return new ComponentsFeatureActivationResult(false, [], errors);
        }

        var resolved = new List<Type>();

        foreach (var identity in manifest.ComponentIdentities ?? [])
        {
            if (_startupMap.TryResolveComponent(identity, out var componentType))
            {
                resolved.Add(componentType);
                continue;
            }

            errors.Add(new ComponentsFeatureActivationError(
                ComponentsFeatureActivationErrorCode.UnresolvedComponent,
                identity,
                $"The component '{identity}' declared by the feature '{manifest.Key}' is not bound by the trusted startup map."));
        }

        return new ComponentsFeatureActivationResult(errors.Count == 0, resolved, errors);
    }
}
