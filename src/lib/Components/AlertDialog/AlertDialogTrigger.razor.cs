using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.AlertDialog;

/// <summary>
/// Trigger button that opens the alert dialog.
/// Must be used inside an AlertDialog component.
/// </summary>
public partial class AlertDialogTrigger : ComponentBase
{
    /// <summary>
    /// The content to render inside the trigger button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to apply to the trigger button.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Cascaded AlertDialog context.
    /// </summary>
    [CascadingParameter]
    private AlertDialog? AlertDialogContext { get; set; }

    private void HandleClick(MouseEventArgs e)
    {
        AlertDialogContext?.ToggleAlertDialog();
    }

    protected override void OnInitialized()
    {
        if (AlertDialogContext == null)
        {
            throw new InvalidOperationException("AlertDialogTrigger must be used inside an AlertDialog component.");
        }
    }
}
