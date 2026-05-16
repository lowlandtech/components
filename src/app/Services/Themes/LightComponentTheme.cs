namespace LowlandTech.Components.App.Services.Themes;

/// <summary>Light theme — maps to the :root variables in variables.css.</summary>
public sealed class LightComponentTheme : IComponentTheme
{
    public string Name => "Light";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#fafafa",
        ["--foreground"]             = "#1a1a27",
        ["--card"]                   = "#ffffff",
        ["--card-foreground"]        = "#1a1a27",
        ["--popover"]                = "#ffffff",
        ["--popover-foreground"]     = "#1a1a27",
        ["--primary"]                = "#6d5cff",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "#f66426",
        ["--secondary-foreground"]   = "#ffffff",
        ["--muted"]                  = "#f0f0f5",
        ["--muted-foreground"]       = "#64648c",
        ["--accent"]                 = "#ebebf5",
        ["--accent-foreground"]      = "#1a1a27",
        ["--destructive"]            = "#e53935",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "#e0e0e8",
        ["--input"]                  = "#e0e0e8",
        ["--ring"]                   = "#6d5cff",
    };
}
