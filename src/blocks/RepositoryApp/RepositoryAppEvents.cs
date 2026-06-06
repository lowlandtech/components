namespace LowlandTech.Components;

public abstract record RepositoryUiEvent : IRepositoryEvent;

public sealed record RepositorySelected(string RepositoryId) : RepositoryUiEvent;

public sealed record RepositoryViewChanged(RepositoryView View) : RepositoryUiEvent;

public sealed record RepositoryCodeViewChanged(RepositoryCodeView CodeView) : RepositoryUiEvent;

public sealed record RepositoryBranchChanged(string Branch) : RepositoryUiEvent;

public sealed record RepositoryPathChanged(string Path) : RepositoryUiEvent;

public sealed record RepositoryFileSelected(string Path) : RepositoryUiEvent;

public sealed record RepositoryFilterChanged(string FilterText) : RepositoryUiEvent;

public sealed record RepositoryPageChanged(int PageNumber, int PageSize) : RepositoryUiEvent;

public sealed record RepositorySortChanged(RepositorySortColumn Column, bool Ascending) : RepositoryUiEvent;

public sealed record RepositoryFileTreeToggled : RepositoryUiEvent;

public sealed record RepositoryClonePanelToggled : RepositoryUiEvent;

public sealed record RepositoryCloneCommand(string Protocol) : RepositoryUiEvent;

public sealed record RepositoryWatchToggled : RepositoryUiEvent;

public sealed record RepositoryStarToggled : RepositoryUiEvent;

public sealed record RepositoryForkRequested : RepositoryUiEvent;
