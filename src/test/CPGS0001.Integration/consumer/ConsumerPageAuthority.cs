using LowlandTech.Components.Features.Pages;

namespace LowlandTech.PageConsumer;

/// <summary>
/// The authority of the consumer host over the plugin owners that contribute
/// pages, at one revision.
/// </summary>
/// <remarks>
/// The authority is the only state the circuits of the consumer share. Every
/// circuit owns its own bridge and follows the revision the authority holds.
/// </remarks>
public sealed class ConsumerPageAuthority
{
    /// <summary>The owner that contributes the page of the consumer.</summary>
    public const string OwnerKey = "consumer.owner";

    /// <summary>The symbolic identity the contributed page resolves through.</summary>
    public const string SymbolicIdentity = "Consumer.PluginPage";

    /// <summary>The route the contributed page is served under.</summary>
    public const string PageRoute = "/plugin/reports";

    private readonly object _gate = new();
    private readonly List<Action<ComponentPageOwnerSnapshot>> _subscribers = new();
    private ComponentPageOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the authority with its first revision: the owner of the consumer
    /// is active.
    /// </summary>
    public ConsumerPageAuthority()
    {
        _snapshot = new ComponentPageOwnerSnapshot(1, [OwnerKey]);
    }

    /// <summary>The authority the consumer currently holds.</summary>
    public ComponentPageOwnerSnapshot Snapshot
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
    /// Whether the owner of the consumer is active in the revision the
    /// authority holds.
    /// </summary>
    public bool IsOwnerActive(string ownerKey) =>
        Snapshot.ActiveOwnerKeys.Contains(ownerKey, StringComparer.Ordinal);

    /// <summary>
    /// Follows the revisions of the authority.
    /// </summary>
    /// <param name="handler">Called with every revision the authority holds.</param>
    public IDisposable Subscribe(Action<ComponentPageOwnerSnapshot> handler)
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
    public void Deliver(ComponentPageOwnerSnapshot snapshot)
    {
        Action<ComponentPageOwnerSnapshot>[] handlers;

        Console.WriteLine(
            $"[authority] revision {snapshot.Revision} active [{string.Join(",", snapshot.ActiveOwnerKeys)}] " +
            $"subscribers {_subscribers.Count}");

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

    private void Unsubscribe(Action<ComponentPageOwnerSnapshot> handler)
    {
        lock (_gate)
        {
            _subscribers.Remove(handler);
        }
    }

    private sealed class Subscription(
        ConsumerPageAuthority authority,
        Action<ComponentPageOwnerSnapshot> handler) : IDisposable
    {
        public void Dispose() => authority.Unsubscribe(handler);
    }
}
