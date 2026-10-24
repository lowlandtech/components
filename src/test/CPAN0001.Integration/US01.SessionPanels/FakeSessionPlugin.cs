namespace LowlandTech.Components.Tests.CPAN0001.Integration.US01.SessionPanels;

/// <summary>
/// A session plugin used to arrange the acceptance of the plugin owner bridge.
/// </summary>
public sealed class FakeSessionPlugin : ISessionPlugin
{
    private readonly Func<SessionPluginContext, RenderFragment> _render;

    public FakeSessionPlugin(
        SessionPluginDescriptor descriptor,
        Func<SessionPluginContext, RenderFragment>? render = null)
    {
        Descriptor = descriptor;
        _render = render ?? (_ => builder =>
        {
            builder.AddContent(0, descriptor.Label);
        });
    }

    /// <summary>The descriptor the plugin publishes.</summary>
    public SessionPluginDescriptor Descriptor { get; }

    /// <summary>Decides whether the plugin is visible for a context.</summary>
    public Func<SessionPluginContext, bool> Visibility { get; init; } = _ => true;

    /// <summary>Produces the badge of the plugin.</summary>
    public Func<SessionPluginContext, string?> Badge { get; init; } = _ => null;

    /// <summary>Counts how often the plugin rendered.</summary>
    public int RenderCount { get; private set; }

    /// <summary>The settings the plugin contributes.</summary>
    public object? Settings { get; init; }

    public ValueTask<bool> IsVisibleAsync(SessionPluginContext context, CancellationToken ct = default) =>
        ValueTask.FromResult(Visibility(context));

    public ValueTask<string?> GetBadgeAsync(SessionPluginContext context, CancellationToken ct = default) =>
        ValueTask.FromResult(Badge(context));

    public object? GetDefaultSettings() => Settings;

    public RenderFragment Render(SessionPluginContext context)
    {
        RenderCount++;
        return _render(context);
    }
}
