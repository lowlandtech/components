using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components;

/// <summary>
/// A utility component for setting text direction (LTR or RTL).
/// Propagates direction to child components via cascading value.
/// </summary>
public class DirectionBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the text direction. Defaults to LTR.
    /// </summary>
    [Parameter]
    public TextDirection Value { get; set; } = TextDirection.Ltr;

    /// <summary>
    /// Gets or sets the child content to render within the direction context.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the current direction to cascade to children.
    /// </summary>
    protected TextDirection CurrentDirection => Value;
}

/// <summary>
/// Text direction options.
/// </summary>
public enum TextDirection
{
    /// <summary>
    /// Left-to-right text direction.
    /// </summary>
    Ltr,

    /// <summary>
    /// Right-to-left text direction.
    /// </summary>
    Rtl
}
