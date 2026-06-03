using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "09")]
[UserStory("01", "Branches sub-view lists branches and allows selection")]
public class SC09_Branches : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Branches list is hidden by default")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered with the default sub-view")]
    [When("the content area is inspected")]
    [Then("the branch list should not be present")]
    public void UAC01_BranchesListHiddenByDefault()
    {
        var cut = RenderRepositoryApp();

        cut.FindAll("[data-slot='repository-branch-list']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Switching to Branches sub-view renders the list")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the user switches to the Branches sub-view")]
    [Then("the branch list should be rendered")]
    public void UAC02_SwitchingToBranchesRendersList()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        cut.Find("[data-slot='repository-branch-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Branch list renders one row per branch")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered on the Branches sub-view")]
    [When("the branch list is inspected")]
    [Then("it should render one row per branch in the state")]
    public void UAC03_BranchListRendersOneRowPerBranch()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        var rows = cut.FindAll("[data-slot='repository-branch-list'] [data-slot='repo-branch-row']");
        rows.Count.ShouldBe(StateService.State.Branches.Count);
    }

    [Fact(DisplayName = "UAC04: Each branch row exposes data-branch attribute")]
    [Trait(Spec.UAC, "04")]
    [Given("the branch list is rendered")]
    [When("the branch rows are inspected")]
    [Then("each row should expose a non-empty data-branch attribute")]
    public void UAC04_EachBranchRowExposesDataBranch()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        var rows = cut.FindAll("[data-slot='repository-branch-list'] [data-slot='repo-branch-row']");
        foreach (var r in rows)
        {
            r.GetAttribute("data-branch").ShouldNotBeNullOrEmpty();
        }
    }

    [Fact(DisplayName = "UAC05: Default branch row has the Default badge")]
    [Trait(Spec.UAC, "05")]
    [Given("the branch list is rendered with a default branch in the state")]
    [When("the default branch row is inspected")]
    [Then("the row should display a Default badge")]
    public void UAC05_DefaultBranchRowHasDefaultBadge()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        var defaultBranch = StateService.State.Branches.First(b => b.IsDefault);
        var row = cut.Find($"[data-slot='repository-branch-list'] [data-branch='{defaultBranch.Name}']");
        row.TextContent.ShouldContain("Default");
    }

    [Fact(DisplayName = "UAC06: Protected branches show the Protected badge")]
    [Trait(Spec.UAC, "06")]
    [Given("the branch list is rendered with a protected branch in the state")]
    [When("the branch list is inspected")]
    [Then("a Protected badge should be visible")]
    public void UAC06_ProtectedBranchesShowProtectedBadge()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        var list = cut.Find("[data-slot='repository-branch-list']");
        list.TextContent.ShouldContain("Protected");
    }

    [Fact(DisplayName = "UAC07: Clicking a branch updates SelectedBranch")]
    [Trait(Spec.UAC, "07")]
    [Given("a RepositoryApp is rendered on the Branches sub-view")]
    [When("the user clicks a non-default branch's select control")]
    [Then("SelectedBranch should update to that branch's name")]
    public void UAC07_ClickingBranchUpdatesSelected()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        var target = StateService.State.Branches.First(b => !b.IsDefault).Name;
        cut.Find($"[data-slot='repository-branch-list'] [data-branch='{target}'] [data-testid='repo-branch-select']").Click();
        cut.Render();

        StateService.State.Ui.SelectedBranch.ShouldBe(target);
    }

    [Fact(DisplayName = "UAC08: Selected branch row carries data-selected=true")]
    [Trait(Spec.UAC, "08")]
    [Given("a non-default branch has been selected")]
    [When("the branch list is rendered")]
    [Then("the matching row should carry data-selected=\"true\"")]
    public void UAC08_SelectedBranchCarriesDataSelected()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        StateService.SetBranch("develop");
        cut.Render();

        var row = cut.Find("[data-slot='repository-branch-list'] [data-branch='develop']");
        row.GetAttribute("data-selected").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC09: Branch ahead/behind counters render when non-zero")]
    [Trait(Spec.UAC, "09")]
    [Given("a branch with AheadBy greater than zero")]
    [When("the branch row is rendered")]
    [Then("the ahead counter should appear in the row")]
    public void UAC09_BranchAheadBehindCountersRender()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        var branch = StateService.State.Branches.First(b => b.AheadBy > 0);
        var row = cut.Find($"[data-slot='repository-branch-list'] [data-branch='{branch.Name}']");
        row.TextContent.ShouldContain(branch.AheadBy!.Value.ToString());
    }

    [Fact(DisplayName = "UAC10: New-branch button is present")]
    [Trait(Spec.UAC, "10")]
    [Given("a RepositoryApp is rendered on the Branches sub-view")]
    [When("the header is inspected")]
    [Then("a New branch button should be present")]
    public void UAC10_NewBranchButtonIsPresent()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Branches);
        cut.Render();

        cut.Find("[data-testid='repo-branch-new']").ShouldNotBeNull();
    }
}
