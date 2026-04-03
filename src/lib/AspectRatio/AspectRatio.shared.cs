namespace LowlandTech.Components.AspectRatio;

public partial class AspectRatio
{
    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? "relative aspect-(--ratio)"
        : $"relative aspect-(--ratio) {Class}";

    private string Style => CombineStyles($"--ratio: {GetRatioValue()}");

    private IReadOnlyDictionary<string, object>? InputAttributes => RemoveAttribute("style");

    private string GetRatioValue() => string.IsNullOrWhiteSpace(ratio) ? "1 / 1" : ratio!;

    private string CombineStyles(string baseStyle)
    {
        var styles = new List<string> { baseStyle.Trim().TrimEnd(';') };

        if (AdditionalAttributes?.TryGetValue("style", out var styleValue) == true)
        {
            var style = Convert.ToString(styleValue);
            if (!string.IsNullOrWhiteSpace(style))
            {
                styles.Add(style.Trim().TrimEnd(';'));
            }
        }

        return string.Join("; ", styles) + ";";
    }

    private IReadOnlyDictionary<string, object>? RemoveAttribute(string attributeName)
    {
        if (AdditionalAttributes is null || !AdditionalAttributes.ContainsKey(attributeName))
        {
            return AdditionalAttributes;
        }

        return AdditionalAttributes
            .Where(entry => !string.Equals(entry.Key, attributeName, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(entry => entry.Key, entry => entry.Value);
    }
}