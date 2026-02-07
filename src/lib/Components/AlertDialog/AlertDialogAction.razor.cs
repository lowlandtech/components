using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.AlertDialog;

/// <summary>
/// Action button for alert dialog that performs the critical action.
/// Typically closes the dialog after action is performed.
/// </summary>
public partial class AlertDialogAction : ComponentBase
{
    /// <summary>
    /// The content to render inside the action button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Event callback when the action button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Additional attributes to apply to the action button.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
