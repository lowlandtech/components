using LowlandTech.Components.Core;
using LowlandTech.Components.Features.Pages;
using LowlandTech.PageConsumer.Components;
using LowlandTech.Plugins.Shell;

namespace LowlandTech.PageConsumer;

/// <summary>
/// The route host of one circuit of the consumer.
/// </summary>
/// <remarks>
/// The host projects the authority of the consumer onto the route and page
/// primitives of the Plugins shell: the component catalog carries the pages of
/// the active owners and the page catalog carries their adapted metadata. A
/// withdrawn owner leaves both catalogs, so the route gateway of the shell can
/// no longer resolve or offer its page.
/// </remarks>
public sealed class ConsumerPageHost
{
    private readonly ConsumerActivationSource _activation;
    private readonly ComponentsStartupMap _startupMap;
    private readonly ComponentPageOwnerBridge _bridge;
    private PluginRouteHost _host = null!;

    /// <summary>The route host the circuit renders through.</summary>
    public PluginRouteHost Host => _host;

    /// <summary>
    /// Raised when the projection of the authority changed.
    /// </summary>
    /// <remarks>
    /// The route host of the Plugins shell is immutable, so a withdrawal
    /// replaces it. The components that render through it follow this signal,
    /// so the outlet and the navigation resolve the new projection instead of
    /// the one they were rendered with.
    /// </remarks>
    public event Action? Changed;

    /// <summary>
    /// Creates the route host of a circuit and publishes the page of the owner
    /// through the bridge.
    /// </summary>
    public ConsumerPageHost(ConsumerPageAuthority authority, ConsumerActivationSource activation)
    {
        ArgumentNullException.ThrowIfNull(authority);
        ArgumentNullException.ThrowIfNull(activation);

        _activation = activation;

        // The trusted startup map of the host: the symbolic identity of the
        // contributed page resolves to its type here and nowhere else.
        _startupMap = new ComponentsStartupMap(
            [
                new ComponentsStartupBinding(
                    ConsumerPageAuthority.OwnerKey,
                    ConsumerPageAuthority.SymbolicIdentity,
                    typeof(ConsumerPluginPage)),
            ],
            [ConsumerPageAuthority.OwnerKey]);

        // The route of the host itself stays reserved, so a page of an owner can
        // never claim it.
        _bridge = new ComponentPageOwnerBridge(_startupMap, authority.Snapshot, ["/"]);

        var validation = _bridge.Publish(new ComponentPageBinding(
            ConsumerPageAuthority.OwnerKey,
            ConsumerPageAuthority.SymbolicIdentity,
            ConsumerPageAuthority.PageRoute));

        if (!validation.IsValid)
        {
            throw new InvalidOperationException(
                "The consumer could not publish the page of its owner: " +
                string.Join("; ", validation.Errors.Select(error => error.Message)));
        }

        Rebuild();
    }

    /// <summary>
    /// Delivers the next revision of the authority to the bridge and rebuilds
    /// the route host of the circuit.
    /// </summary>
    /// <param name="snapshot">The authority of the next revision.</param>
    public void Deliver(ComponentPageOwnerSnapshot snapshot)
    {
        _bridge.Deliver(snapshot);
        Rebuild();

        Console.WriteLine(
            $"[host] revision {_bridge.Revision} active [{string.Join(",", snapshot.ActiveOwnerKeys)}] " +
            $"pages {_bridge.Enumerate().Count}");

        Changed?.Invoke();
    }

    /// <summary>
    /// Rebuilds the route host from the projection the bridge holds. The
    /// component catalog carries the pages of the active owners only, so a
    /// withdrawn owner leaves the gateway without a target.
    /// </summary>
    private void Rebuild()
    {
        var components = _bridge.Enumerate().Select(binding =>
        {
            _startupMap.TryResolveComponent(binding.SymbolicIdentity, out var componentType);

            return new KeyValuePair<(string PluginKey, string Identity), Type>(
                (binding.OwnerKey, binding.SymbolicIdentity),
                componentType!);
        });

        _host = new PluginRouteHost
        {
            Registry = _activation,
            Components = new PluginComponentCatalog(components),
            Pages = new PluginPageCatalog(_bridge.PageCatalog().Entries),
        };
    }
}
