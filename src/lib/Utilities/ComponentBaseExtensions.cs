using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Utilities;

/// <summary>
/// Extension methods for ComponentBase to provide common helper functionality
/// </summary>
public static class ComponentBaseExtensions
{
    /// <summary>
    /// Merges multiple attribute dictionaries, with later dictionaries overriding earlier ones
    /// </summary>
    /// <param name="component">The component (not used, just for extension method syntax)</param>
    /// <param name="attributes">The attribute dictionaries to merge</param>
    /// <returns>A new dictionary with merged attributes</returns>
    public static IReadOnlyDictionary<string, object>? MergeAttributes(
        this ComponentBase component,
        params IReadOnlyDictionary<string, object>?[] attributes)
    {
        if (attributes == null || attributes.Length == 0)
            return null;

        var merged = new Dictionary<string, object>();

        foreach (var dict in attributes)
        {
            if (dict == null)
                continue;

            foreach (var kvp in dict)
            {
                merged[kvp.Key] = kvp.Value;
            }
        }

        return merged.Count > 0 ? merged : null;
    }

    /// <summary>
    /// Creates or updates a data-* attribute in an attribute dictionary
    /// </summary>
    /// <param name="component">The component (not used, just for extension method syntax)</param>
    /// <param name="attributes">The attribute dictionary (can be null)</param>
    /// <param name="name">The data attribute name (without 'data-' prefix)</param>
    /// <param name="value">The attribute value</param>
    /// <returns>A new dictionary with the data attribute added</returns>
    public static IReadOnlyDictionary<string, object> SetDataAttribute(
        this ComponentBase component,
        IReadOnlyDictionary<string, object>? attributes,
        string name,
        object value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Data attribute name cannot be null or empty", nameof(name));

        var dict = attributes != null
            ? new Dictionary<string, object>(attributes)
            : new Dictionary<string, object>();

        var key = name.StartsWith("data-") ? name : $"data-{name}";
        dict[key] = value;

        return dict;
    }

    /// <summary>
    /// Builds a class string from a base class and optional additional classes
    /// Uses CssClassUtils.Merge for proper Tailwind class merging
    /// </summary>
    /// <param name="component">The component (not used, just for extension method syntax)</param>
    /// <param name="baseClass">The base CSS classes</param>
    /// <param name="additionalClasses">Optional additional CSS classes to merge</param>
    /// <returns>A merged class string</returns>
    public static string? BuildClassString(
        this ComponentBase component,
        string? baseClass,
        params string?[] additionalClasses)
    {
        var allClasses = new[] { baseClass }.Concat(additionalClasses).ToArray();
        return CssClassUtils.Merge(allClasses);
    }

    /// <summary>
    /// Builds a class string from a base class and optional additional classes from attributes
    /// Extracts "class" attribute from attributes dictionary and merges with base
    /// </summary>
    /// <param name="component">The component (not used, just for extension method syntax)</param>
    /// <param name="baseClass">The base CSS classes</param>
    /// <param name="attributes">The attribute dictionary that may contain a "class" key</param>
    /// <returns>A merged class string</returns>
    public static string? BuildClassString(
        this ComponentBase component,
        string? baseClass,
        IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes == null || !attributes.ContainsKey("class"))
            return baseClass;

        var additionalClass = attributes["class"]?.ToString();
        return CssClassUtils.Merge(baseClass, additionalClass);
    }
}
