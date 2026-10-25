using System.Collections.Immutable;
using LowlandTech.Plugins;

namespace LowlandTech.PageConsumer;

/// <summary>
/// The Plugins activation snapshot of the consumer.
/// </summary>
/// <remarks>
/// <para>
/// The plugin of the consumer stays installed, so its route keeps being
/// declared in the activation snapshot. The withdrawal of its owner is not an
/// uninstall: the authority of the host decides whether the declared page is
/// published, and the route gateway of the shell serves only what that
/// projection authorizes.
/// </para>
/// <para>
/// The snapshot never changes, so the change event of the source is not
/// raised. The Components authority drives the refresh of the gateway instead.
/// </para>
/// </remarks>
public sealed class ConsumerActivationSource : IPluginActivationSource
{
    /// <summary>The activation snapshot the consumer declares.</summary>
    public PluginActivationSnapshot Current { get; }

    /// <summary>
    /// Raised when the activation snapshot changes. The consumer never changes
    /// its snapshot, so the event is never raised.
    /// </summary>
    public event Action? SnapshotChanged;

    /// <summary>
    /// The consumer resolves no contribution reference of its own.
    /// </summary>
    public PluginContributionReference? ResolveContribution(PluginContributionKind kind, string identity) => null;

    /// <summary>
    /// Creates the activation source with the manifest of the consumer plugin.
    /// </summary>
    public ConsumerActivationSource()
    {
        Current = new PluginActivationSnapshot
        {
            Revision = Guid.NewGuid(),
            ProjectPath = "consumer",
            Plugins =
            [
                new ActivatedPlugin
                {
                    PackageId = "LowlandTech.PageConsumer",
                    Version = "1.0.0",
                    Sha512 = "consumer",
                    Manifest = new PluginManifest
                    {
                        SchemaVersion = 2,
                        Key = ConsumerPageAuthority.OwnerKey,
                        Name = "Consumer page owner",
                        Description = "The plugin owner that contributes the page of the consumer.",
                        Target = PluginTarget.Blazor,
                        Routes =
                        [
                            new PluginRoute
                            {
                                Path = "plugin/reports",
                                Component = ConsumerPageAuthority.SymbolicIdentity,
                            },
                        ],
                        Contributions = new PluginContributions
                        {
                            Pages = [ConsumerPageAuthority.SymbolicIdentity],
                        },
                    },
                },
            ],
        };
    }

    /// <summary>
    /// Raises the change event of the source.
    /// </summary>
    public void NotifySnapshotChanged() => SnapshotChanged?.Invoke();
}
