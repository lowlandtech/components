namespace LowlandTech.Components.Tests.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// The theme a plugin owner contributes to a provider boundary.
///
/// <para>
/// The fixture uses the real theme contracts of the base package, so the
/// acceptance inventory proves the bridge against the contracts a host
/// resolves with, not against a copy of them.
/// </para>
/// </summary>
public sealed class FakePluginTheme : IComponentTheme, IComponentClassTheme, IComponentFragmentTheme, IThemeStyleFamily
{
    /// <summary>The name the host selects this theme by.</summary>
    public const string ThemeName = "plugin:dark";

    /// <summary>The name the host selects this theme by.</summary>
    public string Name => ThemeName;

    /// <summary>The token overrides of the plugin theme.</summary>
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--brand"] = "#5b21b6",
    };

    /// <summary>The class overrides of the plugin theme.</summary>
    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new ComponentSlotKey("Button", "Root")] = new("plugin-class", ThemeClassMergeMode.Replace),
        };

    /// <summary>The fragment overrides of the plugin theme.</summary>
    public IReadOnlyDictionary<ComponentSlotKey, RenderFragment<ThemeFragmentContext>> Fragments { get; } =
        new Dictionary<ComponentSlotKey, RenderFragment<ThemeFragmentContext>>
        {
            [new ComponentSlotKey("Button", "Root")] = context => builder => builder.AddContent(0, "plugin-fragment"),
        };

    /// <summary>The style family of the plugin theme.</summary>
    public string StyleFamily => "brand";
}
