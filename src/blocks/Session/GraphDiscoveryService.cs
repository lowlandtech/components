namespace LowlandTech.Components;

/// <summary>
/// Aggregates all <see cref="IAgentGraphRegistry"/> instances and resolves
/// the appropriate <see cref="IAgentGraphService"/> by agent type key.
/// Also supports generic keyed resolution of any <see cref="IKeyedGraphService"/>.
/// </summary>
public class GraphDiscoveryService : IGraphDiscoveryService
{
    private readonly IReadOnlyList<IAgentGraphRegistry> _orderedRegistries;
    private readonly IServiceProvider _serviceProvider;

    public GraphDiscoveryService(IEnumerable<IAgentGraphRegistry> registries)
        : this(registries, new EmptyServiceProvider())
    {
    }

    public GraphDiscoveryService(IEnumerable<IAgentGraphRegistry> registries, IServiceProvider serviceProvider)
    {
        _orderedRegistries = registries
            .OrderByDescending(r => r.Priority)
            .ToList();
        _serviceProvider = serviceProvider;
    }

    public IAgentGraphService? Get(string agentTypeKey)
    {
        foreach (var registry in _orderedRegistries)
        {
            var service = registry.GetService(agentTypeKey);
            if (service is not null) return service;
        }
        return null;
    }

    public TService? GetAsync<TService>(string key) where TService : class, IKeyedGraphService
    {
        var services = _serviceProvider.GetServices<TService>();
        return services.FirstOrDefault(service =>
            string.Equals(service.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Fallback provider used when no <see cref="IServiceProvider"/> is supplied.
    /// </summary>
    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
