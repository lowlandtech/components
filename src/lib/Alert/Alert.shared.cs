using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Alert;

/// <summary>
/// Extends the auto-generated Alert partial class.
/// </summary>
public partial class Alert
{
    /// <summary>
    /// The variant of the alert.
    /// </summary>
    [Parameter] public string Variant { get; set; } = "default";
}
