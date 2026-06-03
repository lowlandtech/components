namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryAppSubMenu
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    private sealed record SubItem(RepositoryCodeView View, string Label, string Icon);

    private IReadOnlyList<SubItem> _items { get; } = new List<SubItem>
    {
        new(RepositoryCodeView.Files,    "Files",    "files"),
        new(RepositoryCodeView.Commits,  "Commits",  "history"),
        new(RepositoryCodeView.Branches, "Branches", "git-branch"),
        new(RepositoryCodeView.Tags,     "Tags",     "tag")
    };

    private long GetCount(RepositoryCodeView view) => view switch
    {
        RepositoryCodeView.Files => Context?.State.Files.Count ?? 0,
        RepositoryCodeView.Commits => Context?.State.Commits.Count ?? 0,
        RepositoryCodeView.Branches => Context?.State.Branches.Count ?? 0,
        RepositoryCodeView.Tags => Context?.State.Tags.Count ?? 0,
        _ => 0
    };

    private Task HandleCodeViewChangedAsync(string? value)
    {
        if (string.IsNullOrEmpty(value) || Context is null) return Task.CompletedTask;
        if (!Enum.TryParse<RepositoryCodeView>(value, ignoreCase: true, out var view)) return Task.CompletedTask;
        Context.HandleCodeViewChanged(new RepositoryCodeViewChanged(view));
        return Task.CompletedTask;
    }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex w-full items-center border-b border-border bg-card px-2 py-1.5"
        : $"flex w-full items-center border-b border-border bg-card px-2 py-1.5 {Class}";
}
