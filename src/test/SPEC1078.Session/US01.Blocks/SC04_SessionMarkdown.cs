namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Session responses render model Markdown without executing embedded HTML")]
public sealed class SC04_SessionMarkdown : ViewScenario
{
    [Fact(DisplayName = "UAC01: Session response Markdown renders semantic formatting")]
    [Trait(Spec.UAC, "01")]
    [Given("a session response containing headings, emphasis, lists, links, and fenced code")]
    [When("the SessionMarkdown response surface renders the model output")]
    [Then("the Markdown syntax becomes semantic HTML instead of visible punctuation")]
    public void UAC01_RendersSemanticMarkdown()
    {
        const string markdown = """
            ## Providers

            Select a **provider**:

            - Ollama
            - Kimi

            Read the [provider guide](https://example.test/providers).

            ```csharp
            var provider = "kimi";
            ```
            """;

        var cut = Ctx.Render<SessionMarkdown>(parameters => parameters
            .Add(component => component.Value, markdown)
            .AddUnmatched("data-slot", "session-turn-response-text")
            .AddUnmatched("data-streaming", "false"));

        cut.Find("h2").TextContent.ShouldBe("Providers");
        cut.Find("strong").TextContent.ShouldBe("provider");
        cut.FindAll("ul li").Select(item => item.TextContent).ShouldBe(["Ollama", "Kimi"]);
        cut.Find("a").GetAttribute("href").ShouldBe("https://example.test/providers");
        cut.Find("pre code.language-csharp").TextContent.ShouldContain("var provider");
        cut.Find("[data-slot='session-turn-response-text']")
            .GetAttribute("data-streaming")
            .ShouldBe("false");
    }

    [Fact(DisplayName = "UAC02: Session response Markdown does not execute model-supplied HTML")]
    [Trait(Spec.UAC, "02")]
    [Given("a session response containing raw script and HTML elements")]
    [When("the SessionMarkdown response surface renders the model output")]
    [Then("raw HTML is displayed as text and no executable element is created")]
    public void UAC02_DisablesRawHtml()
    {
        const string markdown = "<script>alert('no')</script>\n\n<strong>model html</strong>";

        var cut = Ctx.Render<SessionMarkdown>(parameters => parameters
            .Add(component => component.Value, markdown));

        cut.FindAll("script").ShouldBeEmpty();
        cut.FindAll("strong").ShouldBeEmpty();
        var root = cut.Find(".session-markdown");
        root.TextContent.ShouldContain("<script>alert('no')</script>");
        root.TextContent.ShouldContain("<strong>model html</strong>");
    }
}
