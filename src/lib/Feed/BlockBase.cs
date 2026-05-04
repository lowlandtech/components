using Microsoft.AspNetCore.Components;
using LowlandTech.Core.Graph;

namespace LowlandTech.Components.Feed;

public abstract class BlockBase : ComponentBase, IBlock, IDisposable
{
    [Parameter] public string? NodeId { get; set; }
    [Parameter] public string? Name { get; set; }
    [Parameter] public string? BlockType { get; set; }
    [Parameter] public string? ParentNodeId { get; set; }
    [Parameter] public Guid MessageId { get; set; }
    [Parameter] public Guid BlockId { get; set; }
    [Parameter] public EventCallback<Guid> OnDeleted { get; set; }
    [CascadingParameter(Name = "SessionSettings")] protected IReadOnlyDictionary<string, string>? SessionSettings { get; set; }

    /// <summary>
    /// Optional injection — the validity channel BlockBase uses to publish
    /// <see cref="IsValid"/> changes keyed by <see cref="BlockId"/>. Hosts
    /// without BlockContext registered (rare) just won't get publications.
    /// </summary>
    [Inject] protected BlockContext? BlockContext { get; set; }

    private bool? _lastPublishedValid;

    protected string BlockSpacing => SessionBlockSpacingOptions.Normalize(
        SessionSettings?.GetValueOrDefault(SessionBlockSpacingOptions.SettingKey));

    public virtual bool SupportsActivationToggle => true;
    public virtual IReadOnlyList<BlockIntent> IntentSurface => [];

    /// <summary>
    /// Override to expose validation state. Default <c>true</c> — a block with
    /// no rules is always valid. The block can use FluentValidation or any
    /// other strategy under the hood; consumers read the result through
    /// <see cref="Feed.BlockContext.GetValid"/> /
    /// <see cref="Feed.BlockContext.SubscribeValid"/> keyed by
    /// <see cref="BlockId"/>.
    /// </summary>
    public virtual bool IsValid => true;

    /// <summary>
    /// Override to map a semantic intent key + payload to the block's own store
    /// actions. Default is unsupported.
    /// </summary>
    public virtual Task<BlockIntentDispatchResult> DispatchIntentAsync(
        string key,
        IReadOnlyDictionary<string, object?>? payload,
        CancellationToken ct = default)
        => Task.FromResult<BlockIntentDispatchResult>(new BlockIntentDispatchResult.Unsupported(key));

    /// <summary>
    /// Publishes <see cref="IsValid"/> to <see cref="BlockContext"/> when its
    /// value changes. Runs after every render, deduped by the last published
    /// value, so blocks that flip between valid/invalid as the user types
    /// notify subscribers exactly on transitions.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (BlockContext is null || BlockId == Guid.Empty) return;

        if (firstRender)
            BlockContext.RegisterIntent(BlockId, () => IntentSurface, DispatchIntentAsync);

        var current = IsValid;
        if (_lastPublishedValid == current) return;

        _lastPublishedValid = current;
        BlockContext.SetValid(BlockId, current);
    }

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

    public virtual void Dispose()
    {
        if (BlockContext is null || BlockId == Guid.Empty)
            return;

        BlockContext.ClearValid(BlockId);
        BlockContext.UnregisterIntent(BlockId);
    }
}
