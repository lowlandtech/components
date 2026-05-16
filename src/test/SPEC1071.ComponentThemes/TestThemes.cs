namespace LowlandTech.Components.Tests.SPEC1071.ComponentThemes;

// ---------------------------------------------------------------------------
// Lightweight IComponentTheme test doubles used across SPEC1071 US01–US04.
// Values mirror the real implementations so token-specific assertions remain
// meaningful (e.g. Ocean "--background" must equal "#f0f9ff").
// ---------------------------------------------------------------------------

internal sealed class TestLightTheme : IComponentTheme
{
    public string Name => "Light";
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]           = "#ffffff",
        ["--foreground"]           = "#020817",
        ["--card"]                 = "#ffffff",
        ["--card-foreground"]      = "#020817",
        ["--popover"]              = "#ffffff",
        ["--popover-foreground"]   = "#020817",
        ["--primary"]              = "#6d5cff",
        ["--primary-foreground"]   = "#ffffff",
        ["--secondary"]            = "#f1f5f9",
        ["--secondary-foreground"] = "#0f172a",
        ["--muted"]                = "#f1f5f9",
        ["--muted-foreground"]     = "#64748b",
        ["--accent"]               = "#f1f5f9",
        ["--accent-foreground"]    = "#0f172a",
        ["--destructive"]          = "#ef4444",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]               = "#e2e8f0",
        ["--input"]                = "#e2e8f0",
        ["--ring"]                 = "#6d5cff",
    };
}

internal sealed class TestDarkTheme : IComponentTheme
{
    public string Name => "Dark";
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]           = "#020817",
        ["--foreground"]           = "#f8fafc",
        ["--card"]                 = "#0f172a",
        ["--card-foreground"]      = "#f8fafc",
        ["--popover"]              = "#0f172a",
        ["--popover-foreground"]   = "#f8fafc",
        ["--primary"]              = "#7e6fff",
        ["--primary-foreground"]   = "#020817",
        ["--secondary"]            = "#1e293b",
        ["--secondary-foreground"] = "#f8fafc",
        ["--muted"]                = "#1e293b",
        ["--muted-foreground"]     = "#94a3b8",
        ["--accent"]               = "#1e293b",
        ["--accent-foreground"]    = "#f8fafc",
        ["--destructive"]          = "#7f1d1d",
        ["--destructive-foreground"] = "#fef2f2",
        ["--border"]               = "#1e293b",
        ["--input"]                = "#1e293b",
        ["--ring"]                 = "#7e6fff",
    };
}

internal sealed class TestOceanTheme : IComponentTheme
{
    public string Name => "Ocean";
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]           = "#f0f9ff",
        ["--foreground"]           = "#082f49",
        ["--card"]                 = "#e0f2fe",
        ["--card-foreground"]      = "#082f49",
        ["--popover"]              = "#f0f9ff",
        ["--popover-foreground"]   = "#082f49",
        ["--primary"]              = "#0284c7",
        ["--primary-foreground"]   = "#ffffff",
        ["--secondary"]            = "#bae6fd",
        ["--secondary-foreground"] = "#0c4a6e",
        ["--muted"]                = "#bae6fd",
        ["--muted-foreground"]     = "#0369a1",
        ["--accent"]               = "#bae6fd",
        ["--accent-foreground"]    = "#0c4a6e",
        ["--destructive"]          = "#dc2626",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]               = "#7dd3fc",
        ["--input"]                = "#7dd3fc",
        ["--ring"]                 = "#0284c7",
    };
}

internal sealed class TestRoseTheme : IComponentTheme
{
    public string Name => "Rose";
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]           = "#fff1f2",
        ["--foreground"]           = "#4c0519",
        ["--card"]                 = "#ffe4e6",
        ["--card-foreground"]      = "#4c0519",
        ["--popover"]              = "#fff1f2",
        ["--popover-foreground"]   = "#4c0519",
        ["--primary"]              = "#f43f5e",
        ["--primary-foreground"]   = "#ffffff",
        ["--secondary"]            = "#fecdd3",
        ["--secondary-foreground"] = "#881337",
        ["--muted"]                = "#fecdd3",
        ["--muted-foreground"]     = "#be123c",
        ["--accent"]               = "#fecdd3",
        ["--accent-foreground"]    = "#881337",
        ["--destructive"]          = "#dc2626",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]               = "#fda4af",
        ["--input"]                = "#fda4af",
        ["--ring"]                 = "#f43f5e",
    };
}

internal sealed class TestForestTheme : IComponentTheme
{
    public string Name => "Forest";
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"]           = "#f0fdf4",
        ["--foreground"]           = "#052e16",
        ["--card"]                 = "#dcfce7",
        ["--card-foreground"]      = "#052e16",
        ["--popover"]              = "#f0fdf4",
        ["--popover-foreground"]   = "#052e16",
        ["--primary"]              = "#16a34a",
        ["--primary-foreground"]   = "#ffffff",
        ["--secondary"]            = "#bbf7d0",
        ["--secondary-foreground"] = "#14532d",
        ["--muted"]                = "#bbf7d0",
        ["--muted-foreground"]     = "#15803d",
        ["--accent"]               = "#bbf7d0",
        ["--accent-foreground"]    = "#14532d",
        ["--destructive"]          = "#dc2626",
        ["--destructive-foreground"] = "#ffffff",
        ["--border"]               = "#86efac",
        ["--input"]                = "#86efac",
        ["--ring"]                 = "#16a34a",
    };
}

/// <summary>Factory helpers shared by all SPEC1071 tests.</summary>
internal static class TestThemeFactory
{
    internal static IComponentTheme[] AllThemes() =>
    [
        new TestLightTheme(),
        new TestDarkTheme(),
        new TestOceanTheme(),
        new TestRoseTheme(),
        new TestForestTheme(),
    ];

    internal static ComponentThemeService CreateService() =>
        new(AllThemes());
}
