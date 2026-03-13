using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Icon;

public partial class Icon
{
    /// <summary>SVG path data from an Icons static class.</summary>
    [Parameter, EditorRequired]
    public string? Paths { get; set; }

    /// <summary>Width and height in pixels. Default: 20.</summary>
    [Parameter]
    public int Size { get; set; } = 20;

    /// <summary>SVG stroke-width. Default: 2.</summary>
    [Parameter]
    public double StrokeWidth { get; set; } = 2;

    /// <summary>Additional CSS classes applied to the svg element.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Accessible label. When set, renders aria-label and role="img". Leave null for decorative icons.</summary>
    [Parameter]
    public string? Label { get; set; }
}
