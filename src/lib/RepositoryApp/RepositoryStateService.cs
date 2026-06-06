namespace LowlandTech.Components;

public sealed class RepositoryStateService
{
    private RepositoryAppState _state;
    private readonly IReadOnlyList<RepositoryItem> _allRepositories;

    public RepositoryStateService()
        : this(RepositoryUseCaseData.Primary())
    {
    }

    public RepositoryStateService(RepositoryAppState initialState)
    {
        _state = initialState;
        _allRepositories = initialState.Repositories.Count > 0
            ? initialState.Repositories
            : RepositoryUseCaseData.BuildAllRepositories();
    }

    public RepositoryAppState State => _state;

    public event Action<RepositoryAppState>? StateChanged;

    public RepositoryAppState Dispatch(object action)
    {
        var next = RepositoryAppReducer.Reduce(_state, action);
        if (EqualityComparer<RepositoryAppState>.Default.Equals(_state, next))
            return _state;

        _state = next;
        StateChanged?.Invoke(_state);
        return _state;
    }

    public void Reset(RepositoryAppState? state = null)
    {
        _state = state ?? RepositoryAppDefaults.Empty;
        StateChanged?.Invoke(_state);
    }

    public void SelectRepository(string? repositoryId)
    {
        Dispatch(new SelectRepository(repositoryId));
        if (string.IsNullOrWhiteSpace(repositoryId))
        {
            Dispatch(new SetCurrentRepository(null));
            return;
        }

        var repo = _allRepositories.FirstOrDefault(r => r.Id == repositoryId);
        if (repo is null)
        {
            Dispatch(new SetCurrentRepository(null));
            return;
        }

        Dispatch(new SetCurrentRepository(repo));

        if (string.IsNullOrWhiteSpace(_state.Ui.SelectedBranch) && !string.IsNullOrWhiteSpace(repo.DefaultBranch))
        {
            Dispatch(new SetRepositoryBranch(repo.DefaultBranch));
        }
    }

    public void SetView(RepositoryView view)
    {
        Dispatch(new SetRepositoryView(view));
        Dispatch(new SetRepositoryLoading(true));
        Dispatch(new SetRepositoryLoading(false));
    }

    public void SetCodeView(RepositoryCodeView view)
    {
        Dispatch(new SetRepositoryCodeView(view));
    }

    public void SetBranch(string branch)
    {
        if (string.IsNullOrWhiteSpace(branch))
            return;
        Dispatch(new SetRepositoryBranch(branch));
    }

    public void SetPath(string path)
    {
        Dispatch(new SetRepositoryPath(path));
    }

    public void SetFilter(string filterText)
    {
        Dispatch(new SetRepositoryFilter(filterText));
    }

    public void SetPage(int pageNumber)
    {
        Dispatch(new SetRepositoryPage(pageNumber));
    }

    public void SetPageSize(int pageSize)
    {
        Dispatch(new SetRepositoryPageSize(pageSize));
    }

    public void SetSort(RepositorySortColumn column, bool ascending)
    {
        Dispatch(new SetRepositorySort(column, ascending));
    }

    public void ToggleFileSelection(string path)
    {
        Dispatch(new ToggleRepositoryFileSelection(path));
    }

    public void ClearSelection()
    {
        Dispatch(new ClearRepositorySelection());
    }

    public void ToggleFileTree()
    {
        Dispatch(new ToggleRepositoryFileTree());
    }

    public void ToggleClonePanel()
    {
        Dispatch(new ToggleRepositoryClonePanel());
    }

    public void ToggleStar()
    {
        var current = _state.Current;
        if (current is null)
            return;
        var toggled = current with { Stars = current.Stars + (_state.IsReadOnly ? 0 : 1) };
        Dispatch(new SetCurrentRepository(toggled));
    }

    public void ToggleWatch()
    {
        var current = _state.Current;
        if (current is null)
            return;
        var toggled = current with { Watchers = current.Watchers + (_state.IsReadOnly ? 0 : 1) };
        Dispatch(new SetCurrentRepository(toggled));
    }

    public void IncrementFork()
    {
        var current = _state.Current;
        if (current is null)
            return;
        var toggled = current with { Forks = current.Forks + (_state.IsReadOnly ? 0 : 1) };
        Dispatch(new SetCurrentRepository(toggled));
    }

    public IReadOnlyList<RepositoryFileItem> GetVisibleFiles()
    {
        var files = _state.Files;
        if (!string.IsNullOrWhiteSpace(_state.Ui.FilterText))
        {
            files = files
                .Where(f => f.Name.Contains(_state.Ui.FilterText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        return ApplySort(files);
    }

    public RepositoryPager GetPager()
    {
        var total = GetVisibleFiles().Count;
        var pageCount = (int)Math.Ceiling((double)total / _state.Ui.PageSize);
        return new RepositoryPager(_state.Ui.PageNumber, _state.Ui.PageSize, total, Math.Max(1, pageCount));
    }

    public IReadOnlyList<RepositoryFileItem> GetPagedFiles()
    {
        var visible = GetVisibleFiles();
        var skip = (_state.Ui.PageNumber - 1) * _state.Ui.PageSize;
        return visible.Skip(skip).Take(_state.Ui.PageSize).ToList();
    }

    public IReadOnlyList<RepositoryBreadcrumbCrumb> GetPathBreadcrumbs()
    {
        var crumbs = new List<RepositoryBreadcrumbCrumb>
        {
            new(_state.Current?.Name ?? string.Empty, string.Empty, false)
        };

        if (string.IsNullOrWhiteSpace(_state.Ui.CurrentPath))
            return crumbs;

        var segments = _state.Ui.CurrentPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var accumulated = string.Empty;
        for (var i = 0; i < segments.Length; i++)
        {
            accumulated = i == 0 ? segments[i] : $"{accumulated}/{segments[i]}";
            var isLast = i == segments.Length - 1;
            crumbs.Add(new RepositoryBreadcrumbCrumb(segments[i], accumulated, isLast));
        }

        return crumbs;
    }

    public IReadOnlyList<RepositoryFileItem> GetRootFiles()
    {
        return _state.Files
            .Where(f => string.IsNullOrEmpty(f.Path) || !f.Path.Contains('/'))
            .ToList();
    }

    private IReadOnlyList<RepositoryFileItem> ApplySort(IReadOnlyList<RepositoryFileItem> files)
    {
        IEnumerable<RepositoryFileItem> ordered = _state.Ui.SortColumn switch
        {
            RepositorySortColumn.Name => files.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase),
            RepositorySortColumn.Size => files.OrderBy(f => f.Size),
            RepositorySortColumn.UpdatedAt => files.OrderBy(f => f.UpdatedAt ?? DateTimeOffset.MinValue),
            RepositorySortColumn.CommittedAt => files.OrderBy(f => f.LastCommit?.CommittedAt ?? DateTimeOffset.MinValue),
            _ => files.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
        };

        if (_state.Ui.SortColumn == RepositorySortColumn.Name && !_state.Ui.SortAscending)
        {
            ordered = ordered.Reverse();
        }
        else if (_state.Ui.SortAscending == false)
        {
            ordered = ordered.Reverse();
        }

        return ordered.ToList();
    }
}
