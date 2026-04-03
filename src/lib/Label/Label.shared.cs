namespace LowlandTech.Components.Label;

public partial class Label
{
    private const string BaseClass = "cn-label flex items-center select-none group-data-[disabled=true]:pointer-events-none peer-disabled:cursor-not-allowed";

    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? BaseClass
        : $"{BaseClass} {Class}";
}