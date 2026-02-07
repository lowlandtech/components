using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.AlertDialog;

/// <summary>
/// Cancel button that closes the alert dialog without taking action.
/// Must be used inside an AlertDialog component.
/// </summary>
public partial class AlertDialogCancel : ComponentBase
{
    /// <summary>
    /// The content to render inside the cancel button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to apply to the cancel button.
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
        AlertDialogContext?.CloseAlertDialog();
    }

    protected override void OnInitialized()
    {
        if (AlertDialogContext == null)
        {
            throw new InvalidOperationException("AlertDialogCancel must be used inside an AlertDialog component.");
        }
    }
}
