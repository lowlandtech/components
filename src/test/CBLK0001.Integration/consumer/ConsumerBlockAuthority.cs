using LowlandTech.Components;

namespace LowlandTech.BlockConsumer;

/// <summary>
/// The authority of the consumer host over the plugin owners that contribute
/// session blocks, at one revision.
/// </summary>
/// <remarks>
/// The authority is the only state the circuits of the consumer share. Every
/// circuit owns its own bridge and follows the revision the authority holds.
/// </remarks>
public sealed class ConsumerBlockAuthority
{
    /// <summary>The owner that contributes the block of the consumer.</summary>
    public const string OwnerKey = "consumer.owner";

    /// <summary>The node type key the contributed block handles.</summary>
    public const string NodeTypeKey = "plugin.order";

    /// <summary>The symbolic identity the contributed block resolves through.</summary>
    public const string SymbolicIdentity = "Consumer.OwnedBlock";

    private readonly object _gate = new();
    private readonly List<Action<SessionBlockOwnerSnapshot>> _subscribers = new();
    private SessionBlockOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the authority with its first revision: the owner of the consumer
    /// is active.
    /// </summary>
    public ConsumerBlockAuthority()
    {
        _snapshot = new SessionBlockOwnerSnapshot(1, [OwnerKey]);
    }

    /// <summary>The authority the consumer currently holds.</summary>
    public SessionBlockOwnerSnapshot Snapshot
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
    public IDisposable Subscribe(Action<SessionBlockOwnerSnapshot> handler)
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
    public void Deliver(SessionBlockOwnerSnapshot snapshot)
    {
        Action<SessionBlockOwnerSnapshot>[] handlers;

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

    private void Unsubscribe(Action<SessionBlockOwnerSnapshot> handler)
    {
        lock (_gate)
        {
            _subscribers.Remove(handler);
        }
    }

    private sealed class Subscription(
        ConsumerBlockAuthority authority,
        Action<SessionBlockOwnerSnapshot> handler) : IDisposable
    {
        public void Dispose() => authority.Unsubscribe(handler);
    }
}
