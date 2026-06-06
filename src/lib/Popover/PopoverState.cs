namespace LowlandTech.Components;

public record PopoverState
{
    public bool Open { get; init; }
    public RenderFragment? Fragment { get; init; }
    public string? CssClass { get; init; }
    public PopoverPlacement Placement { get; init; } = PopoverPlacement.BottomStart;

    /// <summary>
    /// Absolute X coordinate for positioning (used by context menus and similar).
    /// When set, Placement is ignored and the popover is positioned at this exact coordinate.
    /// </summary>
    public double? X { get; init; }

    /// <summary>
    /// Absolute Y coordinate for positioning (used by context menus and similar).
    /// When set, Placement is ignored and the popover is positioned at this exact coordinate.
    /// </summary>
    public double? Y { get; init; }
}

public enum PopoverPlacement
{
    TopStart,
    Top,
    TopEnd,
    BottomStart,
    Bottom,
    BottomEnd,
    LeftStart,
    Left,
    LeftEnd,
    RightStart,
    Right,
    RightEnd
}
