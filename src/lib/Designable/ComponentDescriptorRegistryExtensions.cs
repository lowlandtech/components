using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LowlandTech.Components;

public static class ComponentDescriptorRegistryExtensions
{
    /// <summary>
    /// Register the <see cref="IComponentDescriptorRegistry"/> singleton.
    /// Idempotent — safe to call from multiple plugin entrypoints.
    /// SPEC2100.US45.
    /// </summary>
    public static IServiceCollection AddDesignableComponents(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        return services;
    }
}
