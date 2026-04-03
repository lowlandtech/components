using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Input;

/// <summary>
/// Extends the auto-generated Input partial class.
/// </summary>
public partial class Input
{
    /// <summary>
    /// The variant of the input.
    /// </summary>
    [Parameter] public string Variant { get; set; } = "default";

    /// <summary>
    /// The size of the input.
    /// </summary>
    [Parameter] public string Size { get; set; } = "default";

    /// <summary>
    /// The placeholder text.
    /// </summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the input is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Whether the input is read-only.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// The value of the input.
    /// </summary>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// Callback when the value changes.
    /// </summary>
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
}
