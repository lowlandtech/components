using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Feed;

public abstract class BlockBase : ComponentBase, IBlock
{
    [Parameter] public string? NodeId { get; set; }
    [Parameter] public string? Name { get; set; }
    [Parameter] public string? ParentNodeId { get; set; }
    [Parameter] public Guid MessageId { get; set; }
    [Parameter] public EventCallback<Guid> OnDeleted { get; set; }
    [CascadingParameter(Name = "SessionSettings")] protected IReadOnlyDictionary<string, string>? SessionSettings { get; set; }
    protected string BlockSpacing => SessionBlockSpacingOptions.Normalize(
        SessionSettings?.GetValueOrDefault(SessionBlockSpacingOptions.SettingKey));

    public virtual bool SupportsActivationToggle => true;

    protected string InactiveBannerClass => "flex items-center gap-2 border-b border-amber-500/30 bg-amber-500/10 px-3 py-2 text-[11px] font-semibold uppercase tracking-[0.22em] text-amber-700";

    /// <summary>
    /// Gets the rounded corner class based on block spacing setting.
    /// Returns "rounded-lg" when spacing is on, "rounded-none" when off.
    /// </summary>
    protected string BlockRoundedClass => SessionBlockSpacingOptions.IsEnabled(BlockSpacing) ? "rounded-lg" : "rounded-none";

    /// <summary>
    /// Gets the background class based on block spacing setting.
    /// Returns "bg-muted/50" when spacing is on, "bg-transparent" when off.
    /// </summary>
    protected string BlockBackgroundClass => SessionBlockSpacingOptions.IsEnabled(BlockSpacing) ? "bg-muted/50" : "bg-transparent";

    /// <summary>
    /// Gets the border class based on block spacing setting.
    /// Returns "border border-border" when spacing is on, empty when off.
    /// </summary>
    protected string BlockBorderClass => SessionBlockSpacingOptions.IsEnabled(BlockSpacing) ? "border border-border" : "";

    /// <summary>
    /// Gets the padding class based on block spacing setting.
    /// Returns "p-3" when spacing is on, "p-0" when off.
    /// </summary>
    protected string BlockPaddingClass => SessionBlockSpacingOptions.IsEnabled(BlockSpacing) ? "p-3" : "p-0";

    /// <summary>
    /// Gets a complete container class string with rounded, background, and optional border.
    /// </summary>
    protected string GetBlockContainerClass(string? additionalClasses = null)
    {
        var classes = $"{BlockRoundedClass} {BlockBackgroundClass}";
        if (!string.IsNullOrEmpty(BlockBorderClass))
            classes += " " + BlockBorderClass;
        if (!string.IsNullOrEmpty(additionalClasses))
            classes += " " + additionalClasses;
        return classes.Trim();
    }

    protected string GetActivationContainerClass(string baseClass, bool isActive)
        => isActive ? baseClass : $"{baseClass} relative opacity-70";

    protected string GetActivationStateValue(bool isActive)
        => isActive ? "active" : "inactive";

    protected bool ShowInactiveState(bool isActive)
        => !isActive;
}
