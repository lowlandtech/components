namespace LowlandTech.Components.Stories.Services.Themes;

/// <summary>Forest theme — lush green palette.</summary>
public sealed class ForestComponentTheme : IComponentTheme
{
    public string Name => "Forest";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#f0fdf4",
        ["--foreground"]             = "#052e16",
        ["--card"]                   = "#dcfce7",
        ["--card-foreground"]        = "#052e16",
        ["--popover"]                = "#dcfce7",
        ["--popover-foreground"]     = "#052e16",
        ["--primary"]                = "#16a34a",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "#22c55e",
        ["--secondary-foreground"]   = "#ffffff",
        ["--muted"]                  = "#bbf7d0",
        ["--muted-foreground"]       = "#166534",
        ["--accent"]                 = "#dcfce7",
        ["--accent-foreground"]      = "#052e16",
        ["--destructive"]            = "#dc2626",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "#86efac",
        ["--input"]                  = "#86efac",
        ["--ring"]                   = "#16a34a",
    };
}
