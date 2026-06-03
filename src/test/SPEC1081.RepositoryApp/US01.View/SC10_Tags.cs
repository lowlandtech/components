using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "10")]
[UserStory("01", "Tags sub-view lists tags with sha and created-at")]
public class SC10_Tags : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Tag list is hidden by default")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered with the default sub-view")]
    [When("the content area is inspected")]
    [Then("the tag list should not be present")]
    public void UAC01_TagListHiddenByDefault()
    {
        var cut = RenderRepositoryApp();

        cut.FindAll("[data-slot='repository-tag-list']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Switching to Tags sub-view renders the list")]
    [Trait(Spec.UAC, "02")]
    [Given("a RepositoryApp is rendered on the Code view")]
    [When("the user switches to the Tags sub-view")]
    [Then("the tag list should be rendered")]
    public void UAC02_SwitchingToTagsRendersList()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Tags);
        cut.Render();

        cut.Find("[data-slot='repository-tag-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Tag list renders one row per tag")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered on the Tags sub-view")]
    [When("the tag list is inspected")]
    [Then("it should render one row per tag in the state")]
    public void UAC03_TagListRendersOneRowPerTag()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Tags);
        cut.Render();

        var rows = cut.FindAll("[data-slot='repository-tag-list'] [data-slot='repo-tag-row']");
        rows.Count.ShouldBe(StateService.State.Tags.Count);
    }

    [Fact(DisplayName = "UAC04: Each tag row exposes data-tag attribute")]
    [Trait(Spec.UAC, "04")]
    [Given("the tag list is rendered")]
    [When("the tag rows are inspected")]
    [Then("each row should expose a non-empty data-tag attribute")]
    public void UAC04_EachTagRowExposesDataTag()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Tags);
        cut.Render();

        var rows = cut.FindAll("[data-slot='repository-tag-list'] [data-slot='repo-tag-row']");
        foreach (var r in rows)
        {
            r.GetAttribute("data-tag").ShouldNotBeNullOrEmpty();
        }
    }

    [Fact(DisplayName = "UAC05: Tag list shows total tag count")]
    [Trait(Spec.UAC, "05")]
    [Given("the tag list is rendered")]
    [When("the header text is inspected")]
    [Then("the total tag count should be visible in the format 'N tags'")]
    public void UAC05_TagListShowsTotalCount()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Tags);
        cut.Render();

        var list = cut.Find("[data-slot='repository-tag-list']");
        list.TextContent.ShouldContain($"{StateService.State.Tags.Count:N0} tags");
    }

    [Fact(DisplayName = "UAC06: New-tag button is present")]
    [Trait(Spec.UAC, "06")]
    [Given("a RepositoryApp is rendered on the Tags sub-view")]
    [When("the header is inspected")]
    [Then("a New tag button should be present")]
    public void UAC06_NewTagButtonIsPresent()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Tags);
        cut.Render();

        cut.Find("[data-testid='repo-tag-new']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: First tag row's name matches the seed")]
    [Trait(Spec.UAC, "07")]
    [Given("the tag list is rendered from the seed")]
    [When("the first tag row is inspected")]
    [Then("its data-tag attribute should match the seeded first tag name")]
    public void UAC07_FirstTagNameMatchesSeed()
    {
        var cut = RenderRepositoryApp();
        StateService.SetCodeView(RepositoryCodeView.Tags);
        cut.Render();

        var first = StateService.State.Tags[0];
        var row = cut.Find($"[data-slot='repository-tag-list'] [data-tag='{first.Name}']");
        row.ShouldNotBeNull();
    }
}
