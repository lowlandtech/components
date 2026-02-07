namespace LowlandTech.Components.Utilities;

/// <summary>
/// Port of class-variance-authority (CVA) pattern from React to C#.
/// Allows defining component variants with base classes and variant-specific classes.
/// </summary>
public class VariantBuilder
{
    private readonly string _baseClasses;
    private readonly Dictionary<string, Dictionary<string, string>> _variants;
    private readonly Dictionary<string, string> _defaults;

    /// <summary>
    /// Creates a new VariantBuilder.
    /// </summary>
    /// <param name="baseClasses">Base CSS classes applied to all variants</param>
    /// <param name="variants">Dictionary of variant definitions. 
    /// Key = variant name (e.g., "variant", "size"), 
    /// Value = dictionary of option name to CSS classes</param>
    /// <param name="defaults">Default variant values. Key = variant name, Value = option name</param>
    public VariantBuilder(
        string baseClasses,
        Dictionary<string, Dictionary<string, string>>? variants = null,
        Dictionary<string, string>? defaults = null)
    {
        _baseClasses = baseClasses ?? string.Empty;
        _variants = variants ?? new Dictionary<string, Dictionary<string, string>>();
        _defaults = defaults ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Builds the final CSS class string by combining base classes with selected variants.
    /// </summary>
    /// <param name="selectedVariants">Selected variant options. Key = variant name, Value = option name</param>
    /// <param name="additionalClasses">Additional CSS classes to append</param>
    /// <returns>Merged CSS class string</returns>
    public string Build(
        Dictionary<string, string>? selectedVariants = null,
        string? additionalClasses = null)
    {
        var classes = new List<string> { _baseClasses };

        // Apply variants (use defaults if not specified)
        foreach (var (variantName, options) in _variants)
        {
            string? selectedOption = null;

            // Check if this variant was explicitly selected
            if (selectedVariants?.TryGetValue(variantName, out selectedOption) != true)
            {
                // Not selected - use default if available
                _defaults.TryGetValue(variantName, out selectedOption);
            }

            // Add the classes for this variant option
            if (selectedOption != null && options.TryGetValue(selectedOption, out var variantClasses))
            {
                classes.Add(variantClasses);
            }
        }

        // Add additional classes
        if (!string.IsNullOrWhiteSpace(additionalClasses))
        {
            classes.Add(additionalClasses);
        }

        return CssClassUtils.Merge(classes.ToArray());
    }

    /// <summary>
    /// Fluent API: Adds a variant definition.
    /// </summary>
    public VariantBuilder WithVariant(string variantName, Dictionary<string, string> options)
    {
        _variants[variantName] = options;
        return this;
    }

    /// <summary>
    /// Fluent API: Sets a default value for a variant.
    /// </summary>
    public VariantBuilder WithDefault(string variantName, string defaultOption)
    {
        _defaults[variantName] = defaultOption;
        return this;
    }
}

/// <summary>
/// Extension methods for VariantBuilder to support fluent syntax.
/// </summary>
public static class VariantBuilderExtensions
{
    /// <summary>
    /// Creates a new VariantBuilder with base classes.
    /// </summary>
    public static VariantBuilder CreateVariantBuilder(this string baseClasses)
    {
        return new VariantBuilder(baseClasses);
    }
}
