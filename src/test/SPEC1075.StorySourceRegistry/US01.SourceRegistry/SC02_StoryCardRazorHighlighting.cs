namespace LowlandTech.Components.Tests.SPEC1075.Showcode.US01.SourceRegistry;

[Trait(Spec.SPEC, "1075")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "02")]
[UserStory("01", "StoryCard highlights Razor source snippets")]
public class SC02_StoryCardRazorHighlighting
{
    [Fact(DisplayName = "UAC01: PascalCase Razor component tags are highlighted")]
    [Trait(Spec.UAC, "01")]
    [Given("a Razor source line that contains a PascalCase component tag")]
    [When("StoryCard highlights the source line")]
    [Then("the component name is wrapped in the markup tag token class")]
    public void UAC01_PascalCaseRazorComponentTagsAreHighlighted()
    {
        var highlightLine = typeof(StoryCard)
            .GetMethod("HighlightLine", BindingFlags.NonPublic | BindingFlags.Static);

        highlightLine.ShouldNotBeNull();

        var html = highlightLine.Invoke(null, ["<WorkAppMain>", "razor"]) as string;

        html.ShouldNotBeNull();
        html.ShouldContain("story-card-token-tag");
        html.ShouldContain("WorkAppMain");
    }
}
