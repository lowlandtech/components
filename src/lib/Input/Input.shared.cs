using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Input;

/// <summary>
/// Extends the auto-generated Input partial class.
/// </summary>
[Properties(Group = "Forms", Icon = "form-input",
    Description = "Single-line text input")]
public partial class Input
{
    /// <summary>
    /// The variant of the input.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "destructive" })]
    public string Variant { get; set; } = "default";

    /// <summary>
    /// The size of the input.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "sm", "default", "lg" })]
    public string Size { get; set; } = "default";

    /// <summary>
    /// The placeholder text.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Behavior",
        Placeholder = "Enter your text…",
        Description = "Hint text shown when the input is empty")]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the input is disabled.
    /// </summary>
    [Parameter, PropertyGrid(Group = "State")]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the input is read-only.
    /// </summary>
    [Parameter, PropertyGrid(Group = "State", Label = "Read only")]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Whether the input has an invalid/error state.
    /// </summary>
    [Parameter, PropertyGrid(Group = "State")]
    public bool Invalid { get; set; }

    /// <summary>
    /// The value of the input.
    /// </summary>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// Callback when the value changes.
    /// </summary>
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
}
