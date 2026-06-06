using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "13")]
[UserStory("01", "Active view drives the rendered content area")]
public class SC13_ViewSwitching : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Code view renders the file list by default")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered")]
    [When("the default content area is inspected")]
    [Then("the file list should be visible and the placeholder should not")]
    public void UAC01_CodeViewRendersFileList()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-slot='repository-file-list']").ShouldNotBeNull();
        cut.FindAll("[data-slot='repository-placeholder-view']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Issues view renders the placeholder")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Issues view")]
    [Then("a placeholder labelled Issues should render and the file list should be gone")]
    public void UAC02_IssuesViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Issues);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Issues");
        cut.FindAll("[data-slot='repository-file-list']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Pulls view renders the placeholder with PR title")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Pull Requests view")]
    [Then("a placeholder labelled Pull Requests should render")]
    public void UAC03_PullsViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Pulls);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Pull Requests");
    }

    [Fact(DisplayName = "UAC04: Actions view renders the placeholder")]
    [Trait(Spec.UAC, "04")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Actions view")]
    [Then("a placeholder labelled Actions should render")]
    public void UAC04_ActionsViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Actions);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Actions");
    }

    [Fact(DisplayName = "UAC05: Packages view renders the placeholder")]
    [Trait(Spec.UAC, "05")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Packages view")]
    [Then("a placeholder labelled Packages should render")]
    public void UAC05_PackagesViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Packages);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Packages");
    }

    [Fact(DisplayName = "UAC06: Projects view renders the placeholder")]
    [Trait(Spec.UAC, "06")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Projects view")]
    [Then("a placeholder labelled Projects should render")]
    public void UAC06_ProjectsViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Projects);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Projects");
    }

    [Fact(DisplayName = "UAC07: Releases view renders the placeholder")]
    [Trait(Spec.UAC, "07")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Releases view")]
    [Then("a placeholder labelled Releases should render")]
    public void UAC07_ReleasesViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Releases);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Releases");
    }

    [Fact(DisplayName = "UAC08: Wiki view renders the placeholder")]
    [Trait(Spec.UAC, "08")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Wiki view")]
    [Then("a placeholder labelled Wiki should render")]
    public void UAC08_WikiViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Wiki);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Wiki");
    }

    [Fact(DisplayName = "UAC09: Activity view renders the placeholder")]
    [Trait(Spec.UAC, "09")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Activity view")]
    [Then("a placeholder labelled Activity should render")]
    public void UAC09_ActivityViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Activity);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Activity");
    }

    [Fact(DisplayName = "UAC10: Settings view renders the placeholder")]
    [Trait(Spec.UAC, "10")]
    [Given("a RepositoryApp is rendered")]
    [When("the user switches to the Settings view")]
    [Then("a placeholder labelled Settings should render")]
    public void UAC10_SettingsViewRendersPlaceholder()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Settings);
        cut.Render();

        var placeholder = cut.Find("[data-slot='repository-placeholder-view']");
        placeholder.TextContent.ShouldContain("Settings");
    }

    [Fact(DisplayName = "UAC11: Code view re-renders after switching away and back")]
    [Trait(Spec.UAC, "11")]
    [Given("the user switches to Issues and then back to Code")]
    [When("the content area is inspected")]
    [Then("the file list should be visible again and the placeholder should be gone")]
    public void UAC11_CodeViewRestoresAfterSwitching()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Issues);
        cut.Render();
        StateService.SetView(RepositoryView.Code);
        cut.Render();

        cut.Find("[data-slot='repository-file-list']").ShouldNotBeNull();
        cut.FindAll("[data-slot='repository-placeholder-view']").Count.ShouldBe(0);
    }
}
