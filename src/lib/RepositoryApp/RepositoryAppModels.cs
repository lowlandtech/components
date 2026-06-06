namespace LowlandTech.Components;

public enum RepositoryView
{
    Code,
    Issues,
    Pulls,
    Actions,
    Packages,
    Projects,
    Releases,
    Wiki,
    Activity,
    Settings
}

public enum RepositoryCodeView
{
    Files,
    Commits,
    Branches,
    Tags
}

public enum RepositoryFileKind
{
    File,
    Directory,
    Symlink,
    Submodule
}

public enum RepositoryVisibility
{
    Public,
    Internal,
    Private
}

public enum RepositorySortColumn
{
    Name,
    Size,
    UpdatedAt,
    CommittedAt
}

public enum RepositoryMetricStyle
{
    Default,
    Success,
    Warning,
    Danger,
    Info
}

public sealed record RepositoryOwner(
    string Name,
    string DisplayName,
    string? AvatarUrl,
    string ProfileUrl);

public sealed record RepositoryLanguageItem(
    string Name,
    decimal Percentage,
    string Color);

public sealed record RepositoryTopicItem(string Name);

public sealed record RepositoryCommitAuthor(
    string Name,
    string? Email,
    string? AvatarUrl);

public sealed record RepositoryCommitItem(
    string Sha,
    string ShortSha,
    string Message,
    RepositoryCommitAuthor Author,
    DateTimeOffset CommittedAt);

public sealed record RepositoryBranchItem(
    string Name,
    bool IsDefault,
    bool IsProtected,
    string? Sha,
    DateTimeOffset? UpdatedAt = null,
    int? AheadBy = null,
    int? BehindBy = null);

public sealed record RepositoryTagItem(
    string Name,
    string ShortSha,
    DateTimeOffset? CreatedAt = null);

public sealed record RepositoryReleaseItem(
    string Title,
    string TagName,
    bool IsPreRelease,
    bool IsDraft,
    DateTimeOffset PublishedAt,
    string? Url = null);

public sealed record RepositoryFileItem(
    string Name,
    string Path,
    RepositoryFileKind Kind,
    long Size,
    string? SymlinkTarget,
    RepositoryCommitItem? LastCommit,
    DateTimeOffset? UpdatedAt = null);

public sealed record RepositoryItem(
    string Id,
    string Name,
    string FullName,
    RepositoryOwner Owner,
    string? Description,
    string? Website,
    RepositoryVisibility Visibility,
    bool IsArchived,
    bool IsTemplate,
    bool IsFork,
    bool IsMirror,
    bool IsEmpty,
    int Stars,
    int Forks,
    int Watchers,
    int OpenIssues,
    int OpenPulls,
    string DefaultBranch,
    string CurrentRef,
    long SizeKb,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<RepositoryTopicItem> Topics,
    IReadOnlyList<RepositoryLanguageItem> Languages,
    IReadOnlyList<string> AvailableBranches,
    IReadOnlyList<string> AvailableTags);

public sealed record RepositoryMetricItem(
    string Title,
    string? Value,
    string? Url = null,
    RepositoryMetricStyle Style = RepositoryMetricStyle.Default,
    bool IsHighlighted = false);

public sealed record RepositoryPager(
    int PageNumber,
    int PageSize,
    int TotalItems,
    int PageCount);

public sealed record RepositoryBreadcrumbCrumb(
    string Label,
    string? Path,
    bool IsCurrent);

public sealed record RepositoryCloneInfo(
    string Https,
    string Ssh,
    string Tea);

public sealed record RepositoryUiState(
    RepositoryView ActiveView,
    RepositoryCodeView ActiveCodeView,
    string? SelectedRepositoryId,
    string SelectedBranch,
    string CurrentPath,
    string FilterText,
    int PageNumber,
    int PageSize,
    IReadOnlyList<string> SelectedFilePaths,
    RepositorySortColumn SortColumn,
    bool SortAscending,
    bool IsLoading,
    bool ShowFileTree,
    bool ShowClonePanel);

public sealed record RepositoryAppState
{
    public RepositoryUiState Ui { get; init; } = RepositoryUiDefaults.Empty;
    public IReadOnlyList<RepositoryItem> Repositories { get; init; } = [];
    public RepositoryItem? Current { get; init; }
    public IReadOnlyList<RepositoryFileItem> Files { get; init; } = [];
    public IReadOnlyList<RepositoryFileItem> TreeFiles { get; init; } = [];
    public IReadOnlyList<RepositoryCommitItem> Commits { get; init; } = [];
    public IReadOnlyList<RepositoryBranchItem> Branches { get; init; } = [];
    public IReadOnlyList<RepositoryTagItem> Tags { get; init; } = [];
    public IReadOnlyList<RepositoryReleaseItem> Releases { get; init; } = [];
    public IReadOnlyList<RepositoryMetricItem> Metrics { get; init; } = [];
    public RepositoryCommitItem? LatestCommit { get; init; }
    public RepositoryCloneInfo? CloneInfo { get; init; }
    public bool IsReadOnly { get; init; }
    public string? LastPollingError { get; init; }
}

public static class RepositoryUiDefaults
{
    public static RepositoryUiState Empty { get; } = new(
        RepositoryView.Code,
        RepositoryCodeView.Files,
        null,
        "main",
        string.Empty,
        string.Empty,
        1,
        50,
        [],
        RepositorySortColumn.Name,
        true,
        false,
        true,
        false);
}

public static class RepositoryAppDefaults
{
    public static RepositoryAppState Empty { get; } = new();
}

public interface IRepositoryAction { }

public interface IRepositoryEvent { }

public abstract record RepositoryUiAction : IRepositoryAction;

public abstract record RepositoryAppAction : IRepositoryAction;

public sealed record SelectRepository(string? RepositoryId) : RepositoryUiAction;

public sealed record SetRepositoryView(RepositoryView View) : RepositoryUiAction;

public sealed record SetRepositoryCodeView(RepositoryCodeView CodeView) : RepositoryUiAction;

public sealed record SetRepositoryBranch(string Branch) : RepositoryUiAction;

public sealed record SetRepositoryPath(string Path) : RepositoryUiAction;

public sealed record SetRepositoryFilter(string FilterText) : RepositoryUiAction;

public sealed record SetRepositoryPage(int PageNumber) : RepositoryUiAction;

public sealed record SetRepositoryPageSize(int PageSize) : RepositoryUiAction;

public sealed record ToggleRepositoryFileSelection(string Path) : RepositoryUiAction;

public sealed record ClearRepositorySelection : RepositoryUiAction;

public sealed record SetRepositorySort(RepositorySortColumn Column, bool Ascending) : RepositoryUiAction;

public sealed record SetRepositoryLoading(bool IsLoading) : RepositoryUiAction;

public sealed record ToggleRepositoryFileTree : RepositoryUiAction;

public sealed record ToggleRepositoryClonePanel : RepositoryUiAction;

public sealed record SetRepositories(IReadOnlyList<RepositoryItem> Repositories) : IRepositoryAction;

public sealed record SetCurrentRepository(RepositoryItem? Current) : IRepositoryAction;

public sealed record SetRepositoryFiles(IReadOnlyList<RepositoryFileItem> Files) : IRepositoryAction;

public sealed record SetRepositoryTreeFiles(IReadOnlyList<RepositoryFileItem> Files) : IRepositoryAction;

public sealed record SetRepositoryCommits(IReadOnlyList<RepositoryCommitItem> Commits) : IRepositoryAction;

public sealed record SetRepositoryBranches(IReadOnlyList<RepositoryBranchItem> Branches) : IRepositoryAction;

public sealed record SetRepositoryTags(IReadOnlyList<RepositoryTagItem> Tags) : IRepositoryAction;

public sealed record SetRepositoryReleases(IReadOnlyList<RepositoryReleaseItem> Releases) : IRepositoryAction;

public sealed record SetRepositoryMetrics(IReadOnlyList<RepositoryMetricItem> Metrics) : IRepositoryAction;

public sealed record SetRepositoryLatestCommit(RepositoryCommitItem? Commit) : IRepositoryAction;

public sealed record SetRepositoryCloneInfo(RepositoryCloneInfo Info) : IRepositoryAction;

public sealed record SetRepositoryReadOnly(bool IsReadOnly) : IRepositoryAction;

public sealed record SetRepositoryPollingError(string? Error) : IRepositoryAction;
