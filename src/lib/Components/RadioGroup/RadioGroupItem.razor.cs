using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.RadioGroup;

/// <summary>
/// RadioGroupItem component that represents a single radio option.
/// Based on shadcn/ui RadioGroup component with dark theme styling.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public partial class RadioGroupItem<TValue> : ComponentBase
{
    /// <summary>
    /// The content to render next to the radio button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The value represented by this radio item.
    /// </summary>
    [Parameter, EditorRequired]
    public TValue Value { get; set; } = default!;

    /// <summary>
    /// Whether this radio item is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the radio item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the radio item element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The parent RadioGroup component.
    /// </summary>
    [CascadingParameter]
    public RadioGroup<TValue>? RadioGroup { get; set; }

    private bool IsChecked => RadioGroup?.IsSelected(Value) ?? false;

    private async Task HandleClickAsync()
    {
        if (!Disabled && RadioGroup != null)
        {
            await RadioGroup.SelectValueAsync(Value);
        }
    }

    private async Task HandleChangeAsync(ChangeEventArgs args)
    {
        if (!Disabled && RadioGroup != null && args.Value is bool isChecked && isChecked)
        {
            await RadioGroup.SelectValueAsync(Value);
        }
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "radio-group-item",
            Disabled ? "radio-group-item-disabled" : null,
            Class
        );
    }
}
