namespace LowlandTech.Components.Session;

/// <summary>
/// Aggregates all <see cref="IAgentGraphRegistry"/> instances and resolves
/// the appropriate <see cref="IAgentGraphService"/> by agent type key.
/// </summary>
public class GraphDiscoveryService(IEnumerable<IAgentGraphRegistry> registries) : IGraphDiscoveryService
{
    private readonly IReadOnlyList<IAgentGraphRegistry> _orderedRegistries = registries
        .OrderByDescending(r => r.Priority)
        .ToList();

    public IAgentGraphService? Get(string agentTypeKey)
    {
        foreach (var registry in _orderedRegistries)
        {
            var service = registry.GetService(agentTypeKey);
            if (service is not null) return service;
        }
        return null;
    }
}

/// <summary>
/// Default registry that collects all <see cref="IAgentGraphService"/> implementations
/// from the DI container into a single lookup table.
/// </summary>
public class AgentGraphRegistry(IEnumerable<IAgentGraphService> services) : IAgentGraphRegistry
{
    private readonly Dictionary<string, IAgentGraphService> _services = services.ToDictionary(
        s => s.AgentTypeKey,
        StringComparer.OrdinalIgnoreCase);

    public IAgentGraphService? GetService(string agentTypeKey)
        => _services.TryGetValue(agentTypeKey, out var service) ? service : null;
}
