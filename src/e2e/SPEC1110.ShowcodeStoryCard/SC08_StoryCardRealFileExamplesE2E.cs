namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US08 - StoryCard can render compiled examples and show source from those real files.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "01")]
public sealed class SC08_StoryCardRealFileExamplesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-05: TreeView story displays source from compiled example files")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC01_RendersTreeViewFromRealExampleFiles()
    {
        await Showcode.GoToStoryAsync("TreeView");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await Assertions.Expect(card).ToBeVisibleAsync();
        await Assertions.Expect(card).ToContainTextAsync("TreeViewCustomTemplateExample.razor");
        await Assertions.Expect(card).Not.ToContainTextAsync("TreeViewStory.razor");

        var activeCode = card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
        await Assertions.Expect(activeCode).ToContainTextAsync("@namespace LowlandTech.Components.App.Showcode.Examples.Navigation");
        await Assertions.Expect(activeCode).ToContainTextAsync("TreeViewSampleData.RootItems");

        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "TreeViewSampleData.cs"
        }).First.ClickAsync();

        activeCode = card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
        await Assertions.Expect(activeCode).ToContainTextAsync("public static readonly IReadOnlyList<TreeViewFileItem> RootItems");

        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Single Selection"
        }).First.ClickAsync();

        await Showcode.Page.WaitForTimeoutAsync(300);

        activeCode = card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
        await Assertions.Expect(activeCode).ToContainTextAsync("TreeViewSelectionExample.razor");
        await Assertions.Expect(activeCode).ToContainTextAsync("SelectedKeysChanged");
    }
}
