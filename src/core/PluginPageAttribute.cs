namespace LowlandTech.Core.Plugins;

/// <summary>
/// Marks a Blazor page component for automatic collection into the shell sidebar.
/// Place on any component that also has an @page directive.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PluginPageAttribute : Attribute
{
    public string Label { get; set; } = string.Empty;

    /// <summary>Logical icon name (e.g. "plugin", "table", "cog"). Mapped to Icons.* in NavMenu.</summary>
    public string Icon { get; set; } = "plugin";

    /// <summary>Sort order in the sidebar. Lower values appear first.</summary>
    public int Order { get; set; } = 100;

    public bool ShowInNav { get; set; } = true;

    /// <summary>Optional group label shown as a section heading above this item.</summary>
    public string? Group { get; set; }

    /// <summary>Optional sub-group label within <see cref="Group"/>. Items sharing the same
    /// SubGroup are rendered as a collapsible section nested under the group heading.</summary>
    public string? SubGroup { get; set; }
}
