using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "01")]
[UserStory("01", "RepositoryApp root renders with the default Code view")]
public class SC01_RootAndComposition : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Root element has data-testid and data-slot")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered")]
    [When("the root element is inspected")]
    [Then("it should expose data-testid and data-slot attributes for the app shell")]
    public void UAC01_RootElementHasDataTestIdAndSlot()
    {
        var cut = RenderRepositoryApp();

        var root = cut.Find("[data-testid='repository-app']");
        root.ShouldNotBeNull();
        root.GetAttribute("data-slot").ShouldBe("repository-app");
    }

    [Fact(DisplayName = "UAC02: Default active view is Code")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered with the default state")]
    [When("the root element is inspected")]
    [Then("data-active-view should be 'code'")]
    public void UAC02_DefaultActiveViewIsCode()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-active-view='code']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Active view attribute stays in sync with state")]
    [Trait(Spec.UAC, "03")]
    [Given("the RepositoryApp is rendered with the default state")]
    [When("the active view is changed to Issues")]
    [Then("the data-active-view attribute should reflect the new view in lowercase")]
    public void UAC03_ActiveViewAttributeStaysInSyncWithState()
    {
        var cut = RenderRepositoryApp();

        StateService.SetView(RepositoryView.Issues);
        cut.Render();

        cut.Find("[data-active-view='issues']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Current repository renders the owner/repo identity")]
    [Trait(Spec.UAC, "04")]
    [Given("a RepositoryApp is rendered with a current repository")]
    [When("the header is inspected")]
    [Then("the owner and repo name should appear in the header text")]
    public void UAC04_HeaderRendersOwnerAndRepoName()
    {
        var cut = RenderRepositoryApp();

        var header = cut.Find("[data-slot='repository-app-header']");
        header.TextContent.ShouldContain("lowlandtech");
        header.TextContent.ShouldContain("foundry");
    }

    [Fact(DisplayName = "UAC05: Current repository description flows through to the selected-file sidebar")]
    [Trait(Spec.UAC, "05")]
    [Given("a RepositoryApp with a selected file")]
    [When("the sidebar is rendered")]
    [Then("the current repository description should flow into the sidebar About section")]
    public void UAC05_SidebarRendersRepositoryDescription()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var about = cut.Find("[data-slot='repo-sidebar-about']");
        about.TextContent.ShouldContain("Modular, extensible");
    }

    [Fact(DisplayName = "UAC06: Root has the rounded border shell classes")]
    [Trait(Spec.UAC, "06")]
    [Given("a RepositoryApp is rendered")]
    [When("the root element is inspected")]
    [Then("it should carry the standard shell classes that bound the surface visually")]
    public void UAC06_RootHasRoundedBorderShellClasses()
    {
        var cut = RenderRepositoryApp();

        var root = cut.Find("[data-testid='repository-app']");
        root.ClassList.ShouldContain("rounded-lg");
        root.ClassList.ShouldContain("border");
        root.ClassList.ShouldContain("overflow-hidden");
    }

    [Fact(DisplayName = "UAC07: All seeded repositories are reachable through the service")]
    [Trait(Spec.UAC, "07")]
    [Given("a RepositoryApp is rendered with the seeded state")]
    [When("the state service's repositories are inspected")]
    [Then("all four seeded repositories should be present in the state")]
    public void UAC07_AllSeededRepositoriesAreReachable()
    {
        RenderRepositoryApp();

        var ids = StateService.State.Repositories
            .Select(r => r.FullName)
            .ToList();

        ids.ShouldContain("lowlandtech/foundry");
        ids.ShouldContain("lowlandtech/work");
        ids.ShouldContain("lowlandtech/reel");
        ids.ShouldContain("lowlandtech/designer");
    }
}
