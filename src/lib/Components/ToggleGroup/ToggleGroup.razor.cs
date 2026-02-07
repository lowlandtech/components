using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ToggleGroup;

/// <summary>
/// ToggleGroup component for grouping toggle buttons with single or multiple selection.
/// Based on shadcn/ui ToggleGroup component.
/// </summary>
public partial class ToggleGroup : ComponentBase
{
    /// <summary>
    /// Selection mode: "single" for radio-like behavior, "multiple" for checkbox-like behavior.
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "single";

    /// <summary>
    /// The currently selected value(s). For single mode, use a single string. For multiple mode, use comma-separated values.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Event callback for when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Default value (for uncontrolled mode).
    /// </summary>
    [Parameter]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Whether the toggle group is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Child content containing ToggleGroupItem components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to forward to the div element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private HashSet<string> _selectedValues = new();

    protected override void OnInitialized()
    {
        // Initialize from Value or DefaultValue
        var initialValue = Value ?? DefaultValue;
        if (!string.IsNullOrEmpty(initialValue))
        {
            _selectedValues = new HashSet<string>(initialValue.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }

    protected override void OnParametersSet()
    {
        // Sync with external Value parameter
        if (Value != null)
        {
            _selectedValues = new HashSet<string>(Value.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "toggle-group",
            Disabled ? "toggle-group-disabled" : "",
            Class
        );
    }

    internal bool IsSelected(string value)
    {
        return _selectedValues.Contains(value);
    }

    internal async Task ToggleItem(string value)
    {
        if (Disabled) return;

        if (Type == "single")
        {
            // Radio-like: only one can be selected
            _selectedValues.Clear();
            _selectedValues.Add(value);
        }
        else
        {
            // Checkbox-like: toggle on/off
            if (_selectedValues.Contains(value))
            {
                _selectedValues.Remove(value);
            }
            else
            {
                _selectedValues.Add(value);
            }
        }

        var newValue = string.Join(",", _selectedValues);
        await ValueChanged.InvokeAsync(newValue);
    }
}
