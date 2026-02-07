using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Kbd;

/// <summary>
/// Kbd component that renders keyboard input styling.
/// Based on shadcn/ui Kbd component with dark theme styling.
/// Useful for displaying keyboard shortcuts inline within text.
/// </summary>
public partial class Kbd : ComponentBase
{
    /// <summary>
    /// The keyboard key or shortcut text to display.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the kbd element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the kbd element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "kbd",
            Class
        );
    }
}
