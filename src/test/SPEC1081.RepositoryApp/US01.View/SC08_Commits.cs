using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "08")]
[UserStory("01", "Commits sub-view renders the commit log")]
public class SC08_Commits : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Commits list is not rendered by default")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered with the default sub-view")]
    [When("the content area is inspected")]
    [Then("the commit list should not be present")]
    public void UAC01_CommitsListHiddenByDefault()
    {
        var cut = RenderRepositoryApp();

        cut.FindAll("[data-slot='repository-commit-list']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Switching to Commits sub-view renders the commit list")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the user switches to the Commits sub-view")]
    [Then("the commit list should be rendered")]
    public void UAC02_SwitchingToCommitsRendersList()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        cut.Find("[data-slot='repository-commit-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Commit list renders one row per commit")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered on the Commits sub-view")]
    [When("the commit list is inspected")]
    [Then("it should render one row per commit in the state")]
    public void UAC03_CommitListRendersOneRowPerCommit()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        var rows = cut.FindAll("[data-slot='repository-commit-list'] [data-slot='repo-commit-row']");
        rows.Count.ShouldBe(StateService.State.Commits.Count);
    }

    [Fact(DisplayName = "UAC04: Each commit row exposes a data-sha attribute")]
    [Trait(Spec.UAC, "04")]
    [Given("the commit list is rendered")]
    [When("the commit rows are inspected")]
    [Then("each row should expose a non-empty data-sha attribute")]
    public void UAC04_EachCommitRowExposesDataSha()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        var rows = cut.FindAll("[data-slot='repository-commit-list'] [data-slot='repo-commit-row']");
        foreach (var r in rows)
        {
            r.GetAttribute("data-sha").ShouldNotBeNullOrEmpty();
        }
    }

    [Fact(DisplayName = "UAC05: First commit message matches the seed")]
    [Trait(Spec.UAC, "05")]
    [Given("the commit list is rendered from the seed")]
    [When("the list content is inspected")]
    [Then("the first commit's message should match the seeded message")]
    public void UAC05_FirstCommitMessageMatchesSeed()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        var list = cut.Find("[data-slot='repository-commit-list']");
        list.TextContent.ShouldContain(StateService.State.Commits[0].Message);
    }

    [Fact(DisplayName = "UAC06: Commit row exposes filter/author/branch buttons")]
    [Trait(Spec.UAC, "06")]
    [Given("the commit list is rendered")]
    [When("the header controls are inspected")]
    [Then("filter, author, and branch buttons should all be present")]
    public void UAC06_CommitListExposesFilterButtons()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        cut.Find("[data-testid='repo-commits-filter']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-commits-author']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-commits-branch']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: Commit list shows total commit count")]
    [Trait(Spec.UAC, "07")]
    [Given("the commit list is rendered")]
    [When("the header text is inspected")]
    [Then("the total commit count should be visible in the format 'N commits'")]
    public void UAC07_CommitListShowsTotalCount()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        var list = cut.Find("[data-slot='repository-commit-list']");
        list.TextContent.ShouldContain($"{StateService.State.Commits.Count:N0} commits");
    }

    [Fact(DisplayName = "UAC08: Author name appears in each row")]
    [Trait(Spec.UAC, "08")]
    [Given("the commit list is rendered")]
    [When("a row is inspected")]
    [Then("the author name should appear in the row text")]
    public void UAC08_AuthorNameAppearsInRow()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        cut.Render();

        var firstAuthor = StateService.State.Commits[0].Author.Name;
        var list = cut.Find("[data-slot='repository-commit-list']");
        list.TextContent.ShouldContain(firstAuthor);
    }

    [Fact(DisplayName = "UAC09: Branch label matches the selected branch")]
    [Trait(Spec.UAC, "09")]
    [Given("a RepositoryApp is rendered on the Commits sub-view with a non-default branch selected")]
    [When("the branch dropdown is inspected")]
    [Then("the label should show the selected branch name")]
    public void UAC09_BranchLabelMatchesSelectedBranch()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Commits);
        StateService.SetBranch("develop");
        cut.Render();

        var button = cut.Find("[data-testid='repo-commits-branch']");
        button.TextContent.ShouldContain("develop");
    }
}
