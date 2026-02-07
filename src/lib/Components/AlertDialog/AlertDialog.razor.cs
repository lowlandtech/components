using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.AlertDialog;

/// <summary>
/// Root AlertDialog component that manages the open/closed state.
/// Provides cascading state to child components (AlertDialogTrigger, AlertDialogContent).
/// AlertDialog is used for critical actions requiring explicit user confirmation.
/// </summary>
public partial class AlertDialog : ComponentBase
{
    private bool _open;

    /// <summary>
    /// The content to render inside the alert dialog context.
    /// Typically contains AlertDialogTrigger and AlertDialogContent components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Controls the open/closed state of the alert dialog.
    /// </summary>
    [Parameter]
    public bool Open
    {
        get => _open;
        set
        {
            if (_open != value)
            {
                _open = value;
                OpenChanged.InvokeAsync(_open);
            }
        }
    }

    /// <summary>
    /// Event callback when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Opens the alert dialog.
    /// </summary>
    public void OpenAlertDialog()
    {
        Open = true;
        StateHasChanged();
    }

    /// <summary>
    /// Closes the alert dialog.
    /// </summary>
    public void CloseAlertDialog()
    {
        Open = false;
        StateHasChanged();
    }

    /// <summary>
    /// Toggles the alert dialog open/closed state.
    /// </summary>
    public void ToggleAlertDialog()
    {
        Open = !Open;
        StateHasChanged();
    }
}
