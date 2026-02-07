using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Dialog;

/// <summary>
/// Dialog content component that renders via Portal with overlay, focus trap, and escape handling.
/// Must be used inside a Dialog component.
/// </summary>
public partial class DialogContent : ComponentBase, IAsyncDisposable
{
    private string _contentId = $"dialog-content-{Guid.NewGuid():N}";
    private FocusTrap? _focusTrap;
    private bool _disposed;

    /// <summary>
    /// The content to render inside the dialog.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the dialog content container.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// Whether to close the dialog when clicking outside (on the overlay).
    /// Defaults to true.
    /// </summary>
    [Parameter]
    public bool CloseOnOverlayClick { get; set; } = true;

    /// <summary>
    /// Whether to close the dialog when pressing Escape.
    /// Defaults to true.
    /// </summary>
    [Parameter]
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Additional attributes to apply to the dialog content container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Cascaded Dialog context.
    /// </summary>
    [CascadingParameter]
    private Dialog? DialogContext { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (DialogContext == null)
        {
            throw new InvalidOperationException("DialogContent must be used inside a Dialog component.");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (DialogContext?.Open == true && _focusTrap == null)
        {
            // Initialize focus trap when dialog opens
            _focusTrap = new FocusTrap(JSRuntime, _contentId);
            await _focusTrap.ActivateAsync(restoreFocusOnRelease: true);
        }
        else if (DialogContext?.Open == false && _focusTrap != null)
        {
            // Deactivate focus trap when dialog closes
            await _focusTrap.DeactivateAsync();
            await _focusTrap.DisposeAsync();
            _focusTrap = null;
        }
    }

    private void OnOverlayClick()
    {
        if (CloseOnOverlayClick)
        {
            DialogContext?.CloseDialog();
        }
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (CloseOnEscape && e.Key == "Escape")
        {
            DialogContext?.CloseDialog();
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
