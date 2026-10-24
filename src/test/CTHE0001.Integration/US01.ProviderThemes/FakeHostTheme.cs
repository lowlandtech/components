namespace LowlandTech.Components.Tests.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// The theme the host registers itself, beside the themes a plugin owner
/// contributes.
///
/// <para>
/// The host theme is registered first, so the resolver of the host falls back
/// to it when a provider selects no theme, exactly as a host does today.
/// </para>
/// </summary>
public sealed class FakeHostTheme : IComponentTheme, IComponentClassTheme, IThemeStyleFamily
{
    /// <summary>The name the host selects this theme by.</summary>
    public const string ThemeName = "host-default";

    /// <summary>The name the host selects this theme by.</summary>
    public string Name => ThemeName;

    /// <summary>The token overrides of the host theme.</summary>
    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--brand"] = "#111827",
    };

    /// <summary>The class overrides of the host theme.</summary>
    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new ComponentSlotKey("Button", "Root")] = new("host-class", ThemeClassMergeMode.Append),
        };

    /// <summary>The style family of the host theme.</summary>
    public string StyleFamily => "default";
}
