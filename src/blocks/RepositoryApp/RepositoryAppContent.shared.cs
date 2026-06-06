namespace LowlandTech.Components;

public partial class RepositoryAppContent
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "min-h-0 flex-1 overflow-auto bg-background"
        : $"min-h-0 flex-1 overflow-auto bg-background {Class}";

    private string GetCodeLayoutClass(bool showTree, bool showRight)
    {
        var gridBase = "grid h-full min-h-0 grid-rows-1";
        if (showTree && showRight)
        {
            return $"repository-code-layout repository-code-layout--tree repository-code-layout--sidebar {gridBase}";
        }
        if (showTree)
        {
            return $"repository-code-layout repository-code-layout--tree {gridBase}";
        }
        if (showRight)
        {
            return $"repository-code-layout repository-code-layout--sidebar {gridBase}";
        }
        return "flex h-full min-h-0 flex-col";
    }
}
