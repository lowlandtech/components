namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US07 - StoryCard supports layout, data, and feedback catalogs.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "01")]
public sealed class SC07_StoryCardLayoutFeedbackBatchE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-06: Layout, data, and feedback stories render selected states with matching source")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    [Trait(Spec.UAC, "05")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC01_RendersLayoutFeedbackStoriesWithMatchingSource()
    {
        await AssertSpinnerStoryAsync();
        await AssertDataGridStoryAsync();
        await AssertCardStoryAsync();
        await AssertEmptyStoryAsync();
        await AssertSeparatorStoryAsync();
        await AssertSkeletonStoryAsync();
    }

    private async Task AssertSpinnerStoryAsync()
    {
        await GoToStoryExactAsync("Spinner");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Inside button");

        await Assertions.Expect(card.Locator("[data-slot='spinner']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Spinner class=\"size-4\" />");
    }

    private async Task AssertDataGridStoryAsync()
    {
        await GoToStoryExactAsync("DataGrid");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Loading");

        await Assertions.Expect(card.Locator("[data-slot='data-grid'][data-loading='true']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<LoadingTemplate>");
    }

    private async Task AssertCardStoryAsync()
    {
        await GoToStoryExactAsync("Card");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "With action");

        await Assertions.Expect(card.Locator("[data-slot='card-action']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<CardAction>");
    }

    private async Task AssertEmptyStoryAsync()
    {
        await GoToStoryExactAsync("Empty");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "With action");

        await Assertions.Expect(card.Locator("[data-slot='empty-content']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<EmptyContent>");
    }

    private async Task AssertSeparatorStoryAsync()
    {
        await GoToStoryExactAsync("Separator");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "With label");

        await Assertions.Expect(card.Locator("[data-slot='separator']").First).ToContainTextAsync("OR");
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Separator>OR</Separator>");
    }

    private async Task AssertSkeletonStoryAsync()
    {
        await GoToStoryExactAsync("Skeleton");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Media");

        await Assertions.Expect(card.Locator("[data-slot='skeleton']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Skeleton class=\"h-40 w-full rounded-lg\" />");
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
