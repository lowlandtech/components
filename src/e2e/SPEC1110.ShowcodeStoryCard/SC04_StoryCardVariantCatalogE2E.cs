namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US04 - StoryCard supports reusable variant catalogs across component groups.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
public sealed class SC04_StoryCardVariantCatalogE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-04: Variant stories render state previews and matching source")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC01_RendersVariantStoriesWithMatchingSource()
    {
        await AssertButtonStoryAsync();
        await AssertBadgeStoryAsync();
        await AssertAlertStoryAsync();
        await AssertTabsStoryAsync();
    }

    private async Task AssertButtonStoryAsync()
    {
        await GoToStoryExactAsync("Button");

        var card = CurrentStoryCard;
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Sizes"
        }).First.ClickAsync();

        await Assertions.Expect(card.Locator("[data-slot='button']").Filter(new LocatorFilterOptions
        {
            HasText = "Large"
        }).First).ToBeVisibleAsync();

        var code = ActiveCodeEditor(card);
        await Assertions.Expect(code).ToContainTextAsync("<Button Size=\"lg\">Large</Button>");
    }

    private async Task AssertBadgeStoryAsync()
    {
        await GoToStoryExactAsync("Badge");

        var card = CurrentStoryCard;
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "In context"
        }).First.ClickAsync();

        await Assertions.Expect(card.Locator("[data-slot='badge']").Filter(new LocatorFilterOptions
        {
            HasText = "Draft"
        }).First).ToBeVisibleAsync();

        var code = ActiveCodeEditor(card);
        await Assertions.Expect(code).ToContainTextAsync("<Badge Variant=\"outline\">Draft</Badge>");
    }

    private async Task AssertAlertStoryAsync()
    {
        await GoToStoryExactAsync("Alert");

        var card = CurrentStoryCard;
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Destructive"
        }).First.ClickAsync();

        var alert = card.Locator("[data-slot='alert']").First;
        await Assertions.Expect(alert).ToBeVisibleAsync();
        (await alert.GetAttributeAsync("data-variant")).ShouldBe("destructive");

        var code = ActiveCodeEditor(card);
        await Assertions.Expect(code).ToContainTextAsync("<Alert Variant=\"destructive\">");
    }

    private async Task AssertTabsStoryAsync()
    {
        await GoToStoryExactAsync("Tabs");

        var card = CurrentStoryCard;
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Line"
        }).First.ClickAsync();

        var lineTabsList = card.Locator("[data-slot='tabs-list'][data-variant='line']").First;
        await Assertions.Expect(lineTabsList).ToBeVisibleAsync();

        var code = ActiveCodeEditor(card);
        await Assertions.Expect(code).ToContainTextAsync("<TabsList Variant=\"line\">");
    }

    private ILocator CurrentStoryCard => Showcode.Page.Locator("main [data-slot='story-card']").First;

    private async Task GoToStoryExactAsync(string storyName)
    {
        await Showcode.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = storyName,
            Exact = true
        }).First.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(600);
    }

    private static ILocator ActiveCodeEditor(ILocator card)
    {
        return card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
    }
}
