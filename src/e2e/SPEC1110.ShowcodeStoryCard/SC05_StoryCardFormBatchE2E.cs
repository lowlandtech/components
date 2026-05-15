namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US05 - StoryCard supports form component state catalogs.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "01")]
public sealed class SC05_StoryCardFormBatchE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-05: Form stories render selected states with matching source")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC01_RendersFormStoriesWithMatchingSource()
    {
        await AssertButtonGroupStoryAsync();
        await AssertInputStoryAsync();
        await AssertLabelStoryAsync();
        await AssertCheckboxStoryAsync();
        await AssertRadioGroupStoryAsync();
    }

    private async Task AssertButtonGroupStoryAsync()
    {
        await GoToStoryExactAsync("Button Group");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "With input");

        await Assertions.Expect(card.Locator("[data-slot='button-group']").First).ToBeVisibleAsync();
        await Assertions.Expect(card.Locator("[data-slot='input']").First).ToHaveAttributeAsync("placeholder", "Search components...");
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<ButtonGroupText>");
    }

    private async Task AssertInputStoryAsync()
    {
        await GoToStoryExactAsync("Input");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "States");

        var invalidInput = card.Locator("[data-slot='input'][data-invalid='true']").First;
        await Assertions.Expect(invalidInput).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Input Placeholder=\"Invalid\" Invalid=\"true\" Value=\"bad value\" />");
    }

    private async Task AssertLabelStoryAsync()
    {
        await GoToStoryExactAsync("Label");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Description");

        await Assertions.Expect(card.Locator("[data-slot='label-description']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Label For=\"bio\" Description=\"Tell us a little bit about yourself.\">Bio</Label>");
    }

    private async Task AssertCheckboxStoryAsync()
    {
        await GoToStoryExactAsync("Checkbox");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Checked");

        await Assertions.Expect(card.Locator("[data-slot='checkbox'][data-state='checked']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<Checkbox id=\"notifications\" Checked=\"true\" />");
    }

    private async Task AssertRadioGroupStoryAsync()
    {
        await GoToStoryExactAsync("Radio Group");

        var card = CurrentStoryCard;
        await SelectStateAsync(card, "Choice card");

        await Assertions.Expect(card.Locator("[data-slot='radio-group-item'][data-variant='choice-card']").First).ToBeVisibleAsync();
        await Assertions.Expect(ActiveCodeEditor(card)).ToContainTextAsync("<RadioGroupItem Value=\"team\" Variant=\"choice-card\">");
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
