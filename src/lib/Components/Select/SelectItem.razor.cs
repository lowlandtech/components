using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Select;

/// <summary>
/// SelectItem component that provides an option in the Select dropdown.
/// </summary>
public partial class SelectItem : ComponentBase
{
    [CascadingParameter]
    private Select? ParentSelect { get; set; }

    /// <summary>
    /// The content to render inside the item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The value of the item.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Whether the item is disabled.
    /// </summary>
    [Parameter]
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the item element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsSelected => ParentSelect?.Value == Value;

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "select-item",
            IsSelected ? "select-item-selected" : null,
            IsDisabled ? "select-item-disabled" : null,
            Class
        );
    }

    protected override void OnInitialized()
    {
        if (ParentSelect != null)
        {
            ParentSelect.RegisterItem(this);
        }
    }

    public void Dispose()
    {
        if (ParentSelect != null)
        {
            ParentSelect.UnregisterItem(this);
        }
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (ParentSelect != null && !IsDisabled)
        {
            await ParentSelect.SelectItemAsync(Value);
        }
    }
}
