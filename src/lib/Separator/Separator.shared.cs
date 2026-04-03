namespace LowlandTech.Components.Separator;

public partial class Separator
{
    private string OrientationValue => string.Equals(orientation, "vertical", StringComparison.OrdinalIgnoreCase)
        ? "vertical"
        : "horizontal";

    private string CssClass
    {
        get
        {
            var baseClass = OrientationValue == "vertical"
                ? "shrink-0 bg-border w-px self-stretch"
                : "shrink-0 bg-border h-px w-full";

            return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}