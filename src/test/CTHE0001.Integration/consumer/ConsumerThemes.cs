using LowlandTech.Components;

namespace LowlandTech.ThemeConsumer;

/// <summary>
/// The theme of the consumer host.
/// </summary>
/// <remarks>
/// The host registers this theme first, so it is the fallback of the resolver
/// and the state a withdrawal of a plugin owner restores.
/// </remarks>
public sealed class ConsumerHostTheme : IComponentTheme, IComponentClassTheme, IThemeStyleFamily
{
    /// <summary>The name the host theme is addressed by.</summary>
    public const string ThemeName = "consumer-host";

    /// <inheritdoc />
    public string Name => ThemeName;

    /// <inheritdoc />
    public string StyleFamily => "default";

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Tokens =>
        new Dictionary<string, string> { ["--consumer-host"] = "1" };

    /// <inheritdoc />
    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots =>
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new ComponentSlotKey("Button", "Root")] = new ThemeClassSlot("host-theme-class"),
        };
}

/// <summary>
/// The theme a plugin owner contributes.
/// </summary>
public sealed class ConsumerPluginTheme : IComponentTheme, IComponentClassTheme, IThemeStyleFamily
{
    /// <summary>The name the contributed theme is addressed by.</summary>
    public const string ThemeName = "consumer-plugin";

    /// <summary>
    /// The stylesheet the owner contributes, as a local package CSS path of a
    /// package the consumer restores.
    /// </summary>
    public const string Stylesheet = "_content/LowlandTech.Components/css/variables.css";

    /// <inheritdoc />
    public string Name => ThemeName;

    /// <inheritdoc />
    public string StyleFamily => "brand";

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Tokens =>
        new Dictionary<string, string> { ["--consumer-plugin"] = "1" };

    /// <inheritdoc />
    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots =>
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new ComponentSlotKey("Button", "Root")] = new ThemeClassSlot("plugin-theme-class"),
        };
}
