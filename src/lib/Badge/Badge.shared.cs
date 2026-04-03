using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Badge;

/// <summary>
/// Extends the auto-generated Badge partial class.
/// </summary>
public partial class Badge
{
    /// <summary>
    /// The variant of the badge.
    /// </summary>
    [Parameter] public string Variant { get; set; } = "default";
}
