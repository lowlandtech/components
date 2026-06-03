namespace LowlandTech.Components.RepositoryApp;

public static class RepositoryUseCaseData
{
    public static readonly DateTimeOffset Now = new(2026, 06, 01, 12, 00, 00, TimeSpan.Zero);

    public static RepositoryAppState Primary()
    {
        var repo = BuildRepository();
        var treeFiles = BuildTreeFiles();
        var files = BuildRootFiles();
        var branches = BuildBranches();
        var tags = BuildTags();
        var commits = BuildCommits();
        var releases = BuildReleases();
        var metrics = BuildMetrics(repo);

        return new RepositoryAppState
        {
            Current = repo,
            Files = files,
            TreeFiles = treeFiles,
            Commits = commits,
            Branches = branches,
            Tags = tags,
            Releases = releases,
            Repositories = BuildAllRepositories(),
            Metrics = metrics,
            LatestCommit = commits[0],
            CloneInfo = BuildCloneInfo(repo),
            IsReadOnly = false,
            Ui = RepositoryUiDefaults.Empty with
            {
                SelectedRepositoryId = repo.Id,
                SelectedBranch = repo.DefaultBranch,
                ActiveView = RepositoryView.Code,
                ActiveCodeView = RepositoryCodeView.Files,
                ShowFileTree = true
            }
        };
    }

    public static RepositoryAppState Empty()
    {
        return new RepositoryAppState
        {
            Ui = RepositoryUiDefaults.Empty,
            Repositories = BuildAllRepositories()
        };
    }

    public static IReadOnlyList<RepositoryItem> BuildAllRepositories()
    {
        var primary = BuildRepository();
        return
        [
            primary,
            primary with
            {
                Id = "lowlandtech/work",
                Name = "work",
                FullName = "lowlandtech/work",
                Description = "Background job processing for the Foundry platform.",
                OpenIssues = 4,
                OpenPulls = 1,
                Stars = 18,
                Forks = 6,
                Watchers = 12
            },
            primary with
            {
                Id = "lowlandtech/reel",
                Name = "reel",
                FullName = "lowlandtech/reel",
                Description = "Video generation pipeline for marketing content.",
                OpenIssues = 0,
                OpenPulls = 0,
                Stars = 9,
                Forks = 2,
                Watchers = 5
            },
            primary with
            {
                Id = "lowlandtech/designer",
                Name = "designer",
                FullName = "lowlandtech/designer",
                Description = "Canvas designer plugin with node-based editing.",
                OpenIssues = 12,
                OpenPulls = 3,
                Stars = 24,
                Forks = 4,
                Watchers = 17
            }
        ];
    }

    public static RepositoryItem BuildRepository()
    {
        return new RepositoryItem(
            Id: "lowlandtech/foundry",
            Name: "foundry",
            FullName: "lowlandtech/foundry",
            Owner: new RepositoryOwner(
                Name: "lowlandtech",
                DisplayName: "LowlandTech",
                AvatarUrl: null,
                ProfileUrl: "/lowlandtech"),
            Description: "Modular, extensible web application platform built on .NET 10 with a graph-based entity model, Blazor UI, and plugin architecture.",
            Website: "https://lowlandtech.com",
            Visibility: RepositoryVisibility.Public,
            IsArchived: false,
            IsTemplate: false,
            IsFork: false,
            IsMirror: false,
            IsEmpty: false,
            Stars: 42,
            Forks: 9,
            Watchers: 18,
            OpenIssues: 7,
            OpenPulls: 2,
            DefaultBranch: "main",
            CurrentRef: "main",
            SizeKb: 4_096,
            CreatedAt: Now.AddDays(-540),
            UpdatedAt: Now.AddMinutes(-12),
            Topics:
            [
                new("blazor"),
                new("dotnet"),
                new("foundry"),
                new("plugin"),
                new("graph")
            ],
            Languages: BuildLanguages(),
            AvailableBranches: ["main", "develop", "feat/auth", "feat/work", "fix/clone-panel", "release/2026.05"],
            AvailableTags: ["v2026.5.29", "v2026.5.15", "v2026.4.30", "v2026.4.0", "v0.9.0-beta"]);
    }

    private static IReadOnlyList<RepositoryLanguageItem> BuildLanguages() =>
    [
        new("C#", 64.2m, "#178600"),
        new("TypeScript", 14.8m, "#3178c6"),
        new("Razor", 9.6m, "#a50e0e"),
        new("CSS", 4.2m, "#563d7c"),
        new("HTML", 2.7m, "#e34c26"),
        new("Other", 4.5m, "#94a3b8")
    ];

    private static IReadOnlyList<RepositoryFileItem> BuildTreeFiles() =>
    [
        new("agents", "agents", RepositoryFileKind.Directory, 0, null, BuildCommits()[5], Now.AddDays(-3)),
        new("components", "components", RepositoryFileKind.Directory, 0, null, BuildCommits()[2], Now.AddDays(-7)),
        new("docs", "docs", RepositoryFileKind.Directory, 0, null, BuildCommits()[3], Now.AddDays(-2)),
        new("plugins", "plugins", RepositoryFileKind.Directory, 0, null, BuildCommits()[1], Now.AddDays(-1)),
        new("samples", "samples", RepositoryFileKind.Directory, 0, null, BuildCommits()[4], Now.AddDays(-14)),
        new("src", "src", RepositoryFileKind.Directory, 0, null, BuildCommits()[0], Now.AddHours(-12)),
        new(".editorconfig", ".editorconfig", RepositoryFileKind.File, 2_188, null, BuildCommits()[6], Now.AddMonths(-6)),
        new(".gitignore", ".gitignore", RepositoryFileKind.File, 4_912, null, BuildCommits()[7], Now.AddMonths(-6)),
        new("AGENTS.md", "AGENTS.md", RepositoryFileKind.File, 28_144, null, BuildCommits()[8], Now.AddDays(-21)),
        new("foundry.slnx", "foundry.slnx", RepositoryFileKind.File, 9_312, null, BuildCommits()[9], Now.AddMonths(-3)),
        new("global.json", "global.json", RepositoryFileKind.File, 412, null, BuildCommits()[10], Now.AddMonths(-2)),
        new("LICENSE", "LICENSE", RepositoryFileKind.File, 11_204, null, BuildCommits()[11], Now.AddYears(-2)),
        new("README.md", "README.md", RepositoryFileKind.File, 8_904, null, BuildCommits()[12], Now.AddDays(-30))
    ];

    private static IReadOnlyList<RepositoryFileItem> BuildRootFiles() =>
    [
        new("agents", "agents", RepositoryFileKind.Directory, 0, null, BuildCommits()[5], Now.AddDays(-3)),
        new("components", "components", RepositoryFileKind.Directory, 0, null, BuildCommits()[2], Now.AddDays(-7)),
        new("docs", "docs", RepositoryFileKind.Directory, 0, null, BuildCommits()[3], Now.AddDays(-2)),
        new("plugins", "plugins", RepositoryFileKind.Directory, 0, null, BuildCommits()[1], Now.AddDays(-1)),
        new("samples", "samples", RepositoryFileKind.Directory, 0, null, BuildCommits()[4], Now.AddDays(-14)),
        new("src", "src", RepositoryFileKind.Directory, 0, null, BuildCommits()[0], Now.AddHours(-12)),
        new(".editorconfig", ".editorconfig", RepositoryFileKind.File, 2_188, null, BuildCommits()[6], Now.AddMonths(-6)),
        new(".gitignore", ".gitignore", RepositoryFileKind.File, 4_912, null, BuildCommits()[7], Now.AddMonths(-6)),
        new("AGENTS.md", "AGENTS.md", RepositoryFileKind.File, 28_144, null, BuildCommits()[8], Now.AddDays(-21)),
        new("foundry.slnx", "foundry.slnx", RepositoryFileKind.File, 9_312, null, BuildCommits()[9], Now.AddMonths(-3)),
        new("global.json", "global.json", RepositoryFileKind.File, 412, null, BuildCommits()[10], Now.AddMonths(-2)),
        new("LICENSE", "LICENSE", RepositoryFileKind.File, 11_204, null, BuildCommits()[11], Now.AddYears(-2)),
        new("README.md", "README.md", RepositoryFileKind.File, 8_904, null, BuildCommits()[12], Now.AddDays(-30))
    ];

    private static IReadOnlyList<RepositoryBranchItem> BuildBranches() =>
    [
        new("main", true, true, "f8c1d2a", Now.AddMinutes(-12), 0, 0),
        new("develop", false, true, "3a4b5c6", Now.AddHours(-2), 4, 1),
        new("feat/auth", false, false, "9d8e7f6", Now.AddHours(-6), 12, 0),
        new("feat/work", false, false, "0a1b2c3", Now.AddHours(-9), 7, 0),
        new("fix/clone-panel", false, false, "4d5e6f7", Now.AddDays(-1), 1, 0),
        new("release/2026.05", false, true, "8a9b0c1", Now.AddDays(-14), 0, 0)
    ];

    private static IReadOnlyList<RepositoryTagItem> BuildTags() =>
    [
        new("v2026.5.29", "f8c1d2a", Now.AddDays(-3)),
        new("v2026.5.15", "a1b2c3d", Now.AddDays(-17)),
        new("v2026.4.30", "4e5f6a7", Now.AddDays(-32)),
        new("v2026.4.0", "0a1b2c3", Now.AddMonths(-2)),
        new("v0.9.0-beta", "9d8e7f6", Now.AddMonths(-4))
    ];

    private static IReadOnlyList<RepositoryCommitItem> BuildCommits() =>
    [
        new("f8c1d2a4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0", "f8c1d2a",
            "feat(repo): add toolbar icons for archive / private / template badges",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddMinutes(-12)),
        new("3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a2b", "3a4b5c6",
            "feat(plugins): wire up auth plugin into the api startup pipeline",
            new RepositoryCommitAuthor("Foundry Bot", "bot@lowlandtech.local", null),
            Now.AddHours(-1)),
        new("9d8e7f6a5b4c3d2e1f0a9b8c7d6e5f4a3b2c1d0e", "9d8e7f6",
            "fix(canvas): respect persistence key when reloading surface instances",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddHours(-3)),
        new("0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b", "0a1b2c3",
            "docs(work): refresh the state machine diagram for the work app",
            new RepositoryCommitAuthor("Claude Opus", "claude@lowlandtech.local", null),
            Now.AddHours(-5)),
        new("4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2d3e", "4d5e6f7",
            "chore(deps): bump Tailwind to 4.0 and refresh npm lockfile",
            new RepositoryCommitAuthor("GitHub Actions", "actions@lowlandtech.local", null),
            Now.AddHours(-9)),
        new("8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b", "8a9b0c1",
            "feat(agents): add multi-agent orchestration via opencode sessions",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddHours(-12)),
        new("1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b", "1a2b3c4",
            "style: align editorconfig with .NET 10 conventions",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddDays(-3)),
        new("5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c", "5b6c7d8",
            "chore: ignore bin/obj artifacts at the repo level",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddDays(-7)),
        new("9c0d1e2f3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d", "9c0d1e2",
            "docs: add AGENTS.md with the central ticket assignment rules",
            new RepositoryCommitAuthor("Claude Opus", "claude@lowlandtech.local", null),
            Now.AddDays(-21)),
        new("2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f", "2e3f4a5",
            "chore: switch solution to the new .slnx format",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddMonths(-3)),
        new("6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5e", "6d7e8f9",
            "chore: pin global.json to the 10.0.300 SDK with rollForward",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddMonths(-2)),
        new("0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d", "0c1d2e3",
            "chore: relicense under MIT",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddYears(-2)),
        new("1e2f3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f", "1e2f3a4",
            "docs: draft the README with the platform overview",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddDays(-30)),
        new("2f3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a", "2f3a4b5",
            "feat(api): add MCP tool discovery via [McpTool] attributes",
            new RepositoryCommitAuthor("Wendell de Moor", "wendell@lowlandtech.local", null),
            Now.AddHours(-18)),
        new("3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a2b", "3a4b5c6",
            "test(work): add scenario coverage for retry + delete commands",
            new RepositoryCommitAuthor("Claude Sonnet", "claude-sonnet@lowlandtech.local", null),
            Now.AddHours(-22))
    ];

    private static IReadOnlyList<RepositoryReleaseItem> BuildReleases() =>
    [
        new("Foundry 2026.5.29", "v2026.5.29", false, false, Now.AddDays(-3), "/lowlandtech/foundry/releases/tag/v2026.5.29"),
        new("Foundry 2026.5.15", "v2026.5.15", false, false, Now.AddDays(-17), "/lowlandtech/foundry/releases/tag/v2026.5.15"),
        new("Foundry 2026.4.30", "v2026.4.30", false, false, Now.AddDays(-32), "/lowlandtech/foundry/releases/tag/v2026.4.30"),
        new("Foundry 2026.4.0", "v2026.4.0", false, false, Now.AddMonths(-2), "/lowlandtech/foundry/releases/tag/v2026.4.0")
    ];

    private static IReadOnlyList<RepositoryMetricItem> BuildMetrics(RepositoryItem repo) =>
    [
        new("Commits", "1,284", null, RepositoryMetricStyle.Default),
        new("Branches", repo.AvailableBranches.Count.ToString(), null, RepositoryMetricStyle.Default),
        new("Tags", repo.AvailableTags.Count.ToString(), null, RepositoryMetricStyle.Default),
        new("Contributors", "8", null, RepositoryMetricStyle.Default),
        new("Releases", "4", null, RepositoryMetricStyle.Default)
    ];

    private static RepositoryCloneInfo BuildCloneInfo(RepositoryItem repo) => new(
        Https: $"https://gitea.lowlandtech.com/{repo.FullName}.git",
        Ssh: $"git@gitea.lowlandtech.com:{repo.FullName}.git",
        Tea: $"tea clone {repo.FullName}");
}
