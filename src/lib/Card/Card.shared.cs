using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Card;

/// <summary>
/// Extends the auto-generated Card partial class.
/// </summary>
public partial class Card
{
    /// <summary>
    /// The size of the card.
    /// </summary>
    [Parameter] public string Size { get; set; } = "default";
}
