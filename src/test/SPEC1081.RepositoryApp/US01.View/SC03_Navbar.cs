using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Top-level navbar drives the active view")]
public class SC03_Navbar : Infrastructure.RepositoryScenario
{
    private static readonly string[] ExpectedViews =
    [
        "code", "issues", "pulls", "actions", "packages",
        "projects", "releases", "wiki", "activity", "settings"
    ];

    [Fact(DisplayName = "UAC01: Navbar renders all 10 expected tabs")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered")]
    [When("the navbar is inspected")]
    [Then("all 10 expected tab triggers should be present")]
    public void UAC01_NavbarRendersAllTenTabs()
    {
        var cut = RenderRepositoryApp();

        foreach (var view in ExpectedViews)
        {
            cut.Find($"[data-testid='repo-nav-{view}']").ShouldNotBeNull();
        }
    }

    [Fact(DisplayName = "UAC02: Default active tab is Code")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered with the default state")]
    [When("the navbar is inspected")]
    [Then("the Code tab should have data-state=\"active\"")]
    public void UAC02_DefaultActiveTabIsCode()
    {
        var cut = RenderRepositoryApp();

        var code = cut.Find("[data-testid='repo-nav-code']");
        code.GetAttribute("data-state").ShouldBe("active");
    }

    [Fact(DisplayName = "UAC03: Clicking Issues sets ActiveView to Issues")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the Issues tab is clicked")]
    [Then("ActiveView should become Issues and the data-active-view attribute should update")]
    public void UAC03_ClickingIssuesSetsActiveView()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-nav-issues']").Click();
        cut.Render();

        StateService.State.Ui.ActiveView.ShouldBe(RepositoryView.Issues);
        cut.Find("[data-active-view='issues']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Clicking Pulls sets ActiveView to Pulls")]
    [Trait(Spec.UAC, "04")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the Pulls tab is clicked")]
    [Then("ActiveView should become Pulls")]
    public void UAC04_ClickingPullsSetsActiveView()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-nav-pulls']").Click();
        cut.Render();

        StateService.State.Ui.ActiveView.ShouldBe(RepositoryView.Pulls);
    }

    [Fact(DisplayName = "UAC05: Clicking Settings sets ActiveView to Settings")]
    [Trait(Spec.UAC, "05")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the Settings tab is clicked")]
    [Then("ActiveView should become Settings")]
    public void UAC05_ClickingSettingsSetsActiveView()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-nav-settings']").Click();
        cut.Render();

        StateService.State.Ui.ActiveView.ShouldBe(RepositoryView.Settings);
    }

    [Fact(DisplayName = "UAC06: Issues badge reflects OpenIssues count")]
    [Trait(Spec.UAC, "06")]
    [Given("a RepositoryApp is rendered with a current repository")]
    [When("the Issues tab is inspected")]
    [Then("the badge text should contain the current OpenIssues count")]
    public void UAC06_IssuesBadgeReflectsOpenIssues()
    {
        var cut = RenderRepositoryApp();

        var issuesTab = cut.Find("[data-testid='repo-nav-issues']");
        issuesTab.TextContent.ShouldContain(StateService.State.Current!.OpenIssues.ToString());
    }

    [Fact(DisplayName = "UAC07: Pulls badge reflects OpenPulls count")]
    [Trait(Spec.UAC, "07")]
    [Given("a RepositoryApp is rendered with a current repository")]
    [When("the Pulls tab is inspected")]
    [Then("the badge text should contain the current OpenPulls count")]
    public void UAC07_PullsBadgeReflectsOpenPulls()
    {
        var cut = RenderRepositoryApp();

        var pullsTab = cut.Find("[data-testid='repo-nav-pulls']");
        pullsTab.TextContent.ShouldContain(StateService.State.Current!.OpenPulls.ToString());
    }

    [Fact(DisplayName = "UAC08: Switching tabs moves data-active to the new tab only")]
    [Trait(Spec.UAC, "08")]
    [Given("the Code tab is active by default")]
    [When("the user clicks the Settings tab")]
    [Then("data-state=\"active\" should be on Settings and not on Code")]
    public void UAC08_ActiveMovesBetweenTabs()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-testid='repo-nav-settings']").Click();
        cut.Render();

        cut.Find("[data-testid='repo-nav-settings']").GetAttribute("data-state").ShouldBe("active");
        cut.Find("[data-testid='repo-nav-code']").GetAttribute("data-state").ShouldBe("inactive");
    }

    [Fact(DisplayName = "UAC09: Empty repository hides actions/releases/activity tabs")]
    [Trait(Spec.UAC, "09")]
    [Given("a repository flagged IsEmpty")]
    [When("the navbar is rendered")]
    [Then("Actions, Releases, and Activity tabs should not appear")]
    public void UAC09_EmptyRepoHidesNonEmptyTabs()
    {
        var emptyState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with { IsEmpty = true }
        };
        var cut = RenderRepositoryAppWithState(emptyState);

        cut.FindAll("[data-testid='repo-nav-actions']").Count.ShouldBe(0);
        cut.FindAll("[data-testid='repo-nav-releases']").Count.ShouldBe(0);
        cut.FindAll("[data-testid='repo-nav-activity']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC10: Navbar tabs use the Tabs library component, not raw HTML")]
    [Trait(Spec.UAC, "10")]
    [Given("the repository navbar is rendered")]
    [When("the tab controls are inspected")]
    [Then("every tab should be a TabsTrigger from the library, not a raw <button> tag")]
    public void UAC10_NavbarButtonsUseTabsComponent()
    {
        var cut = RenderRepositoryApp();

        var code = cut.Find("[data-testid='repo-nav-code']");
        code.GetAttribute("data-slot").ShouldBe("tabs-trigger");
        cut.FindAll("[data-slot='repository-app-navbar'] > button").Count.ShouldBe(0);
    }
}
