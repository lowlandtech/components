using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Badge;

/// <summary>
/// Badge component for displaying status indicators, labels, and tags.
/// Based on shadcn/ui Badge component.
/// </summary>
public partial class Badge : ComponentBase
{
    private static readonly Dictionary<BadgeVariants, string> VariantClasses = new()
    {
        { BadgeVariants.Default, "badge-default" },
        { BadgeVariants.Secondary, "badge-secondary" },
        { BadgeVariants.Destructive, "badge-destructive" },
        { BadgeVariants.Success, "badge-success" },
        { BadgeVariants.Outline, "badge-outline" }
    };

    /// <summary>
    /// The variant style to apply.
    /// </summary>
    [Parameter]
    public BadgeVariants Variant { get; set; } = BadgeVariants.Default;

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Child content of the badge.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to forward to the underlying span element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetBadgeClass()
    {
        var variantClass = VariantClasses.TryGetValue(Variant, out var vc) ? vc : "badge-default";
        return CssClassUtils.Merge(variantClass, Class);
    }
}
