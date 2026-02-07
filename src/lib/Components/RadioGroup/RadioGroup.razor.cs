using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.RadioGroup;

/// <summary>
/// RadioGroup component that groups radio button options.
/// Based on shadcn/ui RadioGroup component with dark theme styling.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public partial class RadioGroup<TValue> : ComponentBase
{
    /// <summary>
    /// The content to render inside the radio group (typically RadioGroupItem components).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The currently selected value.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Callback invoked when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the radio group.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the radio group element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Selects a radio item with the given value.
    /// </summary>
    internal async Task SelectValueAsync(TValue value)
    {
        if (!EqualityComparer<TValue>.Default.Equals(Value, value))
        {
            Value = value;
            await ValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// Checks if a value is currently selected.
    /// </summary>
    internal bool IsSelected(TValue value)
    {
        return EqualityComparer<TValue>.Default.Equals(Value, value);
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "radio-group",
            Class
        );
    }
}
