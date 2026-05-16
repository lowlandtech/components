using Microsoft.AspNetCore.Components.Rendering;

namespace LowlandTech.Components;

/// <summary>
/// Renders an SVG &lt;text&gt; element via BuildRenderTree, bypassing the Razor
/// parser's special treatment of the &lt;text&gt; tag (RZ1023).
/// </summary>
public sealed class SvgText : ComponentBase
{
    [Parameter] public int X { get; set; }
    [Parameter] public int Y { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? TextAnchor { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "text");
        builder.AddAttribute(1, "x", X);
        builder.AddAttribute(2, "y", Y);
        if (Class is not null)
            builder.AddAttribute(3, "class", Class);
        if (TextAnchor is not null)
            builder.AddAttribute(4, "text-anchor", TextAnchor);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
