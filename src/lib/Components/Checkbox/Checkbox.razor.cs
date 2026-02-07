using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Checkbox;

/// <summary>
/// Checkbox component for toggling between checked, unchecked, and indeterminate states.
/// Based on shadcn/ui Checkbox component with dark theme styling.
/// </summary>
public partial class Checkbox : ComponentBase, IAsyncDisposable
{
    private ElementReference _checkboxRef;
    private bool _disposed;

    /// <summary>
    /// Whether the checkbox is checked.
    /// Supports two-way binding via @bind-Checked.
    /// </summary>
    [Parameter]
    public bool Checked { get; set; }

    /// <summary>
    /// Event callback invoked when the checked state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// Whether the checkbox is in an indeterminate state.
    /// Indeterminate state is typically used in tree views or when a parent checkbox
    /// has some but not all child items checked.
    /// </summary>
    [Parameter]
    public bool Indeterminate { get; set; }

    /// <summary>
    /// Whether the checkbox is disabled.
    /// Defaults to false.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Optional ID attribute for the checkbox element.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the checkbox.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the checkbox element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Indeterminate)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "eval",
                    $"document.getElementById('{Id}').indeterminate = true"
                );
            }
            catch (JSException)
            {
                // JSInterop not available - ignore
            }
        }
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (Disabled) return;

        var newValue = e.Value is bool boolValue ? boolValue : false;
        
        if (Checked != newValue)
        {
            Checked = newValue;
            await CheckedChanged.InvokeAsync(Checked);
        }
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "checkbox",
            Class
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
