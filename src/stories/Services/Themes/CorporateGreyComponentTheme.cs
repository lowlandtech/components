namespace LowlandTech.Components.Stories.Services.Themes;

/// <summary>
/// Corporate-Grey theme — restrained enterprise neutral palette.
/// Tighter radius, minimal shadow, high information density.
/// </summary>
public sealed class CorporateGreyComponentTheme :
    IComponentTheme,
    IComponentClassTheme,
    IThemeStyleFamily
{
    public string Name => "CorporateGrey";
    public string StyleFamily => "neutral";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#f4f4f5",
        ["--foreground"]             = "#18181b",
        ["--card"]                   = "#ffffff",
        ["--card-foreground"]        = "#18181b",
        ["--popover"]                = "#ffffff",
        ["--popover-foreground"]     = "#18181b",
        ["--primary"]                = "#3b82f6",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "#6b7280",
        ["--secondary-foreground"]   = "#ffffff",
        ["--muted"]                  = "#e4e4e7",
        ["--muted-foreground"]       = "#71717a",
        ["--accent"]                 = "#dbeafe",
        ["--accent-foreground"]      = "#1e3a8a",
        ["--destructive"]            = "#dc2626",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "#d1d5db",
        ["--input"]                  = "#d1d5db",
        ["--ring"]                   = "#3b82f6",
        ["--radius"]                 = "0.25rem",
        // Tighter density
        ["--control-height-sm"]      = "1.75rem",
        ["--control-height-md"]      = "2rem",
        ["--control-height-lg"]      = "2.25rem",
        ["--density-scale"]          = "0.875",
    };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            // Card — flat, heavily bordered, no elevation
            [new("Card", "Root")] = new(
                "rounded-sm border border-border shadow-none",
                ThemeClassMergeMode.Append),

            // Button default — tight, flat
            [new("Button", "Root", Variant: "default")] = new(
                "rounded-sm h-8 px-3 text-sm",
                ThemeClassMergeMode.Append),

            // Button outline — flat wire frame
            [new("Button", "Root", Variant: "outline")] = new(
                "rounded-sm h-8 px-3 text-sm",
                ThemeClassMergeMode.Append),

            // Badge — small, rectangular
            [new("Badge", "Root")] = new(
                "rounded-sm text-xs",
                ThemeClassMergeMode.Append),
        };
}
