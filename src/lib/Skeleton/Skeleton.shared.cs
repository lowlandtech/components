namespace LowlandTech.Components.Skeleton;

public partial class Skeleton
{
    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? "inline-flex bg-muted rounded-md animate-pulse"
        : $"inline-flex bg-muted rounded-md animate-pulse {Class}";
}
