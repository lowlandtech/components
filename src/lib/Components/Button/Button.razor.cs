using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Button;

/// <summary>
/// Button component that displays a clickable button element with various visual variants and sizes.
/// Based on shadcn/ui Button component with dark theme styling.
/// </summary>
public partial class Button : ComponentBase
{
    /// <summary>
    /// The content to render inside the button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The button variant style.
    /// Defaults to Default.
    /// </summary>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    /// <summary>
    /// The button size.
    /// Defaults to Default.
    /// </summary>
    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    /// <summary>
    /// The HTML button type attribute.
    /// Defaults to "button".
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <summary>
    /// Whether the button is disabled.
    /// Defaults to false.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the button.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Event callback for button click events.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Additional attributes to apply to the button element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "btn",
            GetVariantClass(),
            GetSizeClass(),
            Class
        );
    }

    private string? GetVariantClass()
    {
        return Variant switch
        {
            ButtonVariant.Default => "btn-default",
            ButtonVariant.Secondary => "btn-secondary",
            ButtonVariant.Outline => "btn-outline",
            ButtonVariant.Ghost => "btn-ghost",
            ButtonVariant.Link => "btn-link",
            ButtonVariant.Destructive => "btn-destructive",
            _ => null
        };
    }

    private string? GetSizeClass()
    {
        return Size switch
        {
            ButtonSize.Small => "btn-sm",
            ButtonSize.Large => "btn-lg",
            ButtonSize.Icon => "btn-icon",
            ButtonSize.Default => null,
            _ => null
        };
    }
}

/// <summary>
/// Button visual variants matching shadcn/ui button styles.
/// </summary>
public enum ButtonVariant
{
    /// <summary>
    /// Default primary button style (purple background).
    /// </summary>
    Default,

    /// <summary>
    /// Secondary button style (muted background).
    /// </summary>
    Secondary,

    /// <summary>
    /// Outline button style (transparent with border).
    /// </summary>
    Outline,

    /// <summary>
    /// Ghost button style (transparent, no border).
    /// </summary>
    Ghost,

    /// <summary>
    /// Link button style (appears as a link).
    /// </summary>
    Link,

    /// <summary>
    /// Destructive button style (red background for dangerous actions).
    /// </summary>
    Destructive
}

/// <summary>
/// Button size variants.
/// </summary>
public enum ButtonSize
{
    /// <summary>
    /// Default button size.
    /// </summary>
    Default,

    /// <summary>
    /// Small button size.
    /// </summary>
    Small,

    /// <summary>
    /// Large button size.
    /// </summary>
    Large,

    /// <summary>
    /// Icon button size (square, for icon-only buttons).
    /// </summary>
    Icon
}
