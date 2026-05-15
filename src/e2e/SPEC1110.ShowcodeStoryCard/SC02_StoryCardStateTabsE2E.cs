namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US02 - StoryCard state tabs switch the active rendered example.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
public sealed class SC02_StoryCardStateTabsE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-04: Selecting states switches preview, code tabs, and visible outlines")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC01_SwitchesPopoverStatePreviewAndCodeTabs()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var outlinedStateTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Outlined"
        }).First;

        await outlinedStateTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        var outlineButton = card.Locator("[data-slot='button']").Filter(new LocatorFilterOptions
        {
            HasText = "Open outlined popover"
        }).First;
        await Assertions.Expect(outlineButton).ToBeVisibleAsync();
        await outlineButton.ClickAsync();

        var popoverContent = card.Locator("[data-slot='popover-content']").First;
        await Assertions.Expect(popoverContent).ToBeVisibleAsync();
        (await popoverContent.GetAttributeAsync("data-variant")).ShouldBe("outline");
        await ExpectVisibleBorderAsync(outlineButton);
        await ExpectVisibleBorderAsync(popoverContent);

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

    private static async Task ExpectVisibleBorderAsync(ILocator locator)
    {
        var borderWidth = await locator.EvaluateAsync<string>("element => getComputedStyle(element).borderTopWidth");
        var borderStyle = await locator.EvaluateAsync<string>("element => getComputedStyle(element).borderTopStyle");

        borderWidth.ShouldNotBe("0px");
        borderStyle.ShouldNotBe("none");
    }
}
