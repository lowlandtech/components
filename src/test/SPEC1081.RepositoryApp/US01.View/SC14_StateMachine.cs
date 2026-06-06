using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "14")]
[UserStory("01", "RepositoryStateService dispatches actions and derives derived state")]
public class SC14_StateMachine : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Dispatch is a no-op when next state equals current state")]
    [Trait(Spec.UAC, "01")]
    [Given("the state has a value for Ui.IsLoading")]
    [When("the same value is dispatched again")]
    [Then("StateChanged should not fire")]
    public void UAC01_DispatchIsNoOpWhenEqual()
    {
        var fired = false;
        StateService.StateChanged += _ => fired = true;

        // Dispatching the same value the state already has is a true no-op:
        // the reducer returns the existing state and StateChanged must not fire.
        var initialIsLoading = StateService.State.Ui.IsLoading;
        StateService.Dispatch(new SetRepositoryLoading(initialIsLoading));

        fired.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: SetView updates ActiveView")]
    [Trait(Spec.UAC, "02")]
    [Given("a state service in default state")]
    [When("SetView(Pulls) is called")]
    [Then("Ui.ActiveView should equal Pulls")]
    public void UAC02_SetViewUpdatesActiveView()
    {
        StateService.SetView(RepositoryView.Pulls);

        StateService.State.Ui.ActiveView.ShouldBe(RepositoryView.Pulls);
    }

    [Fact(DisplayName = "UAC03: SetCodeView updates ActiveCodeView")]
    [Trait(Spec.UAC, "03")]
    [Given("a state service in default state")]
    [When("SetCodeView(Tags) is called")]
    [Then("Ui.ActiveCodeView should equal Tags")]
    public void UAC03_SetCodeViewUpdatesActiveCodeView()
    {
        StateService.SetCodeView(RepositoryCodeView.Tags);

        StateService.State.Ui.ActiveCodeView.ShouldBe(RepositoryCodeView.Tags);
    }

    [Fact(DisplayName = "UAC04: SetBranch ignores empty strings")]
    [Trait(Spec.UAC, "04")]
    [Given("a state service with a selected branch")]
    [When("SetBranch('') is called")]
    [Then("the selected branch should remain unchanged")]
    public void UAC04_SetBranchIgnoresEmpty()
    {
        StateService.SetBranch(string.Empty);

        StateService.State.Ui.SelectedBranch.ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "UAC05: SetPath stores the path verbatim")]
    [Trait(Spec.UAC, "05")]
    [Given("a state service in default state")]
    [When("SetPath('src/components') is called")]
    [Then("Ui.CurrentPath should equal 'src/components'")]
    public void UAC05_SetPathStoresVerbatim()
    {
        StateService.SetPath("src/components");

        StateService.State.Ui.CurrentPath.ShouldBe("src/components");
    }

    [Fact(DisplayName = "UAC06: SetFilter narrows the visible files")]
    [Trait(Spec.UAC, "06")]
    [Given("a state service with the default file set")]
    [When("SetFilter('agents') is called")]
    [Then("the visible file count should be less than before")]
    public void UAC06_SetFilterNarrowsVisibleFiles()
    {
        var before = StateService.GetVisibleFiles().Count;
        StateService.SetFilter("agents");

        var after = StateService.GetVisibleFiles().Count;
        after.ShouldBeLessThan(before);
    }

    [Fact(DisplayName = "UAC07: SetSort orders by name ascending")]
    [Trait(Spec.UAC, "07")]
    [Given("a state service in default state")]
    [When("SetSort(Name, true) is called")]
    [Then("the visible files should be ordered by name ascending")]
    public void UAC07_SetSortOrdersByName()
    {
        StateService.SetSort(RepositorySortColumn.Name, true);

        var visible = StateService.GetVisibleFiles();
        var names = visible.Select(f => f.Name).ToList();
        var sorted = names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        names.ShouldBe(sorted);
    }

    [Fact(DisplayName = "UAC08: SetSort orders by name descending when Ascending is false")]
    [Trait(Spec.UAC, "08")]
    [Given("a state service in default state")]
    [When("SetSort(Name, false) is called")]
    [Then("the visible files should be ordered by name descending")]
    public void UAC08_SetSortOrdersByNameDescending()
    {
        StateService.SetSort(RepositorySortColumn.Name, false);

        var visible = StateService.GetVisibleFiles();
        var names = visible.Select(f => f.Name).ToList();
        var sorted = names.OrderByDescending(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        names.ShouldBe(sorted);
    }

    [Fact(DisplayName = "UAC09: GetPager reports total items and page count")]
    [Trait(Spec.UAC, "09")]
    [Given("a state service with visible files and a page size")]
    [When("GetPager is called")]
    [Then("it should report total items and the current page size")]
    public void UAC09_GetPagerReportsTotals()
    {
        var pager = StateService.GetPager();

        pager.TotalItems.ShouldBe(StateService.GetVisibleFiles().Count);
        pager.PageSize.ShouldBe(StateService.State.Ui.PageSize);
    }

    [Fact(DisplayName = "UAC10: GetPagedFiles returns a page-sized slice")]
    [Trait(Spec.UAC, "10")]
    [Given("a state service with multiple files and a page size of 5")]
    [When("GetPagedFiles is called")]
    [Then("it should return at most 5 files")]
    public void UAC10_GetPagedFilesReturnsSlice()
    {
        StateService.SetPageSize(5);
        StateService.SetPage(1);

        var paged = StateService.GetPagedFiles();
        paged.Count.ShouldBeLessThanOrEqualTo(5);
    }

    [Fact(DisplayName = "UAC11: ToggleStar increments current.Stars")]
    [Trait(Spec.UAC, "11")]
    [Given("a state service with a current repository")]
    [When("ToggleStar is called")]
    [Then("Current.Stars should increase by 1")]
    public void UAC11_ToggleStarIncrementsStars()
    {
        var before = StateService.State.Current!.Stars;
        StateService.ToggleStar();

        StateService.State.Current!.Stars.ShouldBe(before + 1);
    }

    [Fact(DisplayName = "UAC12: ToggleWatch increments current.Watchers")]
    [Trait(Spec.UAC, "12")]
    [Given("a state service with a current repository")]
    [When("ToggleWatch is called")]
    [Then("Current.Watchers should increase by 1")]
    public void UAC12_ToggleWatchIncrementsWatchers()
    {
        var before = StateService.State.Current!.Watchers;
        StateService.ToggleWatch();

        StateService.State.Ui.ActiveView.ShouldBe(StateService.State.Ui.ActiveView); // sanity
        StateService.State.Current!.Watchers.ShouldBe(before + 1);
    }

    [Fact(DisplayName = "UAC13: IncrementFork increments current.Forks")]
    [Trait(Spec.UAC, "13")]
    [Given("a state service with a current repository")]
    [When("IncrementFork is called")]
    [Then("Current.Forks should increase by 1")]
    public void UAC13_IncrementForkIncrementsForks()
    {
        var before = StateService.State.Current!.Forks;
        StateService.IncrementFork();

        StateService.State.Current!.Forks.ShouldBe(before + 1);
    }

    [Fact(DisplayName = "UAC14: ToggleFileTree flips ShowFileTree")]
    [Trait(Spec.UAC, "14")]
    [Given("a state service with a known ShowFileTree value")]
    [When("ToggleFileTree is called")]
    [Then("ShowFileTree should be inverted")]
    public void UAC14_ToggleFileTreeFlips()
    {
        var start = StateService.State.Ui.ShowFileTree;
        StateService.ToggleFileTree();

        StateService.State.Ui.ShowFileTree.ShouldBe(!start);
    }

    [Fact(DisplayName = "UAC15: ToggleClonePanel flips ShowClonePanel")]
    [Trait(Spec.UAC, "15")]
    [Given("a state service with a known ShowClonePanel value")]
    [When("ToggleClonePanel is called")]
    [Then("ShowClonePanel should be inverted")]
    public void UAC15_ToggleClonePanelFlips()
    {
        var start = StateService.State.Ui.ShowClonePanel;
        StateService.ToggleClonePanel();

        StateService.State.Ui.ShowClonePanel.ShouldBe(!start);
    }

    [Fact(DisplayName = "UAC16: ToggleFileSelection adds and removes the path")]
    [Trait(Spec.UAC, "16")]
    [Given("a state service with files in the state")]
    [When("ToggleFileSelection is called twice for the same path")]
    [Then("the path should be selected then deselected")]
    public void UAC16_ToggleFileSelectionTogglesPath()
    {
        var target = StateService.State.Files.First().Path;

        StateService.ToggleFileSelection(target);
        StateService.State.Ui.SelectedFilePaths.ShouldContain(target);

        StateService.ToggleFileSelection(target);
        StateService.State.Ui.SelectedFilePaths.ShouldNotContain(target);
    }

    [Fact(DisplayName = "UAC17: ClearSelection empties SelectedFilePaths")]
    [Trait(Spec.UAC, "17")]
    [Given("a state service with two files selected")]
    [When("ClearSelection is called")]
    [Then("SelectedFilePaths should be empty")]
    public void UAC17_ClearSelectionEmpties()
    {
        StateService.ToggleFileSelection(StateService.State.Files[0].Path);
        StateService.ToggleFileSelection(StateService.State.Files[1].Path);

        StateService.ClearSelection();
        StateService.State.Ui.SelectedFilePaths.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC18: GetPathBreadcrumbs splits the path into segments")]
    [Trait(Spec.UAC, "18")]
    [Given("a path with three segments is set")]
    [When("GetPathBreadcrumbs is called")]
    [Then("it should return one root crumb plus one per segment, with the last marked current")]
    public void UAC18_GetPathBreadcrumbsSplitsPath()
    {
        StateService.SetPath("src/components/lib");

        var crumbs = StateService.GetPathBreadcrumbs();
        crumbs.Count.ShouldBe(4);
        crumbs[0].IsCurrent.ShouldBeFalse();
        crumbs[^1].IsCurrent.ShouldBeTrue();
        crumbs[1].Label.ShouldBe("src");
        crumbs[2].Label.ShouldBe("components");
        crumbs[3].Label.ShouldBe("lib");
    }

    [Fact(DisplayName = "UAC19: GetPathBreadcrumbs at root returns only the repo name")]
    [Trait(Spec.UAC, "19")]
    [Given("the current path is at the repository root")]
    [When("GetPathBreadcrumbs is called")]
    [Then("only the current repository name should be returned")]
    public void UAC19_GetPathBreadcrumbsAtRoot()
    {
        var crumbs = StateService.GetPathBreadcrumbs();
        crumbs.Count.ShouldBe(1);
        crumbs[0].Label.ShouldBe(StateService.State.Current!.Name);
    }

    [Fact(DisplayName = "UAC20: GetRootFiles returns only files at the repo root")]
    [Trait(Spec.UAC, "20")]
    [Given("a state service with files in nested paths")]
    [When("GetRootFiles is called")]
    [Then("only files whose path does not contain a slash should be returned")]
    public void UAC20_GetRootFilesReturnsOnlyRoot()
    {
        var rootFiles = StateService.GetRootFiles();
        rootFiles.ShouldAllBe(f => !f.Path.Contains('/'));
    }

    [Fact(DisplayName = "UAC21: Read-only state prevents ToggleStar from incrementing")]
    [Trait(Spec.UAC, "21")]
    [Given("the state service is in read-only mode")]
    [When("ToggleStar is called")]
    [Then("the current stars count should not change")]
    public void UAC21_ReadOnlyPreventsToggleStar()
    {
        var readOnlyState = RepositoryUseCaseData.Primary() with { IsReadOnly = true };
        var service = new RepositoryStateService(readOnlyState);

        var before = service.State.Current!.Stars;
        service.ToggleStar();

        service.State.Current!.Stars.ShouldBe(before);
    }

    [Fact(DisplayName = "UAC22: Read-only state prevents ToggleWatch from incrementing")]
    [Trait(Spec.UAC, "22")]
    [Given("a state service in read-only mode")]
    [When("ToggleWatch is called")]
    [Then("the current watchers count should not change")]
    public void UAC22_ReadOnlyPreventsToggleWatch()
    {
        var readOnlyState = RepositoryUseCaseData.Primary() with { IsReadOnly = true };
        var service = new RepositoryStateService(readOnlyState);

        var before = service.State.Current!.Watchers;
        service.ToggleWatch();

        service.State.Current!.Watchers.ShouldBe(before);
    }

    [Fact(DisplayName = "UAC23: Read-only state prevents IncrementFork from incrementing")]
    [Trait(Spec.UAC, "23")]
    [Given("a state service in read-only mode")]
    [When("IncrementFork is called")]
    [Then("the current forks count should not change")]
    public void UAC23_ReadOnlyPreventsIncrementFork()
    {
        var readOnlyState = RepositoryUseCaseData.Primary() with { IsReadOnly = true };
        var service = new RepositoryStateService(readOnlyState);

        var before = service.State.Current!.Forks;
        service.IncrementFork();

        service.State.Current!.Forks.ShouldBe(before);
    }

    [Fact(DisplayName = "UAC24: SelectRepository clears Current when given an unknown id")]
    [Trait(Spec.UAC, "24")]
    [Given("a state service with a current repository")]
    [When("SelectRepository is called with an unknown id")]
    [Then("Current should be null")]
    public void UAC24_SelectRepositoryClearsOnUnknown()
    {
        StateService.SelectRepository("does/not-exist");
        StateService.State.Current.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC25: SelectRepository sets Current to a matching repository")]
    [Trait(Spec.UAC, "25")]
    [Given("a state service with multiple repositories in the state")]
    [When("SelectRepository is called with a known id")]
    [Then("Current should be set to the matching repository")]
    public void UAC25_SelectRepositorySetsCurrent()
    {
        var target = StateService.State.Repositories.First(r => r.Name == "designer");
        StateService.SelectRepository(target.Id);

        StateService.State.Current.ShouldNotBeNull();
        StateService.State.Current!.Name.ShouldBe("designer");
    }

    [Fact(DisplayName = "UAC26: StateChanged fires when a meaningful dispatch happens")]
    [Trait(Spec.UAC, "26")]
    [Given("a state service subscribed via StateChanged")]
    [When("a dispatch that mutates state is performed")]
    [Then("the StateChanged handler should fire at least once")]
    public void UAC26_StateChangedFiresOnMeaningfulDispatch()
    {
        var fired = 0;
        StateService.StateChanged += _ => fired++;

        StateService.SetView(RepositoryView.Actions);
        fired.ShouldBeGreaterThan(0);
    }
}
