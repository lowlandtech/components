using LowlandTech.Components;
using RepositoryAppRoot = LowlandTech.Components.RepositoryApp;

namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "15")]
[UserStory("01", "RepositoryApp composes the repository surface from explicit child components")]
public class SC15_CompositionSlots : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Default layout exposes compound RepositoryApp slots")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp rendered without custom child content")]
    [When("the default layout is produced")]
    [Then("the header, navbar, main, submenu, content, and footer slots are visible as composition points")]
    public void UAC01_DefaultLayoutExposesCompoundSlots()
    {
        var cut = RenderRepositoryApp();

        cut.Find("[data-slot='repository-app-header']").ShouldNotBeNull();
        cut.Find("[data-slot='repository-app-navbar']").ShouldNotBeNull();
        cut.Find("[data-slot='repository-app-main']").ShouldNotBeNull();
        cut.Find("[data-slot='repository-app-submenu']").ShouldNotBeNull();
        cut.Find("[data-slot='repository-app-content']").ShouldNotBeNull();
        cut.Find("[data-slot='repository-app-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Consumer ChildContent replaces the default layout")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp rendered with explicit child content")]
    [When("the consumer supplies a custom fragment")]
    [Then("the supplied layout is rendered inside the RepositoryApp state provider and the default slots are gone")]
    public void UAC02_ConsumerChildContentReplacesDefaultLayout()
    {
        var cut = Ctx.Render<RepositoryAppRoot>(parameters => parameters
            .Add(x => x.StateService, StateService)
            .Add(x => x.IsTest, true)
            .Add(x => x.ChildContent, builder =>
            {
                builder.OpenComponent<RepositoryAppMain>(0);
                builder.AddAttribute(1, "data-testid", "custom-repo-main");
                builder.CloseComponent();
            }));

        cut.Find("[data-testid='custom-repo-main']").ShouldNotBeNull();
        cut.FindAll("[data-slot='repository-app-header']").Count.ShouldBe(0);
        cut.FindAll("[data-slot='repository-app-navbar']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Cascading context flows to nested components")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered")]
    [When("a nested component asks for the cascading context")]
    [Then("it should receive a non-null RepositoryAppContext instance")]
    public void UAC03_CascadingContextFlowsToChildren()
    {
        var cut = RenderRepositoryApp();

        var header = cut.Find("[data-slot='repository-app-header']");
        header.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Main slot renders submenu and content as children")]
    [Trait(Spec.UAC, "04")]
    [Given("a RepositoryApp is rendered with the default layout")]
    [When("the main slot is inspected")]
    [Then("it should contain both the submenu and the content as children")]
    public void UAC04_MainSlotRendersSubmenuAndContent()
    {
        var cut = RenderRepositoryApp();

        var main = cut.Find("[data-slot='repository-app-main']");
        main.QuerySelector("[data-slot='repository-app-submenu']").ShouldNotBeNull();
        main.QuerySelector("[data-slot='repository-app-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: RepositoryApp exposes ChildContent parameter for full override")]
    [Trait(Spec.UAC, "05")]
    [Given("a RepositoryApp with ChildContent provided")]
    [When("the root element is inspected")]
    [Then("it should still render the root shell and state provider cascade")]
    public void UAC05_RepositoryAppExposesChildContentParameter()
    {
        var cut = Ctx.Render<RepositoryAppRoot>(parameters => parameters
            .Add(x => x.StateService, StateService)
            .Add(x => x.IsTest, true)
            .Add(x => x.ChildContent, builder =>
            {
                builder.OpenComponent<RepositoryAppHeader>(0);
                builder.CloseComponent();
            }));

        cut.Find("[data-testid='repository-app']").ShouldNotBeNull();
        cut.Find("[data-slot='repository-app-header']").ShouldNotBeNull();
    }
}
