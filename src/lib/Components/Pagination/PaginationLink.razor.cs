using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Pagination;

/// <summary>
/// PaginationLink component that provides a clickable link for pagination.
/// </summary>
public partial class PaginationLink : ComponentBase
{
    /// <summary>
    /// The content to render inside the pagination link.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The href attribute for the link.
    /// </summary>
    [Parameter]
    public string? Href { get; set; } = "#";

    /// <summary>
    /// Whether the link is active.
    /// </summary>
    [Parameter]
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the link is disabled.
    /// </summary>
    [Parameter]
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the pagination link.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Event callback for click events.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Additional attributes to apply to the pagination link element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "pagination-link",
            IsActive ? "pagination-link-active" : null,
            IsDisabled ? "pagination-link-disabled" : null,
            Class
        );
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (!IsDisabled)
        {
            await OnClick.InvokeAsync(e);
        }
    }
}
