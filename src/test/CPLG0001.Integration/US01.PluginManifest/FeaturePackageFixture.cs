using System.IO.Compression;
using System.Text.Json;
using LowlandTech.Components.Features.Session;

namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// The locally packed packages, the published manifest, the host profiles and
/// the activation outcomes this scope is accepted against.
///
/// <para>
/// The packages are packed from the current source by
/// <see cref="FeaturePackageScenario"/>, so the inspection always runs against
/// artifacts that carry the manifest this repository publishes today.
/// </para>
/// <para>
/// The manifest is mapped onto the Components activation descriptor the way a
/// host adapter maps an installed manifest. The adapter itself is owned by the
/// adapter scopes; this fixture only proves that the published manifest
/// activates against the trusted startup map of a host that provides the
/// capability the manifest requires.
/// </para>
/// <para>
/// The published manifest is never rewritten to prove the backend host
/// refusal: the same unchanged artifact is validated against a backend only
/// host and against a Blazor Server host, and only the backend only host
/// refuses it.
/// </para>
/// </summary>
public sealed class FeaturePackageFixture : IDisposable
{
    /// <summary>Package id of the opt-in feature package.</summary>
    public const string FeaturePackageId = "LowlandTech.Components.Features.Session";

    /// <summary>The manifest path inside a plugin package.</summary>
    public const string ManifestPath = "lowlandtech/plugin.json";

    private static readonly string[] BasePackageIds =
    [
        "LowlandTech.Components.Core",
        "LowlandTech.Components",
        "LowlandTech.Components.Blocks",
    ];

    public FeaturePackageFixture()
    {
        Scenario = FeaturePackageScenario.Run(FeaturePackageScenario.FindRepositoryRoot());

        FeaturePackagePath = Scenario.Packages[FeaturePackageId];

        var basePackages = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var packageId in BasePackageIds)
        {
            basePackages[packageId] = Scenario.Packages[packageId];
        }

        BasePackagePaths = basePackages;
        InspectedPackagePaths = [FeaturePackagePath, .. basePackages.Values];
        FeatureManifestJson = ReadManifest(FeaturePackagePath);

        Hosts = new HostWorkspaceFixture();

        // The capabilities of both hosts are the ones the shared Plugins
        // capability contract derives from the literal project evidence of the
        // host, never a capability this fixture declares itself.
        BackendValidation = ValidateAsync(
            HostWorkspaceFixture.BackendProfile,
            HostWorkspaceFixture.BackendProjectPath);

        BlazorValidation = ValidateAsync(
            HostWorkspaceFixture.BlazorProfile,
            HostWorkspaceFixture.BlazorProjectPath);

        BackendHostCapabilities = BackendValidation.Target.Profile?.Capabilities ?? [];
        BlazorHostCapabilities = BlazorValidation.Target.Profile?.Capabilities ?? [];

        var registrations = new[]
        {
            ComponentsCoreStartup.Registration,
            ComponentsControlsStartup.Registration,
            ComponentsBlocksStartup.Registration,
            ComponentsSessionFeature.Registration,
        };

        StartupMap = ComponentsStartupMap.From(registrations, BlazorHostCapabilities);
        BackendStartupMap = ComponentsStartupMap.From(registrations, BackendHostCapabilities);

        Manifest = ReadManifestDescriptor(FeatureManifestJson);

        var activator = new ComponentsFeatureActivator(StartupMap);
        var backendActivator = new ComponentsFeatureActivator(BackendStartupMap);
        var deployedFeatureKeys = DeployedFeatureKeys;

        var hostStateBefore = ReadHostState(
            StartupMap, BackendStartupMap, deployedFeatureKeys, FeatureManifestJson);

        var backendProjectBefore = Hosts.ReadProjectBytes(HostWorkspaceFixture.BackendProjectPath);

        ActiveOutcome = activator.Activate(Manifest, deployedFeatureKeys);

        // The unchanged published manifest in a backend only host.
        BackendHostOutcome = backendActivator.Activate(Manifest, deployedFeatureKeys);

        InactiveOutcome = activator.Activate(Manifest, []);
        UndeclaredOutcome = activator.Activate(
            ReadManifestDescriptor(WithContributedComponents(FeatureManifestJson, [])),
            deployedFeatureKeys);

        // A manifest published for another target family. This premise is about
        // the manifest, not about the host that carries it.
        UnsupportedTargetOutcome = activator.Activate(
            ReadManifestDescriptor(WithTarget(FeatureManifestJson, "desktop")),
            deployedFeatureKeys);

        MissingDependencyOutcome = activator.Activate(
            ReadManifestDescriptor(WithLogicalDependency(FeatureManifestJson, "lowlandtech.components.absent")),
            deployedFeatureKeys);

        var hostStateAfter = ReadHostState(
            StartupMap, BackendStartupMap, deployedFeatureKeys, FeatureManifestJson);

        var backendProjectAfter = Hosts.ReadProjectBytes(HostWorkspaceFixture.BackendProjectPath);

        ActivationLeftHostUnchanged = hostStateBefore == hostStateAfter
            && backendProjectBefore.SequenceEqual(backendProjectAfter);
    }

    /// <summary>The packaging scenario that produced the inspected packages.</summary>
    public FeaturePackageScenario Scenario { get; }

    /// <summary>The packed opt-in feature package.</summary>
    public string FeaturePackagePath { get; }

    /// <summary>The packed base packages, by package id.</summary>
    public IReadOnlyDictionary<string, string> BasePackagePaths { get; }

    /// <summary>Every packed package the inspector inspects, feature package first.</summary>
    public IReadOnlyList<string> InspectedPackagePaths { get; }

    /// <summary>The published manifest of the feature package.</summary>
    public string? FeatureManifestJson { get; }

    /// <summary>The published manifest as the Components activation descriptor.</summary>
    public ComponentsFeatureManifest Manifest { get; }

    /// <summary>The real Git workspace that hosts the two capability profiles.</summary>
    public HostWorkspaceFixture Hosts { get; }

    /// <summary>
    /// The validation of the packed package set against the backend only host,
    /// as the shared Plugins capability contract reports it.
    /// </summary>
    public WorkspaceProfilePackageValidation BackendValidation { get; }

    /// <summary>
    /// The validation of the packed package set against the Blazor Server host,
    /// as the shared Plugins capability contract reports it.
    /// </summary>
    public WorkspaceProfilePackageValidation BlazorValidation { get; }

    /// <summary>The capabilities the backend only host provides.</summary>
    public IReadOnlyList<string> BackendHostCapabilities { get; }

    /// <summary>The capabilities the Blazor Server host provides.</summary>
    public IReadOnlyList<string> BlazorHostCapabilities { get; }

    /// <summary>
    /// The trusted startup map of the Blazor Server host.
    /// </summary>
    public ComponentsStartupMap StartupMap { get; }

    /// <summary>
    /// The trusted startup map of the backend only host.
    /// </summary>
    public ComponentsStartupMap BackendStartupMap { get; }

    /// <summary>The feature keys a deployed host carries.</summary>
    public IReadOnlyList<string> DeployedFeatureKeys { get; } = [ComponentsSessionFeature.LogicalKey];

    /// <summary>Whether the activations changed the project or the deployment snapshot.</summary>
    public bool ActivationLeftHostUnchanged { get; }

    /// <summary>The activation of the published manifest in the deployed Blazor host.</summary>
    public ComponentsFeatureActivationResult ActiveOutcome { get; }

    /// <summary>
    /// The activation of the unchanged published manifest in the backend only
    /// host.
    /// </summary>
    public ComponentsFeatureActivationResult BackendHostOutcome { get; }

    /// <summary>The activation of the published manifest in a host that does not carry the feature.</summary>
    public ComponentsFeatureActivationResult InactiveOutcome { get; }

    /// <summary>
    /// The activation of a manifest that declares no component at all, while the
    /// startup map still binds the session host identity.
    /// </summary>
    public ComponentsFeatureActivationResult UndeclaredOutcome { get; }

    /// <summary>The activation of a manifest published for another target family.</summary>
    public ComponentsFeatureActivationResult UnsupportedTargetOutcome { get; }

    /// <summary>The activation of a manifest whose logical dependency is not installed.</summary>
    public ComponentsFeatureActivationResult MissingDependencyOutcome { get; }

    /// <summary>The inspection of the whole package set, as the Plugins inspector reads it.</summary>
    public PluginInspectionResult Inspection => InspectedPackagePaths.InspectPluginPackages();

    /// <summary>The packages of the inspected set that carry a manifest.</summary>
    public IReadOnlyList<PluginPackageDescriptor> ManifestPackages =>
        Inspection.Packages.Where(package => package.IsPlugin).ToList();

    /// <summary>The NuGet dependencies the feature package declares.</summary>
    public IReadOnlyList<string> FeaturePackageDependencies =>
        ReadNuspecDependencies(FeaturePackagePath);

    /// <summary>Removes the temporary scenario directory and the host workspace.</summary>
    public void Dispose()
    {
        Hosts.Dispose();
        Scenario.Dispose();
    }

    /// <summary>Reads the plugin manifest out of a packed package.</summary>
    public static string? ReadManifest(string packagePath)
    {
        if (!File.Exists(packagePath))
        {
            return null;
        }

        using var archive = ZipFile.OpenRead(packagePath);
        var entries = archive.Entries
            .Where(entry => string.Equals(
                entry.FullName.Replace('\\', '/'), ManifestPath, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (entries.Count != 1)
        {
            return null;
        }

        using var stream = entries[0].Open();
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    /// <summary>Reads the NuGet dependencies a packed package declares.</summary>
    public static IReadOnlyList<string> ReadNuspecDependencies(string packagePath)
    {
        if (!File.Exists(packagePath))
        {
            return [];
        }

        using var archive = ZipFile.OpenRead(packagePath);
        var entry = archive.Entries
            .FirstOrDefault(candidate => candidate.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));

        if (entry is null)
        {
            return [];
        }

        using var stream = entry.Open();
        using var reader = new StreamReader(stream);
        var document = XDocument.Parse(reader.ReadToEnd());

        return document
            .Descendants()
            .Where(element => element.Name.LocalName == "dependency")
            .Select(element => element.Attribute("id")?.Value ?? string.Empty)
            .Where(id => id.Length > 0)
            .ToList();
    }

    private WorkspaceProfilePackageValidation ValidateAsync(string profile, string projectPath) =>
        Task.Run(() => Hosts.ValidateAsync(
                profile,
                projectPath,
                InspectedPackagePaths,
                [FeaturePackageId]))
            .GetAwaiter()
            .GetResult();

    private static HostState ReadHostState(
        ComponentsStartupMap startupMap,
        ComponentsStartupMap backendStartupMap,
        IReadOnlyList<string> deployedFeatureKeys,
        string? manifestJson) =>
        new(
            string.Join("|", startupMap.ComponentIdentities.Order(StringComparer.Ordinal)),
            string.Join("|", startupMap.LogicalKeys.Order(StringComparer.Ordinal)),
            string.Join("|", startupMap.HostCapabilities.Order(StringComparer.Ordinal)),
            string.Join("|", backendStartupMap.HostCapabilities.Order(StringComparer.Ordinal)),
            string.Join("|", deployedFeatureKeys),
            manifestJson ?? string.Empty);

    private static ComponentsFeatureManifest ReadManifestDescriptor(string? manifestJson)
    {
        if (manifestJson is null)
        {
            throw new InvalidOperationException(
                $"The feature package carries no '{ManifestPath}'.");
        }

        using var document = JsonDocument.Parse(manifestJson);
        var manifest = document.RootElement;

        return new ComponentsFeatureManifest(
            manifest.GetProperty("key").GetString() ?? string.Empty,
            manifest.GetProperty("schemaVersion").GetInt32(),
            manifest.GetProperty("target").GetString() ?? string.Empty,
            ReadStringArray(manifest, "installationUnit", "requiredCapabilities"),
            ReadDependencyKeys(manifest),
            ReadStringArray(manifest, "contributions", "components"));
    }

    private static IReadOnlyList<string> ReadStringArray(
        JsonElement manifest,
        string container,
        string property)
    {
        if (!manifest.TryGetProperty(container, out var containerElement))
        {
            return [];
        }

        if (containerElement.ValueKind != JsonValueKind.Object
            || !containerElement.TryGetProperty(property, out var propertyElement))
        {
            return [];
        }

        return propertyElement.ValueKind == JsonValueKind.Array
            ? propertyElement.EnumerateArray()
                .Select(element => element.GetString() ?? string.Empty)
                .ToList()
            : [];
    }

    private static IReadOnlyList<string> ReadDependencyKeys(JsonElement manifest)
    {
        if (!manifest.TryGetProperty("dependencies", out var dependencies)
            || dependencies.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return dependencies.EnumerateArray()
            .Select(dependency => dependency.GetProperty("key").GetString() ?? string.Empty)
            .ToList();
    }

    private static string? WithTarget(string? manifestJson, string target)
    {
        if (manifestJson is null)
        {
            return null;
        }

        var manifest = JsonNode.Parse(manifestJson);
        manifest?["target"] = target;

        return manifest?.ToJsonString();
    }

    private static string? WithLogicalDependency(string? manifestJson, string key)
    {
        if (manifestJson is null)
        {
            return null;
        }

        var manifest = JsonNode.Parse(manifestJson);

        if (manifest?["dependencies"] is JsonArray dependencies && dependencies.Count > 0)
        {
            dependencies[0]!["key"] = key;
        }

        return manifest?.ToJsonString();
    }

    private static string? WithContributedComponents(string? manifestJson, string[] identities)
    {
        if (manifestJson is null)
        {
            return null;
        }

        var manifest = JsonNode.Parse(manifestJson);

        if (manifest?["contributions"] is JsonObject contributions)
        {
            contributions["components"] = new JsonArray(
                identities.Select(identity => (JsonNode?)identity).ToArray());
        }

        return manifest?.ToJsonString();
    }

    /// <summary>The state of the host that an activation must not change.</summary>
    private sealed record HostState(
        string ComponentIdentities,
        string LogicalKeys,
        string HostCapabilities,
        string BackendHostCapabilities,
        string DeployedFeatureKeys,
        string ManifestJson);
}
