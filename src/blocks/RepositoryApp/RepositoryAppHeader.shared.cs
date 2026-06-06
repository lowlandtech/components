namespace LowlandTech.Components;

public partial class RepositoryAppHeader
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex w-full flex-col border-b border-border bg-card"
        : $"flex w-full flex-col border-b border-border bg-card {Class}";

    private string GetRepoIcon() => Context?.IsRepoMirror() == true
        ? "git-mirror"
        : Context?.IsRepoFork() == true
            ? "git-fork"
            : "repo";
}
