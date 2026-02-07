using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LowlandTech.Components.Components.Portal;

/// <summary>
/// Portal component that renders content outside the normal DOM hierarchy (typically to document.body).
/// Useful for modals, dialogs, toasts, and other overlay elements that need to escape stacking contexts.
/// </summary>
public partial class Portal : ComponentBase, IAsyncDisposable
{
    private string _portalId = Guid.NewGuid().ToString("N");
    private bool _isInitialized;

    /// <summary>
    /// The content to render in the portal.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// CSS selector for the target element where the portal should render.
    /// Defaults to "body".
    /// </summary>
    [Parameter]
    public string Target { get; set; } = "body";

    /// <summary>
    /// Optional CSS classes to apply to the portal container.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_isInitialized)
        {
            try
            {
                // Create the portal container in the target element
                await JSRuntime.InvokeAsync<object?>(
                    "LowlandTechComponents.portalUtils.create",
                    _portalId,
                    Target,
                    ClassName ?? string.Empty
                );

                _isInitialized = true;
            }
            catch (JSException)
            {
                // JSInterop not available (e.g., during SSR) - graceful degradation
                // Content will render in place instead of in portal
            }
            catch (InvalidOperationException)
            {
                // JSRuntime not available - graceful degradation
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isInitialized)
        {
            try
            {
                // Clean up the portal container
                await JSRuntime.InvokeAsync<object?>(
                    "LowlandTechComponents.portalUtils.destroy",
                    _portalId
                );
            }
            catch (JSException)
            {
                // JSInterop not available - ignore
            }
            catch (InvalidOperationException)
            {
                // JSRuntime not available - ignore
            }
        }

        GC.SuppressFinalize(this);
    }
}
