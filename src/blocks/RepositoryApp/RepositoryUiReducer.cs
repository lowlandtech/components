namespace LowlandTech.Components;

public static class RepositoryUiReducer
{
    public static RepositoryUiState Reduce(RepositoryUiState state, RepositoryUiAction action) => action switch
    {
        SelectRepository select => state with
        {
            SelectedRepositoryId = select.RepositoryId,
            PageNumber = 1,
            FilterText = string.Empty,
            CurrentPath = string.Empty,
            SelectedFilePaths = []
        },
        SetRepositoryView view => state with
        {
            ActiveView = view.View,
            PageNumber = 1,
            SelectedFilePaths = []
        },
        SetRepositoryCodeView code => state with
        {
            ActiveCodeView = code.CodeView,
            PageNumber = 1,
            FilterText = string.Empty
        },
        SetRepositoryBranch branch => state with
        {
            SelectedBranch = branch.Branch,
            CurrentPath = string.Empty,
            PageNumber = 1
        },
        SetRepositoryPath path => state with
        {
            CurrentPath = path.Path,
            PageNumber = 1
        },
        SetRepositoryFilter filter => state with
        {
            FilterText = filter.FilterText,
            PageNumber = 1
        },
        SetRepositoryPage page => state with { PageNumber = page.PageNumber },
        SetRepositoryPageSize size => state with { PageSize = size.PageSize, PageNumber = 1 },
        ToggleRepositoryFileSelection toggle => state with
        {
            SelectedFilePaths = ToggleSelection(state.SelectedFilePaths, toggle.Path)
        },
        ClearRepositorySelection => state with { SelectedFilePaths = [] },
        SetRepositorySort sort => state with
        {
            SortColumn = sort.Column,
            SortAscending = sort.Ascending
        },
        SetRepositoryLoading loading => state with { IsLoading = loading.IsLoading },
        ToggleRepositoryFileTree => state with { ShowFileTree = !state.ShowFileTree },
        ToggleRepositoryClonePanel => state with { ShowClonePanel = !state.ShowClonePanel },
        _ => state
    };

    private static IReadOnlyList<string> ToggleSelection(IReadOnlyList<string> selected, string path)
    {
        var next = selected.ToList();
        if (next.Contains(path))
            next.Remove(path);
        else
            next.Add(path);
        return next;
    }
}
