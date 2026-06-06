namespace LowlandTech.Components;

public static class SessionBlockSpacingOptions
{
    public const string SettingKey = "session.blocks.spacing";
    public const string DefaultValue = "on";

    public static IReadOnlyList<string> AllowedValues { get; } = ["on", "off"];

    public static string Normalize(string? value)
    {
        var trimmed = value?.Trim();
        return AllowedValues.Contains(trimmed, StringComparer.OrdinalIgnoreCase) ? trimmed!.ToLowerInvariant() : DefaultValue;
    }

    public static bool IsEnabled(string? value)
        => Normalize(value) == "on";

    public static string GetSpacingClass(string? value)
        => IsEnabled(value) ? "space-y-4" : "space-y-0";
}
