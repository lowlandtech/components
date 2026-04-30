using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Card;

/// <summary>
/// Extends the auto-generated Card partial class.
/// </summary>
[Properties(Group = "Layout", Icon = "card",
    Description = "Container surface with header / footer slots")]
public partial class Card
{
    /// <summary>
    /// The size of the card.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "sm", "default", "lg" })]
    public string Size { get; set; } = "default";
}
