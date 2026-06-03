using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "05")]
[UserStory("01", "File tree shows root files and dispatches path changes")]
public class SC05_FileTree : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: File tree renders one row per root file")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered on the Code view with the file tree visible")]
    [When("the file tree is inspected")]
    [Then("it should render one row per root file from the state")]
    public void UAC01_FileTreeRendersOneRowPerRootFile()
    {
        var cut = RenderRepositoryApp();

        var rows = cut.FindAll("[data-slot='repository-file-tree'] [data-testid='repo-tree-item']");
        rows.Count.ShouldBe(StateService.State.TreeFiles.Count);
    }

    [Fact(DisplayName = "UAC02: Each row exposes a data-path attribute")]
    [Trait(Spec.UAC, "02")]
    [Given("the file tree is rendered")]
    [When("the tree rows are inspected")]
    [Then("each row should expose a non-empty data-path attribute")]
    public void UAC02_EachRowExposesDataPath()
    {
        var cut = RenderRepositoryApp();

        var rows = cut.FindAll("[data-slot='repository-file-tree'] [data-testid='repo-tree-item']");
        foreach (var row in rows)
        {
            row.GetAttribute("data-path").ShouldNotBeNullOrEmpty();
        }
    }

    [Fact(DisplayName = "UAC03: Directory rows are marked kind=directory")]
    [Trait(Spec.UAC, "03")]
    [Given("the file tree is rendered")]
    [When("directory rows are inspected")]
    [Then("they should be marked with data-kind=\"directory\"")]
    public void UAC03_DirectoryRowsMarkedKindDirectory()
    {
        var cut = RenderRepositoryApp();

        var directoryRows = cut.FindAll("[data-slot='repository-file-tree'] [data-kind='directory']");
        directoryRows.Count.ShouldBe(StateService.State.TreeFiles.Count(f => f.Kind == RepositoryFileKind.Directory));
    }

    [Fact(DisplayName = "UAC04: File rows are marked kind=file")]
    [Trait(Spec.UAC, "04")]
    [Given("the file tree is rendered")]
    [When("file rows are inspected")]
    [Then("they should be marked with data-kind=\"file\"")]
    public void UAC04_FileRowsMarkedKindFile()
    {
        var cut = RenderRepositoryApp();

        var fileRows = cut.FindAll("[data-slot='repository-file-tree'] [data-kind='file']");
        fileRows.Count.ShouldBe(StateService.State.TreeFiles.Count(f => f.Kind == RepositoryFileKind.File));
    }

    [Fact(DisplayName = "UAC05: Clicking a tree row updates CurrentPath")]
    [Trait(Spec.UAC, "05")]
    [Given("the file tree is rendered with a directory row")]
    [When("the user clicks the directory row")]
    [Then("CurrentPath should update to the directory's path")]
    public void UAC05_ClickingTreeRowUpdatesPath()
    {
        var cut = RenderRepositoryApp();

        var target = StateService.State.TreeFiles.First(f => f.Kind == RepositoryFileKind.Directory);
        var row = cut.Find($"[data-slot='repository-file-tree'] [data-path='{target.Path}']");
        row.Click();
        cut.Render();

        StateService.State.Ui.CurrentPath.ShouldBe(target.Path);
    }

    [Fact(DisplayName = "UAC06: Tree rows are Button components, not raw <button> HTML")]
    [Trait(Spec.UAC, "06")]
    [Given("the file tree is rendered")]
    [When("the row controls are inspected")]
    [Then("each row should be a Button component, not a raw <button> tag")]
    public void UAC06_TreeRowsUseButtonComponent()
    {
        var cut = RenderRepositoryApp();

        var row = cut.Find("[data-slot='repository-file-tree'] [data-testid='repo-tree-item']");
        row.GetAttribute("data-slot").ShouldBe("button");
        cut.FindAll("[data-slot='repository-file-tree'] > button").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC07: Toggle button hides the file tree")]
    [Trait(Spec.UAC, "07")]
    [Given("the file tree is visible by default")]
    [When("the user clicks the tree toggle button")]
    [Then("the tree should no longer render and ShowFileTree should be false")]
    public void UAC07_ToggleButtonHidesFileTree()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-file-tree-toggle']").Click();
        cut.Render();

        StateService.State.Ui.ShowFileTree.ShouldBeFalse();
        cut.FindAll("[data-slot='repository-file-tree']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: Toggle button re-shows the file tree after being hidden")]
    [Trait(Spec.UAC, "08")]
    [Given("the file tree has been hidden")]
    [When("the user clicks the tree toggle button again")]
    [Then("the file tree should reappear")]
    public void UAC08_ToggleButtonRestoresFileTree()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileTree();
        cut.Render();
        cut.FindAll("[data-slot='repository-file-tree']").Count.ShouldBe(0);

        StateService.ToggleFileTree();
        cut.Render();
        cut.Find("[data-slot='repository-file-tree']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: The current path row carries data-active=true")]
    [Trait(Spec.UAC, "09")]
    [Given("a directory path has been set as the current path")]
    [When("the file tree is rendered")]
    [Then("the matching row should carry data-active=\"true\"")]
    public void UAC09_ActivePathCarriesDataActive()
    {
        var cut = RenderRepositoryApp();

        var target = StateService.State.TreeFiles.First(f => f.Kind == RepositoryFileKind.Directory);
        StateService.SetPath(target.Path);
        cut.Render();

        var row = cut.Find($"[data-slot='repository-file-tree'] [data-path='{target.Path}']");
        row.GetAttribute("data-active").ShouldBe("true");
    }
}
