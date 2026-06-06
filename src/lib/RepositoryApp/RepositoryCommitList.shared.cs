namespace LowlandTech.Components;

public partial class RepositoryCommitList
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    [Parameter] public IReadOnlyList<RepositoryCommitItem> Commits { get; set; } = [];

    private string GetBranchLabel() => Context?.State.Ui.SelectedBranch ?? "main";

    private string FormatRelative(DateTimeOffset when)
    {
        var delta = DateTimeOffset.Now - when;
        if (delta.TotalSeconds < 60) return "just now";
        if (delta.TotalMinutes < 60) return $"{(int)delta.TotalMinutes} minutes ago";
        if (delta.TotalHours < 24) return $"{(int)delta.TotalHours} hours ago";
        if (delta.TotalDays < 30) return $"{(int)delta.TotalDays} days ago";
        if (delta.TotalDays < 365) return $"{(int)(delta.TotalDays / 30)} months ago";
        return $"{(int)(delta.TotalDays / 365)} years ago";
    }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex h-full min-h-0 w-full flex-col overflow-hidden"
        : $"flex h-full min-h-0 w-full flex-col overflow-hidden {Class}";
}
