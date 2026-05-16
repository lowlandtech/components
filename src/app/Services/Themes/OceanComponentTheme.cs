namespace LowlandTech.Components.App.Services.Themes;

/// <summary>Ocean theme — deep sky-blue palette.</summary>
public sealed class OceanComponentTheme : IComponentTheme
{
    public string Name => "Ocean";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]             = "#f0f9ff",
        ["--foreground"]             = "#082f49",
        ["--card"]                   = "#e0f2fe",
        ["--card-foreground"]        = "#082f49",
        ["--popover"]                = "#e0f2fe",
        ["--popover-foreground"]     = "#082f49",
        ["--primary"]                = "#0284c7",
        ["--primary-foreground"]     = "#ffffff",
        ["--secondary"]              = "#0ea5e9",
        ["--secondary-foreground"]   = "#ffffff",
        ["--muted"]                  = "#bae6fd",
        ["--muted-foreground"]       = "#0369a1",
        ["--accent"]                 = "#e0f2fe",
        ["--accent-foreground"]      = "#082f49",
        ["--destructive"]            = "#dc2626",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]                 = "#7dd3fc",
        ["--input"]                  = "#7dd3fc",
        ["--ring"]                   = "#0284c7",
    };
}
