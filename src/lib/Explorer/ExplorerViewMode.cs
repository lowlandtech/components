namespace LowlandTech.Components;

/// <summary>
/// Controls which panes are visible in <see cref="ExplorerLayout"/>.
/// </summary>
public enum ExplorerViewMode
{
    /// <summary>Left and right panes side-by-side with a draggable splitter. Classic default.</summary>
    SideBySide,

    /// <summary>Only the left list pane is visible. Default when nothing is selected.</summary>
    LeftOnly,

    /// <summary>Only the right detail pane is visible. Use for wizard/full-detail mode.</summary>
    RightOnly,

    /// <summary>Left pane stacked on top, right pane below.</summary>
    LeftOnTop,

    /// <summary>Left pane stacked on bottom, right pane on top.</summary>
    LeftOnBottom,
}
