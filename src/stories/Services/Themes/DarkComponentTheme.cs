namespace LowlandTech.Components.Stories.Services.Themes;

/// <summary>Dark theme — maps to the .dark variables in variables.css.</summary>
public sealed class DarkComponentTheme : IComponentTheme
{
    public string Name => "Dark";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#1a1a27",
        ["--foreground"]             = "#f4f5ff",
        ["--card"]                   = "#1f1f2e",
        ["--card-foreground"]        = "#f4f5ff",
        ["--popover"]                = "#1f1f2e",
        ["--popover-foreground"]     = "#f4f5ff",
        ["--primary"]                = "#7e6fff",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "#f66426",
        ["--secondary-foreground"]   = "#1a1a27",
        ["--muted"]                  = "#2a2a3a",
        ["--muted-foreground"]       = "#a0a3b8",
        ["--accent"]                 = "#2f2a4f",
        ["--accent-foreground"]      = "#f4f5ff",
        ["--destructive"]            = "#ff5252",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "#2f2a4f",
        ["--input"]                  = "#2f2a4f",
        ["--ring"]                   = "#7e6fff",
    };
}
