namespace LowlandTech.Components;

/// <summary>
/// Default registry that aggregates all <see cref="ISessionPlugin"/> instances
/// registered in DI and projects them deterministically by placement.
/// </summary>
public sealed class SessionPluginRegistry : ISessionPluginRegistry
{
    private readonly IReadOnlyList<ISessionPlugin> _plugins;
    private readonly ISessionPluginSettingsStore? _settings;

    public SessionPluginRegistry(
        IEnumerable<ISessionPlugin> plugins,
        ISessionPluginSettingsStore? settings = null)
    {
        _plugins = plugins?.ToList() ?? [];
        _settings = settings;
    }

    public IReadOnlyList<ISessionPlugin> GetAllPlugins() => _plugins;

    public async Task<IReadOnlyList<SessionPluginDescriptorRepresentation>> ListAsync(
        SessionPluginContext context,
        SessionPluginPlacement placement,
        CancellationToken ct = default)
    {
        var seenKeys = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<(ISessionPlugin Plugin, SessionPluginDescriptor Descriptor)>();

        foreach (var plugin in _plugins.OrderBy(p => p.Descriptor.Order).ThenBy(p => p.Descriptor.Key, StringComparer.Ordinal))
        {
            var descriptor = plugin.Descriptor;
            if (descriptor.Placement != placement)
                continue;

            if (!seenKeys.Add(descriptor.Key))
                throw new InvalidOperationException($"Duplicate session plugin key '{descriptor.Key}'.");

            result.Add((plugin, descriptor));
        }

        var representations = new List<SessionPluginDescriptorRepresentation>(result.Count);
        foreach (var (plugin, descriptor) in result)
        {
            var isVisible = await plugin.IsVisibleAsync(context, ct).ConfigureAwait(false);
            if (isVisible && _settings is not null && !descriptor.IsProtected)
                isVisible = await _settings.IsEnabledAsync(descriptor.Key, ct).ConfigureAwait(false);

            var badge = isVisible ? await plugin.GetBadgeAsync(context, ct).ConfigureAwait(false) : null;
            representations.Add(new SessionPluginDescriptorRepresentation(
                descriptor.Key,
                descriptor.Label,
                descriptor.Icon,
                descriptor.Order,
                descriptor.Placement,
                descriptor.IsPinnedByDefault,
                descriptor.IsOrderLocked,
                badge,
                isVisible));
        }

        var order = _settings is not null
            ? await _settings.GetOrderAsync(ct).ConfigureAwait(false)
            : [];

        if (order.Count > 0)
        {
            var orderIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < order.Count; i++)
                orderIndex[order[i]] = i;

            representations = representations
                .OrderBy(r => orderIndex.TryGetValue(r.Key, out var idx) ? idx : int.MaxValue)
                .ThenBy(r => r.Order)
                .ThenBy(r => r.Key, StringComparer.Ordinal)
                .ToList();
        }

        return representations;
    }
}

