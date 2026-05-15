namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US02 - StoryCard state tabs switch the active rendered example.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
public sealed class SC02_StoryCardStateTabsE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-03: Selecting With form switches preview and code tabs")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC01_SwitchesPopoverStatePreviewAndCodeTabs()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var stateTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "With form"
        }).First;

        await stateTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        (await stateTab.GetAttributeAsync("data-state")).ShouldBe("active");
        var formTrigger = card.Locator("[data-slot='popover-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Dimensions"
        }).First;
        await Assertions.Expect(formTrigger).ToBeVisibleAsync();
        await Assertions.Expect(card.GetByText("PopoverProvider.razor", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }
}
