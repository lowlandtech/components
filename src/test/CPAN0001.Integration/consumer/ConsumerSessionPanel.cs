using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.SessionConsumer;

/// <summary>
/// The panel a plugin owner contributes to the session surface of the
/// consumer.
/// </summary>
/// <remarks>
/// The panel is owned by the consumer, not by the Components packages: it is
/// the plain <see cref="ISessionPlugin"/> contract of the Blocks package, so
/// the consumer proves that a host can contribute a panel without changing the
/// base packages.
/// </remarks>
public sealed class ConsumerSessionPanel : ISessionPlugin
{
    /// <summary>The key of the panel in the session surface.</summary>
    public const string PanelKey = "consumer.panel";

    /// <summary>The descriptor the panel publishes.</summary>
    public SessionPluginDescriptor Descriptor { get; } = new(
        PanelKey,
        "Consumer panel",
        "panel",
        10,
        SessionPluginPlacement.RightSidebar,
        IsPinnedByDefault: false);

    /// <summary>The panel is always visible while its owner is active.</summary>
    public ValueTask<bool> IsVisibleAsync(SessionPluginContext context, CancellationToken ct = default) =>
        ValueTask.FromResult(true);

    /// <summary>Renders the content of the panel.</summary>
    public RenderFragment Render(SessionPluginContext context) =>
        builder => builder.AddContent(0, "Consumer panel content");
}
