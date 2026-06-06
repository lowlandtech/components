namespace LowlandTech.Components;

/// <summary>
/// Extends the auto-generated Label partial class.
/// </summary>
public partial class Label
{
    private const string BaseClass = "cn-label flex items-center select-none group-data-[disabled=true]:pointer-events-none peer-disabled:cursor-not-allowed";

    /// <summary>
    /// Whether the field is required. Shows an asterisk when true.
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// Description text displayed below the label.
    /// </summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>
    /// The size of the label.
    /// </summary>
    [Parameter] public string Size { get; set; } = "md";

    /// <summary>
    /// Whether the label is for a disabled field.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// The 'for' attribute to associate with a form input.
    /// </summary>
    [Parameter] public string? For { get; set; }

    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? BaseClass
        : $"{BaseClass} {Class}";

    private string GetSizeClass() => Size switch
    {
        "sm" => "text-xs",
        "lg" => "text-base",
        _ => "text-sm"
    };
}
