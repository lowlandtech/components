using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Select;

/// <summary>
/// SelectContent component that provides the dropdown content for the Select.
/// </summary>
public partial class SelectContent : ComponentBase
{
    [CascadingParameter]
    private Select? ParentSelect { get; set; }

    /// <summary>
    /// The content to render inside the dropdown.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether to render via Portal.
    /// </summary>
    [Parameter]
    public bool UsePortal { get; set; } = true;

    /// <summary>
    /// Optional CSS classes to apply to the content.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the content element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsVisible => ParentSelect?.IsOpen ?? false;

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "select-content",
            Class
        );
    }
}
