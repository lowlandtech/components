namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US06 - StoryCard supports control and feedback state catalogs.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "01")]
public sealed class SC06_StoryCardControlsBatchE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-05: Control stories render selected states with matching source")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC01_RendersControlStoriesWithMatchingSource()
    {
        await AssertSliderStoryAsync();
        await AssertSwitchStoryAsync();
        await AssertTextareaStoryAsync();
        await AssertToggleStoryAsync();
        await AssertProgressStoryAsync();
    }

    private async Task AssertSliderStoryAsync()
    {
        await GoToStoryExactAsync("Slider");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Vertical");

        await Assertions.Expect(card.Locator("[data-slot='slider'][data-orientation='vertical']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Slider Value=\"65\" Orientation=\"vertical\">");
    }

    private async Task AssertSwitchStoryAsync()
    {
        await GoToStoryExactAsync("Switch");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Invalid");

        await Assertions.Expect(card.Locator("[data-slot='switch'][data-invalid='true']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Switch id=\"sw-invalid\" Invalid=\"true\" />");
    }

    private async Task AssertTextareaStoryAsync()
    {
        await GoToStoryExactAsync("Textarea");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "States");

        await Assertions.Expect(card.Locator("[data-slot='textarea'][data-invalid='true']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Textarea Invalid=\"true\" Value=\"This needs a little more detail.\" />");
    }

    private async Task AssertToggleStoryAsync()
    {
        await GoToStoryExactAsync("Toggle");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "With icon");

        await Assertions.Expect(card.Locator("[data-slot='toggle']").Filter(new LocatorFilterOptions
        {
            HasText = "Bold"
        }).First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Icon Name=\"bold\" Size=\"14\" />");
    }

    private async Task AssertProgressStoryAsync()
    {
        await GoToStoryExactAsync("Progress");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Indeterminate");

        await Assertions.Expect(card.Locator("[data-slot='progress'][data-state='indeterminate']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Progress IsIndeterminate=\"true\" />");
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

    private static async Task SelectStateAsync(ILocator card, string stateName)
    {
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = stateName
        }).First.ClickAsync();
    }

    private static ILocator ActiveCodeEditor(ILocator card)
    {
        return card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
    }
}
