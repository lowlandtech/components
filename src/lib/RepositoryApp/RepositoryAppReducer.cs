namespace LowlandTech.Components.RepositoryApp;

public static class RepositoryAppReducer
{
    public static RepositoryAppState Reduce(RepositoryAppState state, object action)
    {
        return action switch
        {
            RepositoryUiAction uiAction => state with { Ui = RepositoryUiReducer.Reduce(state.Ui, uiAction) },
            SetRepositories set => state with { Repositories = set.Repositories },
            SetCurrentRepository current => state with { Current = current.Current },
            SetRepositoryFiles files => state with { Files = files.Files },
            SetRepositoryTreeFiles files => state with { TreeFiles = files.Files },
            SetRepositoryCommits commits => state with { Commits = commits.Commits },
            SetRepositoryBranches branches => state with { Branches = branches.Branches },
            SetRepositoryTags tags => state with { Tags = tags.Tags },
            SetRepositoryReleases releases => state with { Releases = releases.Releases },
            SetRepositoryMetrics metrics => state with { Metrics = metrics.Metrics },
            SetRepositoryLatestCommit commit => state with { LatestCommit = commit.Commit },
            SetRepositoryCloneInfo clone => state with { CloneInfo = clone.Info },
            SetRepositoryReadOnly readOnly => state with { IsReadOnly = readOnly.IsReadOnly },
            SetRepositoryPollingError error => state with { LastPollingError = error.Error },
            _ => state
        };
    }
}
