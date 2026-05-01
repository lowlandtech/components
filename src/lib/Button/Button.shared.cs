using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Button;

/// <summary>
/// Extends the auto-generated Button partial class.
/// </summary>
[Properties(Group = "Forms", Icon = "button",
    Description = "Clickable button with variant + size")]
public partial class Button
{
    /// <summary>
    /// The variant of the button.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "destructive", "outline", "secondary", "ghost", "link", "unstyled" })]
    public string Variant { get; set; } = "default";

    /// <summary>
    /// The size of the button.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "sm", "default", "lg", "icon" })]
    public string Size { get; set; } = "default";
}
