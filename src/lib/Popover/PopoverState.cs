using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Popover;

public record PopoverState
{
    public bool Open { get; init; }
    public RenderFragment? Fragment { get; init; }
    public string? CssClass { get; init; }
    public PopoverPlacement Placement { get; init; } = PopoverPlacement.BottomStart;
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
