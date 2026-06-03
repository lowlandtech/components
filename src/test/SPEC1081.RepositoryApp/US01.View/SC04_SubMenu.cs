using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "04")]
[UserStory("01", "SubMenu switches between Files/Commits/Branches/Tags code views")]
public class SC04_SubMenu : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: SubMenu renders all four sub-items with counts")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the submenu is inspected")]
    [Then("all four sub-items (Files, Commits, Branches, Tags) should be present")]
    public void UAC01_SubMenuRendersAllFourSubItems()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-submenu-files']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-submenu-commits']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-submenu-branches']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-submenu-tags']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Files count matches the root file count")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered with a seeded state")]
    [When("the Files sub-item is inspected")]
    [Then("its count should match the seeded file count")]
    public void UAC02_FilesCountMatchesSeed()
    {
        var cut = RenderRepositoryApp();

        var files = cut.Find("[data-testid='repo-submenu-files']");
        files.TextContent.ShouldContain(StateService.State.Files.Count.ToString("N0"));
    }

    [Fact(DisplayName = "UAC03: Commits count matches the commit count")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered with a seeded state")]
    [When("the Commits sub-item is inspected")]
    [Then("its count should match the seeded commit count")]
    public void UAC03_CommitsCountMatchesSeed()
    {
        var cut = RenderRepositoryApp();

        var commits = cut.Find("[data-testid='repo-submenu-commits']");
        commits.TextContent.ShouldContain(StateService.State.Commits.Count.ToString("N0"));
    }

    [Fact(DisplayName = "UAC04: Branches count matches the branch count")]
    [Trait(Spec.UAC, "04")]
    [Given("a RepositoryApp is rendered with a seeded state")]
    [When("the Branches sub-item is inspected")]
    [Then("its count should match the seeded branch count")]
    public void UAC04_BranchesCountMatchesSeed()
    {
        var cut = RenderRepositoryApp();

        var branches = cut.Find("[data-testid='repo-submenu-branches']");
        branches.TextContent.ShouldContain(StateService.State.Branches.Count.ToString("N0"));
    }

    [Fact(DisplayName = "UAC05: Tags count matches the tag count")]
    [Trait(Spec.UAC, "05")]
    [Given("a RepositoryApp is rendered with a seeded state")]
    [When("the Tags sub-item is inspected")]
    [Then("its count should match the seeded tag count")]
    public void UAC05_TagsCountMatchesSeed()
    {
        var cut = RenderRepositoryApp();

        var tags = cut.Find("[data-testid='repo-submenu-tags']");
        tags.TextContent.ShouldContain(StateService.State.Tags.Count.ToString("N0"));
    }

    [Fact(DisplayName = "UAC06: Default active sub-item is Files")]
    [Trait(Spec.UAC, "06")]
    [Given("a RepositoryApp is rendered with the default state")]
    [When("the submenu is inspected")]
    [Then("the Files sub-item should have data-state=\"active\"")]
    public void UAC06_DefaultActiveSubItemIsFiles()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-submenu-files']").GetAttribute("data-state").ShouldBe("active");
    }

    [Fact(DisplayName = "UAC07: Clicking Commits switches ActiveCodeView")]
    [Trait(Spec.UAC, "07")]
    [Given("a RepositoryApp is rendered with the default sub-view")]
    [When("the Commits sub-item is clicked")]
    [Then("ActiveCodeView should become Commits")]
    public void UAC07_ClickingCommitsSwitchesCodeView()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-submenu-commits']").Click();
        cut.Render();

        StateService.State.Ui.ActiveCodeView.ShouldBe(RepositoryCodeView.Commits);
    }

    [Fact(DisplayName = "UAC08: Clicking Branches switches ActiveCodeView")]
    [Trait(Spec.UAC, "08")]
    [Given("a RepositoryApp is rendered with the default sub-view")]
    [When("the Branches sub-item is clicked")]
    [Then("ActiveCodeView should become Branches")]
    public void UAC08_ClickingBranchesSwitchesCodeView()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-submenu-branches']").Click();
        cut.Render();

        StateService.State.Ui.ActiveCodeView.ShouldBe(RepositoryCodeView.Branches);
    }

    [Fact(DisplayName = "UAC09: Clicking Tags switches ActiveCodeView")]
    [Trait(Spec.UAC, "09")]
    [Given("a RepositoryApp is rendered with the default sub-view")]
    [When("the Tags sub-item is clicked")]
    [Then("ActiveCodeView should become Tags")]
    public void UAC09_ClickingTagsSwitchesCodeView()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-submenu-tags']").Click();
        cut.Render();

        StateService.State.Ui.ActiveCodeView.ShouldBe(RepositoryCodeView.Tags);
    }

    [Fact(DisplayName = "UAC10: SubMenu is hidden when ActiveView is not Code")]
    [Trait(Spec.UAC, "10")]
    [Given("the user has switched to the Issues view")]
    [When("the submenu is inspected")]
    [Then("the submenu should not be rendered")]
    public void UAC10_SubMenuHiddenOffCodeView()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Issues);
        cut.Render();

        cut.FindAll("[data-slot='repository-app-submenu']").Count.ShouldBe(0);
    }
}
