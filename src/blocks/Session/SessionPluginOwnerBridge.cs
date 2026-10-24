namespace LowlandTech.Components;

/// <summary>
/// The host authority over the plugin owners that may contribute session
/// panels.
/// </summary>
/// <param name="Revision">
/// The revision of the authority. The host owns the revision and delivers it
/// to the bridge, which projects the revision it holds.
/// </param>
/// <param name="ActiveOwnerKeys">
/// The owners that are active in this revision. An owner that is not named
/// here contributes no panel.
/// </param>
public sealed record SessionPluginOwnerSnapshot(
    long Revision,
    IReadOnlyCollection<string> ActiveOwnerKeys);

/// <summary>
/// Projects the active plugin owners onto the session panel contracts.
///
/// <para>
/// The host owns which plugin owners are active and delivers that authority
/// to the bridge as a revision. The bridge projects the owners of the
/// revision it holds: a panel of an inactive owner is not projected, and a
/// withdrawal that arrives as the next revision removes the panels of that
/// owner from every lookup path.
/// </para>
/// <para>
/// The bridge reuses the rules of the default registry, so the persisted
/// enablement, the persisted order, the badge of a visible panel, the
/// protected descriptor and the reload callback keep exactly the semantics
/// they have without a bridge. A protected descriptor protects a panel
/// against the persisted enablement only: it never survives the deactivation
/// of its owner, and a reactivation never erases the persisted preferences.
/// </para>
/// </summary>
public sealed class SessionPluginOwnerBridge : ISessionPluginRegistry
{
    private readonly ISessionPluginSettingsStore? _settings;
    private readonly List<(string? OwnerKey, ISessionPlugin Plugin)> _entries = [];
    private readonly List<Action<long>> _subscribers = [];
    private SessionPluginOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the bridge over the panels a host registered and the authority
    /// it holds.
    /// </summary>
    /// <param name="snapshot">The authority the bridge projects.</param>
    /// <param name="plugins">
    /// The panels of the host. A panel that is never registered with an owner
    /// belongs to the host itself and stays projected.
    /// </param>
    /// <param name="settings">The persisted preferences of the host.</param>
    public SessionPluginOwnerBridge(
        SessionPluginOwnerSnapshot snapshot,
        IEnumerable<ISessionPlugin> plugins,
        ISessionPluginSettingsStore? settings = null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(plugins);

        _snapshot = snapshot;
        _settings = settings;

        foreach (var plugin in plugins)
        {
            ArgumentNullException.ThrowIfNull(plugin);

            _entries.Add((null, plugin));
        }
    }

    /// <summary>The revision the bridge currently projects.</summary>
    public long Revision => _snapshot.Revision;

    /// <summary>The authority the bridge currently projects.</summary>
    public SessionPluginOwnerSnapshot Snapshot => _snapshot;

    /// <summary>
    /// Whether the bridge is deactivated, that is whether its current
    /// revision names no active owner. A deactivated bridge admits no new
    /// handler.
    /// </summary>
    public bool IsDeactivated => _snapshot.ActiveOwnerKeys.Count == 0;

    /// <summary>
    /// Registers the panel an owner contributes. The panel leaves the panels
    /// of the host itself and belongs to that owner from now on.
    /// </summary>
    /// <param name="ownerKey">The owner that contributes the panel.</param>
    /// <param name="plugin">The panel the owner contributes.</param>
    public void Register(string ownerKey, ISessionPlugin plugin)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerKey);
        ArgumentNullException.ThrowIfNull(plugin);

        for (var index = 0; index < _entries.Count; index++)
        {
            if (!ReferenceEquals(_entries[index].Plugin, plugin))
            {
                continue;
            }

            _entries[index] = (ownerKey, plugin);
            return;
        }

        _entries.Add((ownerKey, plugin));
    }

    /// <summary>
    /// Delivers the next revision of the host authority. The subscribers that
    /// were admitted before are told about the revision.
    /// </summary>
    /// <param name="snapshot">The authority the host delivers.</param>
    public void Deliver(SessionPluginOwnerSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        _snapshot = snapshot;

        foreach (var subscriber in _subscribers.ToArray())
        {
            subscriber(snapshot.Revision);
        }
    }

    /// <summary>
    /// Subscribes to the revisions the bridge delivers.
    ///
    /// <para>
    /// No handler is newly admitted once the bridge is deactivated: a handler
    /// that arrives while no owner is active is never told about a revision,
    /// because the bridge is closed to new handlers after a withdrawal. The
    /// work that was admitted before keeps running.
    /// </para>
    /// </summary>
    /// <param name="onRevision">The handler that is told about a revision.</param>
    /// <returns>A subscription that ends the admission of the handler.</returns>
    public IDisposable Subscribe(Action<long> onRevision)
    {
        ArgumentNullException.ThrowIfNull(onRevision);

        if (!IsDeactivated)
        {
            _subscribers.Add(onRevision);
        }

        return new Subscription(() => _subscribers.Remove(onRevision));
    }

    /// <summary>
    /// Whether the owner is active in the revision the bridge projects.
    /// </summary>
    /// <param name="ownerKey">The owner to check.</param>
    public bool IsOwnerActive(string ownerKey) =>
        _snapshot.ActiveOwnerKeys.Contains(ownerKey, StringComparer.Ordinal);

    /// <summary>
    /// The panels the bridge projects: the panels of the host itself and the
    /// panels of the owners that are active.
    /// </summary>
    public IReadOnlyList<ISessionPlugin> GetAllPlugins() =>
        _entries
            .Where(entry => entry.OwnerKey is null || IsOwnerActive(entry.OwnerKey))
            .Select(entry => entry.Plugin)
            .ToList();

    /// <summary>
    /// The panels of the selected placement, with the rules of the default
    /// registry.
    /// </summary>
    /// <param name="context">The context every panel is asked for.</param>
    /// <param name="placement">The placement the host selected.</param>
    /// <param name="ct">The cancellation token.</param>
    public Task<IReadOnlyList<SessionPluginDescriptorRepresentation>> ListAsync(
        SessionPluginContext context,
        SessionPluginPlacement placement,
        CancellationToken ct = default) =>
        new SessionPluginRegistry(GetAllPlugins(), _settings).ListAsync(context, placement, ct);

    /// <summary>
    /// Ends the admission of a handler when it is disposed.
    /// </summary>
    private sealed class Subscription(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;

        public void Dispose() => Interlocked.Exchange(ref _unsubscribe, null)?.Invoke();
    }
}
