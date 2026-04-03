namespace LowlandTech.Components.Skeleton;

public partial class Skeleton
{
    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? "cn-skeleton animate-pulse"
        : $"cn-skeleton animate-pulse {Class}";
}