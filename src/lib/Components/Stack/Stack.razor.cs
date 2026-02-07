using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Stack;

/// <summary>
/// A flex container component for stacking elements horizontally or vertically with configurable spacing.
/// Based on MudBlazor Stack component.
/// </summary>
public partial class Stack : ComponentBase
{
    /// <summary>
    /// The content to display within the stack.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the stack container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the stack container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Whether to display items horizontally (row) instead of vertically (column).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c> (vertical stack).
    /// When <c>true</c>, items are displayed horizontally.
    /// </remarks>
    [Parameter]
    public bool Row { get; set; }

    /// <summary>
    /// The gap between items (0-8 representing different spacing sizes).
    /// </summary>
    /// <remarks>
    /// Defaults to 2 (medium spacing).
    /// Maps to CSS gap classes: gap-0 through gap-8.
    /// </remarks>
    [Parameter]
    public int Spacing { get; set; } = 2;

    private string GetClasses()
    {
        var direction = Row ? "stack-row" : "stack-col";
        var gap = Spacing >= 0 && Spacing <= 8 ? $"gap-{Spacing}" : "gap-2";
        
        return CssClassUtils.Merge("stack", direction, gap, Class);
    }
}
