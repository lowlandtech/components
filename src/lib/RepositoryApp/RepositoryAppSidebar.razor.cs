namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryAppSidebar : TinyComponentBase
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    private RepositoryFileItem? GetSelectedFile()
    {
        if (Context is null) return null;
        var selected = Context.State.Ui.SelectedFilePaths;
        if (selected.Count == 0) return null;
        var firstPath = selected[^1];
        return Context.State.Files.FirstOrDefault(f => f.Path == firstPath)
            ?? Context.State.TreeFiles.FirstOrDefault(f => f.Path == firstPath);
    }

    private string GetAuthorInitials(RepositoryCommitAuthor author)
    {
        if (string.IsNullOrWhiteSpace(author.Name)) return "?";
        var parts = author.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();
        return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
    }

    private string GetFileIcon(RepositoryFileKind kind) => kind switch
    {
        RepositoryFileKind.Directory => "folder",
        _ => "file"
    };

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
        ? "hidden h-full min-h-0 w-full flex-col overflow-hidden border-l border-border bg-card lg:flex"
        : $"hidden h-full min-h-0 w-full flex-col overflow-hidden border-l border-border bg-card lg:flex {Class}";
}
