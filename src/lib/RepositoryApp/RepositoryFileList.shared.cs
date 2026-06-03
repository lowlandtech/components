namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryFileList
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    [Parameter] public IReadOnlyList<RepositoryFileItem> Files { get; set; } = [];

    [Parameter] public EventCallback<RepositoryFileItem> OnFileSelected { get; set; }

    private IReadOnlyList<RepositoryFileItem> GetPagedFiles() =>
        Context?.GetPagedFiles() ?? [];

    private IReadOnlyList<RepositoryBreadcrumbCrumb> GetBreadcrumbs() =>
        Context?.GetPathBreadcrumbs() ?? [];

    private string GetBranchLabel() => Context?.State.Ui.SelectedBranch ?? "main";

    private bool ToggleAscending(RepositorySortColumn column)
    {
        if (Context?.State.Ui.SortColumn != column) return true;
        return !Context.State.Ui.SortAscending;
    }

    private string GetFileIcon(RepositoryFileKind kind) => kind switch
    {
        RepositoryFileKind.Directory => "folder",
        RepositoryFileKind.Symlink => "link",
        RepositoryFileKind.Submodule => "git-fork",
        _ => "file"
    };

    private string FormatSize(long bytes) => bytes switch
    {
        0 => "—",
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024.0):F1} MB"
    };

    private string FormatRelative(DateTimeOffset? when) => when is null
        ? "—"
        : FormatRelativeCore(when.Value, DateTimeOffset.Now);

    private static string FormatRelativeCore(DateTimeOffset when, DateTimeOffset now)
    {
        var delta = now - when;
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
