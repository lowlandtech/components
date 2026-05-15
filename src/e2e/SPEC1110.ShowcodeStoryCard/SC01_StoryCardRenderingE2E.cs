namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US01 - StoryCard renders reusable documentation chrome.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
public sealed class SC01_StoryCardRenderingE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-04: StoryCard renders chrome, active preview, and code")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC01_RendersStoryCardChromePreviewAndCode()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await Assertions.Expect(card).ToBeVisibleAsync();

        await Assertions.Expect(card.GetByText("Popover", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        var trigger = card.Locator("[data-slot='popover-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();

        var codeTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverStory.razor"
        }).First;
        await Assertions.Expect(codeTab).ToBeVisibleAsync();

        await Assertions.Expect(card.Locator("pre code").First).ToContainTextAsync("<Popover>");
    }
}
