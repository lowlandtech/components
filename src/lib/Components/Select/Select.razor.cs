using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Select;

/// <summary>
/// Select component that provides a dropdown selection control.
/// Based on shadcn/ui Select component with dark theme styling.
/// </summary>
public partial class Select : ComponentBase
{
    /// <summary>
    /// The content to render inside the select.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The currently selected value.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Event callback for when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Whether the select is disabled.
    /// </summary>
    [Parameter]
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Whether the select is open.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Event callback for when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the select.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the select element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    internal List<SelectItem> Items { get; } = new();

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "select",
            Class
        );
    }

    internal void RegisterItem(SelectItem item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
        }
    }

    internal void UnregisterItem(SelectItem item)
    {
        Items.Remove(item);
    }

    internal async Task SelectItemAsync(string? itemValue)
    {
        if (Value != itemValue)
        {
            Value = itemValue;
            await ValueChanged.InvokeAsync(Value);
        }
        await CloseAsync();
    }

    internal async Task ToggleAsync()
    {
        if (!IsDisabled)
        {
            IsOpen = !IsOpen;
            await IsOpenChanged.InvokeAsync(IsOpen);
        }
    }

    internal async Task CloseAsync()
    {
        if (IsOpen)
        {
            IsOpen = false;
            await IsOpenChanged.InvokeAsync(IsOpen);
        }
    }
}
