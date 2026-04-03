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
}
