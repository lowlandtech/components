namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryAppNavbar
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    private sealed record NavItem(
        RepositoryView View,
        string Label,
        string Icon,
        bool RequiresCodeAccess = true,
        bool RequiresNonEmpty = false);

    private IReadOnlyList<NavItem> BuildItems()
    {
        return
        [
            new(RepositoryView.Code,      "Code",      "code",            RequiresCodeAccess: false, RequiresNonEmpty: false),
            new(RepositoryView.Issues,     "Issues",    "issue-opened",    RequiresCodeAccess: false, RequiresNonEmpty: false),
            new(RepositoryView.Pulls,      "Pulls",     "git-pull-request", RequiresCodeAccess: false, RequiresNonEmpty: false),
            new(RepositoryView.Actions,    "Actions",   "play",            RequiresCodeAccess: false, RequiresNonEmpty: true),
            new(RepositoryView.Packages,   "Packages",  "package",         RequiresCodeAccess: false, RequiresNonEmpty: false),
            new(RepositoryView.Projects,   "Projects",  "project",         RequiresCodeAccess: false, RequiresNonEmpty: false),
            new(RepositoryView.Releases,   "Releases",  "tag",             RequiresCodeAccess: false, RequiresNonEmpty: true),
            new(RepositoryView.Wiki,       "Wiki",      "book",            RequiresCodeAccess: false, RequiresNonEmpty: false),
            new(RepositoryView.Activity,   "Activity",  "pulse",           RequiresCodeAccess: false, RequiresNonEmpty: true),
            new(RepositoryView.Settings,   "Settings",  "settings",        RequiresCodeAccess: false, RequiresNonEmpty: false)
        ];
    }

    private int? GetBadge(RepositoryView view) => Context?.State.Current switch
    {
        null => null,
        var c when view == RepositoryView.Issues && c.OpenIssues > 0 => c.OpenIssues,
        var c when view == RepositoryView.Pulls && c.OpenPulls > 0 => c.OpenPulls,
        _ => null
    };

    private Task HandleViewChangedAsync(string? value)
    {
        if (string.IsNullOrEmpty(value) || Context is null) return Task.CompletedTask;
        if (!Enum.TryParse<RepositoryView>(value, ignoreCase: true, out var view)) return Task.CompletedTask;
        Context.HandleViewChanged(new RepositoryViewChanged(view));
        return Task.CompletedTask;
    }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex w-full items-center overflow-x-auto border-b border-border bg-card px-2 py-1.5"
        : $"flex w-full items-center overflow-x-auto border-b border-border bg-card px-2 py-1.5 {Class}";
}
