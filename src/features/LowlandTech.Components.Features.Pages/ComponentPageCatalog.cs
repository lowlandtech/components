using LowlandTech.Plugins;

namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// The pages of the active owners, keyed the way the Plugins route gateway
/// keys them.
/// </summary>
/// <remarks>
/// The metadata value is the Plugins owned
/// <see cref="PluginPageMetadata"/>, so the Plugins repository keeps owning
/// the shape of a page description and Components does not duplicate it. A
/// host hands the entries to the Plugins page catalog unchanged.
/// </remarks>
public sealed class ComponentPageCatalog
{
    /// <summary>
    /// The published pages, keyed by owner key and symbolic identity.
    /// </summary>
    public IReadOnlyDictionary<(string PluginKey, string Component), PluginPageMetadata> Entries { get; }

    /// <summary>
    /// Creates a catalog over the given pages.
    /// </summary>
    public ComponentPageCatalog(
        IEnumerable<KeyValuePair<(string PluginKey, string Component), PluginPageMetadata>> pages)
    {
        ArgumentNullException.ThrowIfNull(pages);
        Entries = pages.ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
