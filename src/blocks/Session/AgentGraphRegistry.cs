namespace LowlandTech.Components;

/// <summary>
/// Default component-level registry that aggregates all registered <see cref="IAgentGraphService"/>
/// instances and resolves them by <see cref="IAgentGraphService.AgentTypeKey"/>.
/// </summary>
public sealed class AgentGraphRegistry : IAgentGraphRegistry
{
    private readonly IReadOnlyDictionary<string, IAgentGraphService> _services;

    public AgentGraphRegistry(IEnumerable<IAgentGraphService> services)
    {
        _services = services.ToDictionary(
            s => s.AgentTypeKey,
            StringComparer.OrdinalIgnoreCase);
    }

    public IAgentGraphService? GetService(string agentTypeKey)
    {
        _services.TryGetValue(agentTypeKey, out var service);
        return service;
    }
}
