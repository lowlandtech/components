namespace LowlandTech.Components.Stories.Services.Themes;

/// <summary>Rose theme — warm rose/pink palette.</summary>
public sealed class RoseComponentTheme : IComponentTheme
{
    public string Name => "Rose";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#fff1f2",
        ["--foreground"]             = "#4c0519",
        ["--card"]                   = "#ffe4e6",
        ["--card-foreground"]        = "#4c0519",
        ["--popover"]                = "#ffe4e6",
        ["--popover-foreground"]     = "#4c0519",
        ["--primary"]                = "#f43f5e",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "#fb7185",
        ["--secondary-foreground"]   = "#ffffff",
        ["--muted"]                  = "#fecdd3",
        ["--muted-foreground"]       = "#9f1239",
        ["--accent"]                 = "#ffe4e6",
        ["--accent-foreground"]      = "#4c0519",
        ["--destructive"]            = "#be123c",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "#fda4af",
        ["--input"]                  = "#fda4af",
        ["--ring"]                   = "#f43f5e",
    };
}
