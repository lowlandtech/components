using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components;

/// <summary>
/// A progress bar component that displays completion percentage.
/// </summary>
public class ProgressBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the progress value from 0 to 100.
    /// </summary>
    [Parameter]
    public double Value { get; set; } = 0;

    /// <summary>
    /// Gets or sets the CSS class for the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the computed CSS class for the component.
    /// </summary>
    protected string CssClass => CssClassUtils.Merge("progress", Class);
}
