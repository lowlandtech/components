using LowlandTech.Components;

namespace LowlandTech.ThemeConsumer;

/// <summary>
/// The authority of the consumer host over the plugin owners that contribute
/// themes, at one revision.
/// </summary>
/// <remarks>
/// <para>
/// The authority is the only state the circuits of the consumer share. Every
/// circuit owns its own resolver and its own bridge and follows the revision
/// the authority holds, exactly as a host delivers its authority to the panel
/// and block scopes.
/// </para>
/// <para>
/// The authority never carries a theme. It names the owners that are active,
/// and the owner contributes its theme through the bridge of the circuit.
/// </para>
/// </remarks>
public sealed class ThemeAuthority
{
    /// <summary>The owner that contributes the theme of the consumer.</summary>
    public const string OwnerKey = "consumer.owner";

    /// <summary>The key of the theme inside its owner.</summary>
    public const string ThemeKey = "brand";

    /// <summary>The provider boundary the consumer renders.</summary>
    public const string ProviderId = "consumer.provider";

    private readonly object _gate = new();
    private readonly List<Action<ComponentThemeOwnerSnapshot>> _subscribers = new();
    private ComponentThemeOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the authority with its first revision: the owner of the consumer
    /// is active.
    /// </summary>
    public ThemeAuthority()
    {
        _snapshot = new ComponentThemeOwnerSnapshot(1, [OwnerKey]);
    }

    /// <summary>The authority the consumer currently holds.</summary>
    public ComponentThemeOwnerSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                return _snapshot;
            }
        }
    }

    /// <summary>
    /// Follows the revisions of the authority.
    /// </summary>
    /// <param name="handler">Called with every revision the authority holds.</param>
    public IDisposable Subscribe(Action<ComponentThemeOwnerSnapshot> handler)
    {
        lock (_gate)
        {
            _subscribers.Add(handler);
        }

        return new Subscription(this, handler);
    }

    /// <summary>
    /// Delivers the next revision of the authority. Every live circuit follows
    /// it.
    /// </summary>
    /// <param name="snapshot">The authority of the next revision.</param>
    public void Deliver(ComponentThemeOwnerSnapshot snapshot)
    {
        Action<ComponentThemeOwnerSnapshot>[] handlers;

        lock (_gate)
        {
            _snapshot = snapshot;
            handlers = [.. _subscribers];
        }

        foreach (var handler in handlers)
        {
            handler(snapshot);
        }
    }

    private void Unsubscribe(Action<ComponentThemeOwnerSnapshot> handler)
    {
        lock (_gate)
        {
            _subscribers.Remove(handler);
        }
    }

    private sealed class Subscription(
        ThemeAuthority authority,
        Action<ComponentThemeOwnerSnapshot> handler) : IDisposable
    {
        public void Dispose() => authority.Unsubscribe(handler);
    }
}
