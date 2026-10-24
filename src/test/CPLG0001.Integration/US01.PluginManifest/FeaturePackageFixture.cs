using System.IO.Compression;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// The locally packed packages and the published manifest this scope is
/// accepted against.
///
/// <para>
/// The shared local feed is where this repository publishes its packages
/// (see <c>docs/local-build.md</c>). The fixture resolves the feed from the
/// repository's own <c>NuGet.Config</c>, so the acceptance never depends on a
/// hard coded path.
/// </para>
/// <para>
/// The fixture never fails while it resolves: a missing package is reported as
/// a missing package, so an unimplemented scope stays a skipped Fact instead of
/// an erroring one.
/// </para>
/// </summary>
public sealed class FeaturePackageFixture
{
    /// <summary>Package id of the opt-in feature package.</summary>
    public const string FeaturePackageId = "LowlandTech.Components.Features.Session";

    /// <summary>The stable logical plugin key of the feature.</summary>
    public const string FeatureKey = "lowlandtech.components.session";

    /// <summary>The logical key the base Blocks library owns and registers.</summary>
    public const string LogicalDependencyKey = "lowlandtech.components.blocks";

    /// <summary>The symbolic identity of the component the feature contributes.</summary>
    public const string SessionHostComponentIdentity =
        "LowlandTech.Components.Blocks.Session.SessionPluginHost";

    /// <summary>The manifest path inside a plugin package.</summary>
    public const string ManifestPath = "lowlandtech/plugin.json";

    private static readonly string[] BasePackageIds =
    [
        "LowlandTech.Components.Core",
        "LowlandTech.Components",
        "LowlandTech.Components.Blocks",
    ];

    /// <summary>The logical keys the host registers for the base libraries.</summary>
    private static readonly string[] HostLogicalKeys =
    [
        "lowlandtech.components.core",
        "lowlandtech.components.controls",
        LogicalDependencyKey,
    ];

    /// <summary>The shared local feed the packages were published to.</summary>
    public string FeedDirectory { get; }

    /// <summary>The packed feature package, when it exists.</summary>
    public string? FeaturePackagePath { get; }

    /// <summary>The packed base packages, by package id.</summary>
    public IReadOnlyDictionary<string, string> BasePackagePaths { get; }

    /// <summary>The base package ids that are not in the feed.</summary>
    public IReadOnlyList<string> MissingBasePackageIds { get; }

    /// <summary>Every packed package the inspector inspects, feature package first.</summary>
    public IReadOnlyList<string> InspectedPackagePaths { get; }

    /// <summary>The published manifest of the feature package, when it exists.</summary>
    public string? FeatureManifestJson { get; }

    public FeaturePackageFixture()
    {
        FeedDirectory = FindFeedDirectory();
        FeaturePackagePath = FindPackage(FeedDirectory, FeaturePackageId);
        FeatureManifestJson = ReadManifest(FeaturePackagePath);

        var basePackages = new Dictionary<string, string>(StringComparer.Ordinal);
        var missing = new List<string>();

        foreach (var packageId in BasePackageIds)
        {
            var path = FindPackage(FeedDirectory, packageId);

            if (path is null)
            {
                missing.Add(packageId);
                continue;
            }

            basePackages[packageId] = path;
        }

        BasePackagePaths = basePackages;
        MissingBasePackageIds = missing;

        var inspected = new List<string>();

        if (FeaturePackagePath is not null)
        {
            inspected.Add(FeaturePackagePath);
        }

        inspected.AddRange(basePackages.Values);
        InspectedPackagePaths = inspected;
    }

    /// <summary>The inspection of the whole package set, as the Plugins inspector reads it.</summary>
    public PluginInspectionResult Inspection =>
        InspectedPackagePaths.InspectPluginPackages();

    /// <summary>The packages of the inspected set that carry a manifest.</summary>
    public IReadOnlyList<PluginPackageDescriptor> ManifestPackages =>
        Inspection.Packages.Where(package => package.IsPlugin).ToList();

    /// <summary>The trusted startup bindings of a host that carries the feature.</summary>
    public IReadOnlyList<(string LogicalKey, string SymbolicIdentity, Type ComponentType)> StartupBindings =>
    [
        (FeatureKey, SessionHostComponentIdentity, typeof(SessionPluginHost)),
    ];

    /// <summary>The feature keys a deployed host carries.</summary>
    public IReadOnlyList<string> DeployedFeatureKeys { get; } = [FeatureKey];

    /// <summary>The activation of the published manifest in a deployed host.</summary>
    public ComponentsFeatureActivationOutcome ActiveOutcome => Activate(
        FeatureManifestJson,
        StartupBindings,
        HostLogicalKeys,
        DeployedFeatureKeys);

    /// <summary>The activation of the published manifest in a host that does not carry the feature.</summary>
    public ComponentsFeatureActivationOutcome InactiveOutcome => Activate(
        FeatureManifestJson,
        StartupBindings,
        HostLogicalKeys,
        []);

    /// <summary>
    /// The activation of a manifest that declares no component at all, while
    /// the startup map still binds the session host identity.
    /// </summary>
    public ComponentsFeatureActivationOutcome UndeclaredOutcome => Activate(
        WithContributedComponents(FeatureManifestJson, []),
        StartupBindings,
        HostLogicalKeys,
        DeployedFeatureKeys);

    /// <summary>The activation of a manifest that targets a backend only host.</summary>
    public ComponentsFeatureActivationOutcome BackendTargetOutcome => Activate(
        WithTarget(FeatureManifestJson, "backend"),
        StartupBindings,
        HostLogicalKeys,
        DeployedFeatureKeys);

    /// <summary>The activation of a manifest whose logical dependency is not installed.</summary>
    public ComponentsFeatureActivationOutcome MissingDependencyOutcome => Activate(
        WithLogicalDependency(FeatureManifestJson, "lowlandtech.components.absent"),
        StartupBindings,
        HostLogicalKeys,
        DeployedFeatureKeys);

    /// <summary>
    /// Resolves the shared local feed from the repository's own NuGet
    /// configuration.
    /// </summary>
    public static string FindFeedDirectory()
    {
        var repositoryRoot = FindRepositoryRoot();
        var configuration = Path.Combine(repositoryRoot, "NuGet.Config");

        if (File.Exists(configuration))
        {
            var document = XDocument.Load(configuration);
            var source = document.Root?
                .Elements("packageSources")
                .Elements("add")
                .FirstOrDefault(element =>
                    string.Equals((string?)element.Attribute("key"), "lowlandtech-local",
                        StringComparison.Ordinal));

            var value = (string?)source?.Attribute("value");

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        var parent = Directory.GetParent(repositoryRoot);

        return parent is null ? repositoryRoot : Path.Combine(parent.FullName, "packages");
    }

    /// <summary>
    /// Locates the repository root by walking up from the test binaries until
    /// the solution file is found.
    /// </summary>
    public static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "components.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"The Components solution was not found above '{AppContext.BaseDirectory}'.");
    }

    /// <summary>
    /// Finds the newest package with the given id in the feed.
    /// </summary>
    public static string? FindPackage(string feedDirectory, string packageId)
    {
        if (!Directory.Exists(feedDirectory))
        {
            return null;
        }

        var candidates = new DirectoryInfo(feedDirectory)
            .GetFiles($"{packageId}.*.nupkg")
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .ToList();

        return candidates.Count == 0 ? null : candidates[0].FullName;
    }

    /// <summary>Reads the plugin manifest out of a packed package.</summary>
    public static string? ReadManifest(string? packagePath)
    {
        if (packagePath is null || !File.Exists(packagePath))
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

    private static ComponentsFeatureActivationOutcome Activate(
        string? manifestJson,
        IReadOnlyList<(string LogicalKey, string SymbolicIdentity, Type ComponentType)> bindings,
        IReadOnlyCollection<string> logicalKeys,
        IReadOnlyCollection<string> deployedFeatureKeys)
    {
        if (manifestJson is null)
        {
            throw new InvalidOperationException(
                $"The feature package '{FeaturePackageId}' carries no '{ManifestPath}'.");
        }

        return ComponentsFeatureContracts.Activate(manifestJson, bindings, logicalKeys, deployedFeatureKeys);
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
}
