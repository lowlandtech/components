namespace LowlandTech.Components.ToggleGroup;

public record ToggleGroupContext(string Variant, string Size, int Spacing, string Orientation);

public partial class ToggleGroup
{
    [Parameter] public int spacing { get; set; } = 0;

    private string VariantValue => string.IsNullOrWhiteSpace(variant) ? "default" : variant!;
    private string SizeValue => string.IsNullOrWhiteSpace(size) ? "default" : size!;
    private string OrientationValue => string.IsNullOrWhiteSpace(orientation) ? "horizontal" : orientation!;

    private ToggleGroupContext GroupContext => new(VariantValue, SizeValue, spacing, OrientationValue);

    private string GroupStyle => spacing > 0 ? $"--gap:{spacing}" : string.Empty;

    private string CssClass
    {
        get
        {
            const string baseClass =
                "cn-toggle-group group/toggle-group flex w-fit items-center " +
                "data-[orientation=vertical]:flex-col data-[orientation=vertical]:items-stretch";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
