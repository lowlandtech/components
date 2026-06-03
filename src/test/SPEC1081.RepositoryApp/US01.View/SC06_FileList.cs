using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "06")]
[UserStory("01", "File list shows sortable, filterable, paged root files")]
public class SC06_FileList : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: File list renders one row per file")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered on the Code Files view")]
    [When("the file list is inspected")]
    [Then("it should render one row per paged file")]
    public void UAC01_FileListRendersOneRowPerFile()
    {
        var cut = RenderRepositoryApp();

        var rows = cut.FindAll("[data-slot='repository-file-list'] [data-slot='repo-file-row']");
        rows.Count.ShouldBe(StateService.GetPagedFiles().Count);
    }

    [Fact(DisplayName = "UAC02: Each row exposes data-path")]
    [Trait(Spec.UAC, "02")]
    [Given("the file list is rendered")]
    [When("the file rows are inspected")]
    [Then("each row should expose a non-empty data-path attribute")]
    public void UAC02_EachRowExposesDataPath()
    {
        var cut = RenderRepositoryApp();

        var rows = cut.FindAll("[data-slot='repository-file-list'] [data-slot='repo-file-row']");
        foreach (var r in rows)
        {
            r.GetAttribute("data-path").ShouldNotBeNullOrEmpty();
        }
    }

    [Fact(DisplayName = "UAC03: Header has the branch dropdown button")]
    [Trait(Spec.UAC, "03")]
    [Given("the file list is rendered")]
    [When("the header is inspected")]
    [Then("a branch dropdown button showing the selected branch should be present")]
    public void UAC03_HeaderHasBranchDropdown()
    {
        var cut = RenderRepositoryApp();

        var branch = cut.Find("[data-testid='repo-branch-dropdown']");
        branch.ShouldNotBeNull();
        branch.TextContent.ShouldContain("main");
    }

    [Fact(DisplayName = "UAC04: Header has the add-file button")]
    [Trait(Spec.UAC, "04")]
    [Given("the file list is rendered")]
    [When("the header is inspected")]
    [Then("an Add file button should be present")]
    public void UAC04_HeaderHasAddFileButton()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-add-file']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Clone toggle flips ShowClonePanel")]
    [Trait(Spec.UAC, "05")]
    [Given("the file list is rendered with the clone panel hidden")]
    [When("the user clicks the Clone toggle button")]
    [Then("ShowClonePanel should become true and the clone panel should render")]
    public void UAC05_CloneToggleFlipsPanel()
    {
        var cut = RenderRepositoryApp();
        StateService.State.Ui.ShowClonePanel.ShouldBeFalse();

        cut.Find("[data-testid='repo-clone-toggle']").Click();
        cut.Render();

        StateService.State.Ui.ShowClonePanel.ShouldBeTrue();
        cut.Find("[data-slot='repository-clone-panel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Sort-name toggles ascending on first click")]
    [Trait(Spec.UAC, "06")]
    [Given("the file list is sorted by a different column")]
    [When("the user clicks the Name sort header")]
    [Then("the sort column should become Name with ascending order")]
    public void UAC06_SortNameTogglesAscending()
    {
        var cut = RenderRepositoryApp();
        StateService.SetSort(RepositorySortColumn.UpdatedAt, true);
        cut.Render();

        cut.Find("[data-testid='repo-sort-name']").Click();
        cut.Render();

        StateService.State.Ui.SortColumn.ShouldBe(RepositorySortColumn.Name);
        StateService.State.Ui.SortAscending.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC07: Sort-name toggles descending on second click")]
    [Trait(Spec.UAC, "07")]
    [Given("the file list is already sorted by Name ascending")]
    [When("the user clicks the Name sort header again")]
    [Then("the sort order should toggle to descending")]
    public void UAC07_SortNameTogglesDescending()
    {
        var cut = RenderRepositoryApp();
        StateService.SetSort(RepositorySortColumn.Name, true);
        cut.Render();

        cut.Find("[data-testid='repo-sort-name']").Click();
        cut.Render();

        StateService.State.Ui.SortAscending.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC08: Sort-size switches to Size column with ascending order")]
    [Trait(Spec.UAC, "08")]
    [Given("the file list is sorted by Name")]
    [When("the user clicks the Size sort header")]
    [Then("the sort column should become Size")]
    public void UAC08_SortSizeSwitchesColumn()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-sort-size']").Click();
        cut.Render();

        StateService.State.Ui.SortColumn.ShouldBe(RepositorySortColumn.Size);
    }

    [Fact(DisplayName = "UAC09: File select toggles selection on the row")]
    [Trait(Spec.UAC, "09")]
    [Given("the file list is rendered with no selection")]
    [When("the user clicks the select control on a file row")]
    [Then("the file's path should appear in SelectedFilePaths")]
    public void UAC09_FileSelectTogglesSelection()
    {
        var cut = RenderRepositoryApp();
        var target = StateService.State.Files.First().Path;
        StateService.ClearSelection();
        cut.Render();

        var row = cut.Find($"[data-slot='repository-file-list'] [data-path='{target}']");
        row.QuerySelector("[data-testid='repo-file-select']").Click();
        cut.Render();

        StateService.State.Ui.SelectedFilePaths.ShouldContain(target);
    }

    [Fact(DisplayName = "UAC10: Selected row carries data-selected=true")]
    [Trait(Spec.UAC, "10")]
    [Given("a file has been added to the selection")]
    [When("the file list is rendered")]
    [Then("the matching row should carry data-selected=\"true\"")]
    public void UAC10_SelectedRowCarriesDataSelected()
    {
        var cut = RenderRepositoryApp();
        var target = StateService.State.Files.First().Path;
        StateService.ToggleFileSelection(target);
        cut.Render();

        var row = cut.Find($"[data-slot='repository-file-list'] [data-path='{target}']");
        row.GetAttribute("data-selected").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC11: Empty filter result renders the empty state")]
    [Trait(Spec.UAC, "11")]
    [Given("a filter that matches no files is set")]
    [When("the file list is rendered")]
    [Then("the empty state placeholder should be visible")]
    public void UAC11_EmptyFilterRendersEmptyState()
    {
        var cut = RenderRepositoryApp();
        StateService.SetFilter("zzzzz-no-such-file");
        cut.Render();

        cut.FindAll("[data-slot='repository-file-list'] [data-slot='repo-file-row']").Count.ShouldBe(0);
        var list = cut.Find("[data-slot='repository-file-list']");
        list.TextContent.ShouldContain("empty");
    }

    [Fact(DisplayName = "UAC12: Selecting a file twice deselects it")]
    [Trait(Spec.UAC, "12")]
    [Given("a file has been added to the selection")]
    [When("the user selects the same file again")]
    [Then("the file's path should be removed from SelectedFilePaths")]
    public void UAC12_SelectingFileTwiceDeselects()
    {
        var cut = RenderRepositoryApp();
        var target = StateService.State.Files.First().Path;
        StateService.ToggleFileSelection(target);
        StateService.ToggleFileSelection(target);

        StateService.State.Ui.SelectedFilePaths.ShouldNotContain(target);
    }

    [Fact(DisplayName = "UAC13: File list root has the overflow-hidden shell classes")]
    [Trait(Spec.UAC, "13")]
    [Given("the file list is rendered")]
    [When("the file list root is inspected")]
    [Then("it should carry overflow-hidden and flex-col shell classes")]
    public void UAC13_FileListRootHasShellClasses()
    {
        var cut = RenderRepositoryApp();

        var list = cut.Find("[data-slot='repository-file-list']");
        list.ClassList.ShouldContain("overflow-hidden");
        list.ClassList.ShouldContain("flex-col");
    }
}
