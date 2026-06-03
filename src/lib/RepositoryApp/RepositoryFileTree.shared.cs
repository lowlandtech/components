namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryFileTree
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    [Parameter] public IReadOnlyList<RepositoryFileItem> Files { get; set; } = [];

    [Parameter] public EventCallback<string> OnPathChanged { get; set; }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex h-full min-h-0 w-full flex-col overflow-hidden bg-muted/10"
        : $"flex h-full min-h-0 w-full flex-col overflow-hidden bg-muted/10 {Class}";

    private string GetRowClass(bool isActive, int depth) => isActive
        ? $"flex w-full items-center gap-1.5 rounded-md py-1 pr-2 text-left text-sm font-medium text-primary bg-primary/10 transition-colors"
        : $"flex w-full items-center gap-1.5 rounded-md py-1 pr-2 text-left text-sm font-normal text-foreground/80 hover:bg-muted/60 hover:text-foreground transition-colors";

    private string GetFileIcon(RepositoryFileKind kind) => kind switch
    {
        RepositoryFileKind.Directory => "folder",
        RepositoryFileKind.Symlink => "link",
        RepositoryFileKind.Submodule => "git-fork",
        _ => "file"
    };
}
