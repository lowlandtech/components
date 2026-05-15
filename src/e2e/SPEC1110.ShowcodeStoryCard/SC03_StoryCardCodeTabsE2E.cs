namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US03 - StoryCard code tabs switch source for the selected state.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
public sealed class SC03_StoryCardCodeTabsE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-04: Selecting provider code reveals highlighted provider source")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC01_SwitchesCodeTabsForSelectedState()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "With form"
        }).First.ClickAsync();

        var providerTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverProvider.razor"
        }).First;

        await providerTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        (await providerTab.GetAttributeAsync("data-state")).ShouldBe("active");

        var activeCode = card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
        await Assertions.Expect(activeCode).ToContainTextAsync("<PopoverProvider />");
        await Assertions.Expect(activeCode).Not.ToContainTextAsync("<Input id=\"pop-w\"");
        await Assertions.Expect(activeCode.Locator(".story-card-token-tag").First).ToHaveTextAsync("PopoverProvider");
    }
}
