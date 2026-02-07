using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.Pagination;

/// <summary>
/// PaginationNext component that provides a next page button.
/// </summary>
public partial class PaginationNext : ComponentBase
{
    /// <summary>
    /// The content to render inside the pagination next button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The href attribute for the link.
    /// </summary>
    [Parameter]
    public string? Href { get; set; } = "#";

    /// <summary>
    /// Whether the link is disabled.
    /// </summary>
    [Parameter]
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Event callback for click events.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Additional attributes to apply to the element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
