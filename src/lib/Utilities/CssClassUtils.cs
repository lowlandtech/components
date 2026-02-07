using System.Text.RegularExpressions;

namespace LowlandTech.Components.Utilities;

/// <summary>
/// Port of shadcn/ui's cn() function for merging Tailwind CSS classes with conflict resolution.
/// Uses tailwind-merge logic to handle class conflicts (later classes override earlier ones).
/// </summary>
public static class CssClassUtils
{
    // Tailwind class group patterns for conflict resolution
    private static readonly Dictionary<string, string> ConflictGroups = new()
    {
        // Padding
        { @"^p-", "padding" },
        { @"^px-", "padding-x" },
        { @"^py-", "padding-y" },
        { @"^pt-", "padding-top" },
        { @"^pb-", "padding-bottom" },
        { @"^pl-", "padding-left" },
        { @"^pr-", "padding-right" },
        { @"^ps-", "padding-start" },
        { @"^pe-", "padding-end" },
        
        // Margin
        { @"^m-", "margin" },
        { @"^mx-", "margin-x" },
        { @"^my-", "margin-y" },
        { @"^mt-", "margin-top" },
        { @"^mb-", "margin-bottom" },
        { @"^ml-", "margin-left" },
        { @"^mr-", "margin-right" },
        { @"^ms-", "margin-start" },
        { @"^me-", "margin-end" },
        
        // Width/Height
        { @"^w-", "width" },
        { @"^h-", "height" },
        { @"^min-w-", "min-width" },
        { @"^min-h-", "min-height" },
        { @"^max-w-", "max-width" },
        { @"^max-h-", "max-height" },
        
        // Text
        { @"^text-\d", "font-size" }, // text-sm, text-lg, etc.
        { @"^text-[a-z]+-\d", "text-color" }, // text-red-500, etc.
        { @"^font-", "font-weight" },
        { @"^leading-", "line-height" },
        { @"^tracking-", "letter-spacing" },
        
        // Background
        { @"^bg-[a-z]+-\d", "bg-color" },
        { @"^bg-(transparent|current|inherit)", "bg-color" },
        
        // Border
        { @"^border$", "border-width" },
        { @"^border-\d", "border-width" },
        { @"^border-[a-z]+-\d", "border-color" },
        { @"^border-t-", "border-top" },
        { @"^border-b-", "border-bottom" },
        { @"^border-l-", "border-left" },
        { @"^border-r-", "border-right" },
        { @"^rounded-", "border-radius" },
        
        // Display
        { @"^(block|inline|flex|grid|hidden)", "display" },
        
        // Position
        { @"^(static|fixed|absolute|relative|sticky)", "position" },
        { @"^(top|bottom|left|right|inset)-", "inset" },
        
        // Flex/Grid
        { @"^flex-", "flex" },
        { @"^grid-cols-", "grid-cols" },
        { @"^grid-rows-", "grid-rows" },
        { @"^gap-", "gap" },
        { @"^justify-", "justify" },
        { @"^items-", "items" },
        { @"^content-", "content" },
        
        // Shadow
        { @"^shadow-", "shadow" },
        
        // Opacity
        { @"^opacity-", "opacity" },
        
        // Z-index
        { @"^z-", "z-index" },
        
        // Overflow
        { @"^overflow-", "overflow" },
    };

    /// <summary>
    /// Merges CSS class strings with Tailwind conflict resolution.
    /// Null values are ignored. Later classes override earlier conflicting classes.
    /// </summary>
    /// <param name="classes">Class strings to merge</param>
    /// <returns>Merged class string</returns>
    public static string Merge(params string?[] classes)
    {
        if (classes == null || classes.Length == 0)
            return string.Empty;

        // Filter out nulls and split into individual classes
        var allClasses = classes
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .SelectMany(c => c!.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToList();

        if (allClasses.Count == 0)
            return string.Empty;

        // Track classes by their conflict group
        // Key: conflict group name, Value: (class, index)
        var groupedClasses = new Dictionary<string, (string className, int index)>();
        var ungroupedClasses = new Dictionary<string, int>(); // className -> first index

        for (int i = 0; i < allClasses.Count; i++)
        {
            var className = allClasses[i];
            var groupName = GetConflictGroup(className);

            if (groupName != null)
            {
                // This class belongs to a conflict group
                // Keep the latest UNLESS it's the same class (duplicate)
                if (groupedClasses.TryGetValue(groupName, out var existing))
                {
                    // Only update if it's a different class in the same group
                    if (existing.className != className)
                    {
                        groupedClasses[groupName] = (className, i);
                    }
                }
                else
                {
                    groupedClasses[groupName] = (className, i);
                }
            }
            else
            {
                // No conflict group - keep first occurrence only
                if (!ungroupedClasses.ContainsKey(className))
                {
                    ungroupedClasses[className] = i;
                }
            }
        }

        // Combine all classes maintaining order
        var result = groupedClasses.Values
            .Select(v => (className: v.className, index: v.index))
            .Concat(ungroupedClasses.Select(kv => (className: kv.Key, index: kv.Value)))
            .OrderBy(c => c.index)
            .Select(c => c.className)
            .ToList();

        return string.Join(" ", result);
    }

    /// <summary>
    /// Determines the conflict group for a class name.
    /// Returns null if the class doesn't belong to any conflict group.
    /// </summary>
    private static string? GetConflictGroup(string className)
    {
        foreach (var (pattern, group) in ConflictGroups)
        {
            if (Regex.IsMatch(className, pattern))
                return group;
        }
        return null;
    }

    /// <summary>
    /// Merges multiple class attribute dictionaries, with later dictionaries overriding earlier ones.
    /// </summary>
    public static IReadOnlyDictionary<string, object> MergeAttributes(
        params IReadOnlyDictionary<string, object>?[] attributeDictionaries)
    {
        var result = new Dictionary<string, object>();

        foreach (var dict in attributeDictionaries)
        {
            if (dict == null) continue;

            foreach (var (key, value) in dict)
            {
                if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    // Special handling for class attribute
                    var existingClass = result.TryGetValue("class", out var existing) 
                        ? existing?.ToString() 
                        : null;
                    result["class"] = Merge(existingClass, value?.ToString());
                }
                else
                {
                    result[key] = value;
                }
            }
        }

        return result;
    }
}
