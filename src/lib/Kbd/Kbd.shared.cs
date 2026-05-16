namespace LowlandTech.Components.Kbd;

/// <summary>
/// Extends the auto-generated Kbd partial class.
/// </summary>
public partial class Kbd
{
    /// <summary>
    /// The size of the keyboard key.
    /// Options: "sm", "md" (default), "lg"
    /// </summary>
    [Parameter] public string Size { get; set; } = "md";
}
