using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Dialog;

/// <summary>
/// Root Dialog component that manages the open/closed state.
/// Provides cascading state to child components (DialogTrigger, DialogContent).
/// </summary>
public partial class Dialog : ComponentBase
{
    private bool _open;

    /// <summary>
    /// The content to render inside the dialog context.
    /// Typically contains DialogTrigger and DialogContent components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Controls the open/closed state of the dialog.
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
    /// Opens the dialog.
    /// </summary>
    public void OpenDialog()
    {
        Open = true;
        StateHasChanged();
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public void CloseDialog()
    {
        Open = false;
        StateHasChanged();
    }

    /// <summary>
    /// Toggles the dialog open/closed state.
    /// </summary>
    public void ToggleDialog()
    {
        Open = !Open;
        StateHasChanged();
    }
}
