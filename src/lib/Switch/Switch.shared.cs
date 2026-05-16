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

    /// <summary>
    /// Whether the switch is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Whether the switch has an invalid/error state.
    /// </summary>
    [Parameter] public bool Invalid { get; set; }

    /// <summary>
    /// Whether to render as a Choice Card pattern (bordered container with label).
    /// </summary>
    [Parameter] public bool ChoiceCard { get; set; }

    /// <summary>
    /// Title text for the Choice Card pattern.
    /// </summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>
    /// Description text for the Choice Card pattern.
    /// </summary>
    [Parameter] public string? Description { get; set; }
}
