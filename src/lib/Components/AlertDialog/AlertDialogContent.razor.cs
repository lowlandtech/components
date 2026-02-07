using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.AlertDialog;

/// <summary>
/// AlertDialog content component that renders via Portal with overlay, focus trap, and escape handling.
/// Must be used inside an AlertDialog component.
/// AlertDialog uses role="alertdialog" and typically prevents closing on overlay click for critical actions.
/// </summary>
public partial class AlertDialogContent : ComponentBase, IAsyncDisposable
{
    private string _contentId = $"alert-dialog-content-{Guid.NewGuid():N}";
    private FocusTrap? _focusTrap;
    private bool _disposed;

    /// <summary>
    /// The content to render inside the alert dialog.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the alert dialog content container.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// Whether to close the alert dialog when clicking outside (on the overlay).
    /// Defaults to false for alert dialogs to prevent accidental dismissal during critical actions.
    /// </summary>
    [Parameter]
    public bool CloseOnOverlayClick { get; set; } = false;

    /// <summary>
    /// Whether to close the alert dialog when pressing Escape.
    /// Defaults to true, but can be disabled for critical actions requiring explicit confirmation.
    /// </summary>
    [Parameter]
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Additional attributes to apply to the alert dialog content container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Cascaded AlertDialog context.
    /// </summary>
    [CascadingParameter]
    private AlertDialog? AlertDialogContext { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (AlertDialogContext == null)
        {
            throw new InvalidOperationException("AlertDialogContent must be used inside an AlertDialog component.");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (AlertDialogContext?.Open == true && _focusTrap == null)
        {
            // Initialize focus trap when alert dialog opens
            _focusTrap = new FocusTrap(JSRuntime, _contentId);
            await _focusTrap.ActivateAsync(restoreFocusOnRelease: true);
        }
        else if (AlertDialogContext?.Open == false && _focusTrap != null)
        {
            // Deactivate focus trap when alert dialog closes
            await _focusTrap.DeactivateAsync();
            await _focusTrap.DisposeAsync();
            _focusTrap = null;
        }
    }

    private void OnOverlayClick()
    {
        if (CloseOnOverlayClick)
        {
            AlertDialogContext?.CloseAlertDialog();
        }
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (CloseOnEscape && e.Key == "Escape")
        {
            AlertDialogContext?.CloseAlertDialog();
        }
    }

    private string GetContentClasses()
    {
        var baseClasses = "fixed left-1/2 top-1/2 z-50 -translate-x-1/2 -translate-y-1/2 bg-white dark:bg-gray-800 shadow-lg rounded-lg p-6 outline-none";
        return string.IsNullOrWhiteSpace(ClassName) ? baseClasses : $"{baseClasses} {ClassName}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_focusTrap != null)
        {
            await _focusTrap.DisposeAsync();
            _focusTrap = null;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
