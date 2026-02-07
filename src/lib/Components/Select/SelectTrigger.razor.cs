using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Select;

/// <summary>
/// SelectTrigger component that provides the trigger button for the Select dropdown.
/// </summary>
public partial class SelectTrigger : ComponentBase
{
    [CascadingParameter]
    private Select? ParentSelect { get; set; }

    /// <summary>
    /// The content to render inside the trigger.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The placeholder text when no value is selected.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; } = "Select an option...";

    /// <summary>
    /// Optional CSS classes to apply to the trigger.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the trigger element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsDisabled => ParentSelect?.IsDisabled ?? false;

    private string? DisplayValue => ParentSelect?.Value ?? Placeholder;

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "select-trigger",
            IsDisabled ? "select-trigger-disabled" : null,
            Class
        );
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (ParentSelect != null && !IsDisabled)
        {
            await ParentSelect.ToggleAsync();
        }
    }
}
