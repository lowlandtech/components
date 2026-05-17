namespace LowlandTech.Components.App.Showcode;

public static partial class StorySourceRegistry
{
    public static StoryCard.CodeTab CodeTab(
        string value,
        string name,
        string sourceKey,
        string language = "razor")
    {
        return new StoryCard.CodeTab(value, name, GetCode(sourceKey), language);
    }

    public static string GetCode(string sourceKey)
    {
        return Sources.TryGetValue(sourceKey, out var code)
            ? NormalizeLineEndings(code)
            : $"// Story source '{sourceKey}' was not generated.";
    }

    private static string NormalizeLineEndings(string value) =>
        value.Replace("\r\n", "\n").TrimEnd();
}
