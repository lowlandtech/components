using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Avatar;

/// <summary>
/// Extends the auto-generated Avatar partial class.
/// </summary>
public partial class Avatar
{
    /// <summary>
    /// The size of the avatar.
    /// </summary>
    [Parameter] public string Size { get; set; } = "default";
}
