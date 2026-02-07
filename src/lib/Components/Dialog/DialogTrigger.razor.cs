using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.Dialog;

/// <summary>
/// Trigger button that opens the dialog.
/// Must be used inside a Dialog component.
/// </summary>
public partial class DialogTrigger : ComponentBase
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
    /// Cascaded Dialog context.
    /// </summary>
    [CascadingParameter]
    private Dialog? DialogContext { get; set; }

    private void HandleClick(MouseEventArgs e)
    {
        DialogContext?.ToggleDialog();
    }

    protected override void OnInitialized()
    {
        if (DialogContext == null)
        {
            throw new InvalidOperationException("DialogTrigger must be used inside a Dialog component.");
        }
    }
}
