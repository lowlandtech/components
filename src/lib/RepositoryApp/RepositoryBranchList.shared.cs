namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryBranchList
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    [Parameter] public IReadOnlyList<RepositoryBranchItem> Branches { get; set; } = [];

    private string FormatRelative(DateTimeOffset? when)
    {
        if (when is null) return "—";
        var delta = DateTimeOffset.Now - when.Value;
        if (delta.TotalSeconds < 60) return "just now";
        if (delta.TotalMinutes < 60) return $"{(int)delta.TotalMinutes}m ago";
        if (delta.TotalHours < 24) return $"{(int)delta.TotalHours}h ago";
        if (delta.TotalDays < 30) return $"{(int)delta.TotalDays}d ago";
        if (delta.TotalDays < 365) return $"{(int)(delta.TotalDays / 30)}mo ago";
        return $"{(int)(delta.TotalDays / 365)}y ago";
    }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex h-full min-h-0 w-full flex-col overflow-hidden"
        : $"flex h-full min-h-0 w-full flex-col overflow-hidden {Class}";
}
