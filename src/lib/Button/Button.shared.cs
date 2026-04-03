using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Button;

/// <summary>
/// Extends the auto-generated Button partial class.
/// </summary>
public partial class Button
{
    /// <summary>
    /// The variant of the button.
    /// </summary>
    [Parameter] public string Variant { get; set; } = "default";

    /// <summary>
    /// The size of the button.
    /// </summary>
    [Parameter] public string Size { get; set; } = "default";
}
