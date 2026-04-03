using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Switch;

/// <summary>
/// Extends the auto-generated Switch partial class.
/// </summary>
public partial class Switch
{
    /// <summary>
    /// The size of the switch.
    /// </summary>
    [Parameter] public string Size { get; set; } = "default";

    /// <summary>
    /// Whether the switch is checked.
    /// </summary>
    [Parameter] public bool IsChecked { get; set; }

    /// <summary>
    /// Event callback when checked state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> IsCheckedChanged { get; set; }

    /// <summary>
    /// The name attribute for form integration.
    /// </summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>
    /// The value attribute for form integration.
    /// </summary>
    [Parameter] public string? Value { get; set; }
}
