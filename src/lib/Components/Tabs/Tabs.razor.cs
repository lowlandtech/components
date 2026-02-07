using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Tabs;

/// <summary>
/// Tabs component for organizing content into switchable panels.
/// Based on shadcn/ui Tabs component with dark theme styling.
/// </summary>
public partial class Tabs : ComponentBase
{
    private string? _value;

    /// <summary>
    /// The content to render inside the tabs container.
    /// Should contain TabsList and TabsContent components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The currently active tab value.
    /// </summary>
    [Parameter]
    public string? Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                ValueChanged.InvokeAsync(value);
            }
        }
    }

    /// <summary>
    /// Event callback when the active tab changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// The default active tab value if Value is not set.
    /// </summary>
    [Parameter]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// The orientation of the tabs.
    /// Options: "horizontal" (default), "vertical".
    /// </summary>
    [Parameter]
    public string Orientation { get; set; } = "horizontal";

    /// <summary>
    /// Optional CSS classes to apply to the tabs container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tabs container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void OnInitialized()
    {
        if (_value == null && DefaultValue != null)
        {
            _value = DefaultValue;
        }
    }

    /// <summary>
    /// Sets the active tab value.
    /// Called by TabsTrigger components.
    /// </summary>
    public async Task SetValueAsync(string? value)
    {
        if (_value != value)
        {
            _value = value;
            await ValueChanged.InvokeAsync(value);
            await InvokeAsync(StateHasChanged);
        }
    }

    private string GetClasses()
    {
        var baseClasses = "tabs";
        if (Orientation == "vertical")
        {
            baseClasses += " tabs-vertical";
        }
        else
        {
            baseClasses += " tabs-horizontal";
        }
        return CssClassUtils.Merge(baseClasses, Class);
    }
}
