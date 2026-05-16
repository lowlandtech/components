namespace LowlandTech.Components.Badge;

/// <summary>
/// Extends the auto-generated Badge partial class.
/// </summary>
[Properties(Group = "Display", Icon = "badge",
    Description = "Inline status / category pill")]
public partial class Badge
{
    /// <summary>
    /// The variant of the badge.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "secondary", "destructive", "outline" })]
    public string Variant { get; set; } = "default";
}
