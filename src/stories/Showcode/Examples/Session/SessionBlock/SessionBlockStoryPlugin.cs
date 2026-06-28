namespace LowlandTech.Components.Stories.Showcode.Examples.Session;

public sealed class SessionBlockStoryPlugin : ISessionPlugin
{
    public SessionPluginDescriptor Descriptor { get; } = new(
        "story.session-tools",
        "Tools",
        "panel-right",
        10,
        SessionPluginPlacement.RightSidebar,
        true,
        IsProtected: true);

    public ValueTask<bool> IsVisibleAsync(SessionPluginContext context, CancellationToken ct = default)
        => ValueTask.FromResult(true);

    public ValueTask<string?> GetBadgeAsync(SessionPluginContext context, CancellationToken ct = default)
        => ValueTask.FromResult<string?>(context.ContextNodes.Count.ToString(CultureInfo.InvariantCulture));

    public RenderFragment Render(SessionPluginContext context) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "data-slot", "session-block-story-plugin");
        builder.AddAttribute(2, "class", "space-y-2 text-xs");
        builder.OpenElement(3, "p");
        builder.AddAttribute(4, "class", "font-medium text-foreground");
        builder.AddContent(5, "Registered plugin");
        builder.CloseElement();
        builder.OpenElement(6, "p");
        builder.AddAttribute(7, "class", "text-muted-foreground");
        builder.AddContent(8, $"Active session: {context.ActiveSessionId ?? "none"}");
        builder.CloseElement();
        builder.OpenElement(9, "p");
        builder.AddAttribute(10, "class", "text-muted-foreground");
        builder.AddContent(11, $"Context roots: {context.ContextNodes.Count}");
        builder.CloseElement();
        builder.CloseElement();
    };
}

