using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components;
 
/// <summary>
/// A loading spinner component with size variants.
/// </summary>
public class Spinner : ComponentBase
{
    /// <summary>
    /// Gets or sets the size of the spinner.
    /// </summary>
    [Parameter]
    public SpinnerSize Size { get; set; } = SpinnerSize.Medium;

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
    protected string CssClass => CssClassUtils.Merge("spinner", SizeClass, Class);

    /// <summary>
    /// Gets the CSS class for the current size.
    /// </summary>
    private string SizeClass => Size switch
    {
        SpinnerSize.Small => "spinner-sm",
        SpinnerSize.Medium => "spinner-md",
        SpinnerSize.Large => "spinner-lg",
        _ => "spinner-md"
    };
}

/// <summary>
/// Size variants for the Spinner component.
/// </summary>
public enum SpinnerSize
{
    Small,
    Medium,
    Large
}
