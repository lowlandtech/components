namespace LowlandTech.Components;

/// <summary>
/// Describes a session block that can be rendered inside a <see cref="SessionContextBlock"/>.
/// </summary>
public interface ISessionBlockDescriptor
{
    /// <summary>
    /// The node type key this block handles (e.g., "session", "workitem", "session.pin").
    /// </summary>
    string NodeTypeKey { get; }

    /// <summary>
    /// The Razor component type to render for this block.
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Optional human-readable label for the block.
    /// </summary>
    string? Label { get; }

    /// <summary>
    /// Optional icon name for the block.
    /// </summary>
    string? Icon { get; }

    /// <summary>
    /// Priority for ordering when multiple descriptors match. Higher values win.
    /// </summary>
    int Priority => 0;
}

/// <summary>
/// Registry that owns session block descriptors for one feature domain.
/// </summary>
public interface ISessionBlockRegistry
{
    /// <summary>
    /// Registry priority. Higher values are checked first.
    /// </summary>
    int Priority => 0;

    /// <summary>
    /// Returns all descriptors provided by this registry.
    /// </summary>
    IReadOnlyList<ISessionBlockDescriptor> GetDescriptors();
}

/// <summary>
/// Discovery service that aggregates all <see cref="ISessionBlockRegistry"/> instances
/// and resolves component types by node type key.
/// </summary>
public interface ISessionBlockDiscovery
{
    /// <summary>
    /// Resolves the component type for the given node type key.
    /// </summary>
    Type? ResolveComponentType(string? nodeTypeKey);

    /// <summary>
    /// Gets the descriptor for the given node type key.
    /// </summary>
    ISessionBlockDescriptor? GetDescriptor(string? nodeTypeKey);

    /// <summary>
    /// Returns all discovered descriptors.
    /// </summary>
    IReadOnlyList<ISessionBlockDescriptor> GetDescriptors();
}

/// <summary>
/// Default implementation of <see cref="ISessionBlockDescriptor"/>.
/// </summary>
public sealed record SessionBlockDescriptor(
    string NodeTypeKey,
    Type ComponentType,
    string? Label = null,
    string? Icon = null,
    int Priority = 0) : ISessionBlockDescriptor;

/// <summary>
/// Default registry with built-in session block mappings.
/// </summary>
public sealed class DefaultSessionBlockRegistry : ISessionBlockRegistry
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Discovery reads the types of the loaded assemblies, which a trimmed publish cannot name statically; the host that publishes trimmed preserves and registers the block and component types, which the trimmed-host preservation probe checks.")]
    public IReadOnlyList<ISessionBlockDescriptor> GetDescriptors() =>
    [
        new SessionBlockDescriptor("session", typeof(SessionCard), "Session Card", "message-square"),
        new SessionBlockDescriptor("session.pin", typeof(SessionContextPin), "Pinned Context", "pin"),
        new SessionBlockDescriptor("workitem", typeof(SessionCard), "Workitem", "briefcase"),
        new SessionBlockDescriptor("workspace", typeof(SessionCard), "Workspace", "layout"),
        new SessionBlockDescriptor("node", typeof(NodeBlock), "Node", "box"),
    ];
}

/// <summary>
/// Registry that discovers session blocks by scanning all loaded assemblies
/// for ComponentBase types whose name matches a given component block name.
/// This provides backward-compatible auto-discovery for blocks that haven't
/// been explicitly registered.
/// </summary>
public sealed class AssemblyScanningSessionBlockRegistry : ISessionBlockRegistry
{
    public int Priority => -100; // Lowest priority

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Discovery reads the types of the loaded assemblies, which a trimmed publish cannot name statically; the host that publishes trimmed preserves and registers the block and component types, which the trimmed-host preservation probe checks.")]
    public IReadOnlyList<ISessionBlockDescriptor> GetDescriptors()
    {
        var descriptors = new List<ISessionBlockDescriptor>();
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(type => type is not null).Cast<Type>().ToArray();
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (type.IsAbstract || !typeof(ComponentBase).IsAssignableFrom(type))
                    continue;

                var key = type.Name;
                if (!seenKeys.Add(key))
                    continue; // Skip duplicates

                descriptors.Add(new SessionBlockDescriptor(key, type, key, null, Priority: -100));
            }
        }

        return descriptors;
    }
}

/// <summary>
/// Aggregates all <see cref="ISessionBlockRegistry"/> instances and resolves
/// component types by node type key.
/// </summary>
public sealed class SessionBlockDiscovery : ISessionBlockDiscovery
{
    private readonly IReadOnlyList<ISessionBlockDescriptor> _descriptors;

    public SessionBlockDiscovery(IEnumerable<ISessionBlockRegistry> registries)
    {
        _descriptors = registries
            .OrderByDescending(r => r.Priority)
            .SelectMany(r => r.GetDescriptors())
            .OrderByDescending(d => d.Priority)
            .ToList();
    }

    public Type? ResolveComponentType(string? nodeTypeKey)
    {
        if (string.IsNullOrWhiteSpace(nodeTypeKey))
            return null;

        return GetDescriptor(nodeTypeKey)?.ComponentType;
    }

    public ISessionBlockDescriptor? GetDescriptor(string? nodeTypeKey)
    {
        if (string.IsNullOrWhiteSpace(nodeTypeKey))
            return null;

        return _descriptors.FirstOrDefault(d =>
            string.Equals(d.NodeTypeKey, nodeTypeKey, StringComparison.OrdinalIgnoreCase));
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Discovery reads the types of the loaded assemblies, which a trimmed publish cannot name statically; the host that publishes trimmed preserves and registers the block and component types, which the trimmed-host preservation probe checks.")]
    public IReadOnlyList<ISessionBlockDescriptor> GetDescriptors() => _descriptors;
}
