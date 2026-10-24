namespace LowlandTech.Components.Tests.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// A theme that breaks every rule a theme binding has to satisfy: it has no
/// name, no style family and a class slot with a merge mode that does not
/// exist.
/// </summary>
public sealed class FakeInvalidTheme : IComponentTheme, IComponentClassTheme, IThemeStyleFamily
{
    /// <summary>The theme has no name.</summary>
    public string Name => string.Empty;

    /// <summary>The theme has no tokens.</summary>
    public IReadOnlyDictionary<string, string> Tokens { get; } =
        new Dictionary<string, string>();

    /// <summary>The class override of the theme uses a merge mode that does not exist.</summary>
    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new ComponentSlotKey("Button", "Root")] = new("invalid-class", (ThemeClassMergeMode)99),
        };

    /// <summary>The style family of the theme is empty.</summary>
    public string StyleFamily => string.Empty;
}
