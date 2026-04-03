namespace LowlandTech.Components.Spinner;

public partial class Spinner
{
    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? "inline-flex size-4 animate-spin items-center justify-center"
        : $"inline-flex size-4 animate-spin items-center justify-center {Class}";
}