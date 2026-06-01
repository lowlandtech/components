namespace LowlandTech.Components.Stories.Services.Themes;

/// <summary>
/// Chadcn-Black theme — close to shadcn-ui's neutral black/white defaults.
/// Pure greyscale with sharp contrast and minimal radius.
/// </summary>
public sealed class ChadcnBlackComponentTheme :
    IComponentTheme,
    IComponentClassTheme,
    IThemeStyleFamily
{
    public string Name => "ChadcnBlack";
    public string StyleFamily => "neutral";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#09090b",
        ["--foreground"]             = "#fafafa",
        ["--card"]                   = "#09090b",
        ["--card-foreground"]        = "#fafafa",
        ["--popover"]                = "#09090b",
        ["--popover-foreground"]     = "#fafafa",
        ["--primary"]                = "#fafafa",
        ["--primary-foreground"]     = "#18181b",
        ["--secondary"]              = "#27272a",
        ["--secondary-foreground"]   = "#fafafa",
        ["--muted"]                  = "#27272a",
        ["--muted-foreground"]       = "#a1a1aa",
        ["--accent"]                 = "#27272a",
        ["--accent-foreground"]      = "#fafafa",
        ["--destructive"]            = "#7f1d1d",
        ["--destructive-foreground"] = "#fafafa",
        ["--border"]                 = "#27272a",
        ["--input"]                  = "#27272a",
        ["--ring"]                   = "#d4d4d8",
        ["--radius"]                 = "0.25rem",
    };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            // Card — sharp-edge, solid background, heavy border
            [new("Card", "Root")] = new(
                "rounded-sm border border-border",
                ThemeClassMergeMode.Append),

            // Button default — solid white-on-black
            [new("Button", "Root", Variant: "default")] = new(
                "rounded-sm",
                ThemeClassMergeMode.Append),

            // Button outline — wire-frame
            [new("Button", "Root", Variant: "outline")] = new(
                "rounded-sm border-border",
                ThemeClassMergeMode.Append),

            // Badge — tight, square pill
            [new("Badge", "Root")] = new(
                "rounded-sm",
                ThemeClassMergeMode.Append),
        };
}
