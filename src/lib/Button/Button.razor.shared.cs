namespace LowlandTech.Components.Button;

/// <summary>Visual style variant constants for the Button component.</summary>
public static class ButtonVariant
{
    public const string Default     = "default";
    public const string Primary     = "default";  // alias — maps to same CSS as Default
    public const string Secondary   = "secondary";
    public const string Destructive = "destructive";
    public const string Outline     = "outline";
    public const string Ghost       = "ghost";
    public const string Link        = "link";
    /// <summary>
    /// Opt out of variant-level bg/height/padding. The button still gets the
    /// `.btn` base (typography, transitions, focus ring, disabled state)
    /// but no `btn-default` / `btn-secondary` / etc. visual chrome. Use this
    /// when the consumer applies its own full styling via a `class="..."`
    /// (e.g. the canvas Property Grid's `lt-style-section-header` / `lt-
    /// property-panel-menu-item` rules) and the default variant's
    /// `bg-primary h-9 px-4 py-2` would fight those rules.
    /// </summary>
    public const string Unstyled   = "unstyled";
}

/// <summary>Size preset constants for the Button component.</summary>
public static class ButtonSize
{
    public const string Default = "default";
    public const string Sm      = "sm";
    public const string Lg      = "lg";
    public const string Icon    = "icon";
    public const string IconSm  = "icon-sm";
}

// This file extends the auto-generated Button partial class.
public partial class Button
{
    private static string VariantClass(string v) => v switch
    {
        "destructive" => "btn-destructive",
        "outline"     => "btn-outline",
        "secondary"   => "btn-secondary",
        "ghost"       => "btn-ghost",
        "link"        => "btn-link",
        "unstyled"    => string.Empty,
        _             => "btn-default"
    };

    private static string SizeClass(string s) => s switch
    {
        "sm"      => "btn-sm",
        "lg"      => "btn-lg",
        "icon"    => "btn-icon",
        "icon-sm" => "btn-icon-sm",
        _         => string.Empty
    };
}
