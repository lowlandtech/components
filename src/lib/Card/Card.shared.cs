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

    /// <summary>
    /// The visual variant of the card.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "outlined" })]
    public string Variant { get; set; } = "default";

    /// <summary>
    /// The elevation (shadow depth) of the card. Ignored when <see cref="Variant"/> is "outlined".
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Number)]
    public int Elevation { get; set; } = 1;

    /// <summary>
    /// Whether the card should have square corners (no border-radius).
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool Square { get; set; }

    /// <summary>
    /// The display mode of the card.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "normal", "minimized", "maximized", "overlay-page", "overlay-app" })]
    public string Mode { get; set; } = "normal";

    /// <summary>
    /// Event callback when the card mode changes.
    /// </summary>
    [Parameter] public EventCallback<string> ModeChanged { get; set; }
}
