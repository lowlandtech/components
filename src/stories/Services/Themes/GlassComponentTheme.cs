namespace LowlandTech.Components.Stories.Services.Themes;

/// <summary>
/// Glass theme — translucent surfaces, backdrop blur, softer borders, and
/// layered elevation.  Requires a dark or image background behind the provider
/// boundary to look correct.
/// </summary>
public sealed class GlassComponentTheme :
    IComponentTheme,
    IComponentClassTheme,
    IThemeStyleFamily
{
    public string Name => "Glass";
    public string StyleFamily => "glass";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "oklch(0.18 0.03 260)",
        ["--foreground"]             = "oklch(0.96 0.01 260)",
        ["--card"]                   = "oklch(0.28 0.03 260 / 0.58)",
        ["--card-foreground"]        = "oklch(0.96 0.01 260)",
        ["--popover"]                = "oklch(0.22 0.03 260 / 0.75)",
        ["--popover-foreground"]     = "oklch(0.96 0.01 260)",
        ["--primary"]                = "oklch(0.65 0.18 270)",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "oklch(0.55 0.14 200)",
        ["--secondary-foreground"]   = "#ffffff",
        ["--muted"]                  = "oklch(0.30 0.02 260 / 0.50)",
        ["--muted-foreground"]       = "oklch(0.72 0.03 260)",
        ["--accent"]                 = "oklch(0.35 0.05 260 / 0.50)",
        ["--accent-foreground"]      = "oklch(0.96 0.01 260)",
        ["--destructive"]            = "oklch(0.60 0.20 25)",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "oklch(0.95 0.01 260 / 0.18)",
        ["--input"]                  = "oklch(0.95 0.01 260 / 0.18)",
        ["--ring"]                   = "oklch(0.65 0.18 270)",
        // Surface effect tokens
        ["--surface-blur"]           = "18px",
        ["--surface-opacity"]        = "0.62",
        ["--surface-border-opacity"] = "0.18",
    };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            // Card — translucent frosted-glass surface
            [new("Card", "Root")] = new(
                "backdrop-blur-xl border border-white/15 bg-card/70 shadow-2xl",
                ThemeClassMergeMode.Append),

            // Button default — semi-transparent with blur
            [new("Button", "Root", Variant: "default")] = new(
                "backdrop-blur-md border border-white/10",
                ThemeClassMergeMode.Append),

            // Button outline — glass border treatment
            [new("Button", "Root", Variant: "outline")] = new(
                "backdrop-blur-sm border-white/20 bg-white/5 hover:bg-white/10",
                ThemeClassMergeMode.Append),

            // Badge — frosted pill
            [new("Badge", "Root")] = new(
                "backdrop-blur-sm border border-white/15 bg-white/10",
                ThemeClassMergeMode.Append),
        };
}
