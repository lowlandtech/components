namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US10 - StoryCard code tab selection is persisted to query string and sessionStorage.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "01")]
public sealed class SC10_StoryCardCodeTabPersistenceE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: selecting a non-first code tab writes codetab to the URL query string")]
    [Trait(Spec.UAC, "01")]
    [Given("the Popover story is displayed with multiple code tabs")]
    [When("the user clicks the second code tab 'PopoverFormExample.razor'")]
    [Then("the page URL contains the codetab query parameter with value 'PopoverFormExample.razor'")]
    public async Task UAC01_CodeTabSelectionWritesToQueryString()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var formTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;

        await formTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        var url = Showcode.Page.Url;
        url.ShouldContain("codetab=");
        url.ShouldContain("PopoverFormExample.razor");
    }

    [Fact(DisplayName = "UAC02: reloading the page restores the previously selected code tab")]
    [Trait(Spec.UAC, "02")]
    [Given("the Popover story is displayed and the user selected 'PopoverFormExample.razor'")]
    [When("the page is reloaded")]
    [Then("the 'PopoverFormExample.razor' code tab is active and its content is visible")]
    public async Task UAC02_ReloadRestoresSelectedCodeTab()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var formTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;

        await formTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        await Showcode.Page.ReloadAsync();
        await Showcode.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Showcode.Page.WaitForSelectorAsync("[data-slot='story-card']",
            new PageWaitForSelectorOptions { Timeout = 30_000 });

        card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var restoredTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;

        (await restoredTab.GetAttributeAsync("data-state")).ShouldBe("active");

        var activeCode = card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
        await Assertions.Expect(activeCode).ToContainTextAsync("<PopoverFormExample />");
    }

    [Fact(DisplayName = "UAC03: switching scenario tabs resets the code tab to the first file")]
    [Trait(Spec.UAC, "03")]
    [Given("the Popover story is on the 'With form' state with 'PopoverFormExample.razor' selected")]
    [When("the user clicks the 'Outlined' scenario tab")]
    [Then("the first code tab of the 'Outlined' scenario is active")]
    public async Task UAC03_ScenarioSwitchResetsCodeTab()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;

        // Select "With form" scenario
        var withFormTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "With form"
        }).First;
        await withFormTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        // Select a non-first code tab in that scenario
        var formTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;
        await formTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        // Switch to "Outlined" scenario
        var outlinedTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Outlined"
        }).First;
        await outlinedTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        (await outlinedTab.GetAttributeAsync("data-state")).ShouldBe("active");

        // The first code tab of the new scenario should be active
        var firstCodeTab = card.Locator(".story-card-code-tabs [data-slot='tabs-trigger']").First;
        (await firstCodeTab.GetAttributeAsync("data-state")).ShouldBe("active");
    }

    [Fact(DisplayName = "UAC04: code tab selection is written to sessionStorage")]
    [Trait(Spec.UAC, "04")]
    [Given("the Popover story is displayed")]
    [When("the user clicks the 'PopoverFormExample.razor' code tab")]
    [Then("sessionStorage contains the persisted state with CodeTab set to 'PopoverFormExample.razor'")]
    public async Task UAC04_CodeTabIsPersistedToSessionStorage()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var formTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;

        await formTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        var storageJson = await Showcode.Page.EvaluateAsync<string?>(@"() => {
            const keys = Object.keys(sessionStorage).filter(k => k.startsWith('storycard:'));
            return keys.length > 0 ? sessionStorage.getItem(keys[0]) : null;
        }");

        storageJson.ShouldNotBeNullOrEmpty();
        storageJson.ShouldContain("\"CodeTab\"");
        storageJson.ShouldContain("PopoverFormExample.razor");
    }

    [Fact(DisplayName = "UAC05: persistence key is a deterministic GuidV5 derived from the story title")]
    [Trait(Spec.UAC, "05")]
    [Given("the expected GuidV5 key for title 'Popover' is computed")]
    [When("the user clicks any code tab on the Popover story")]
    [Then("the query string contains the 32-char hex GuidV5 key as the parameter prefix")]
    public async Task UAC05_PersistKeyUsesGuidV5()
    {
        var expectedKey = GuidV5.Create(GuidV5.Namespace, "Popover").ToString("N");

        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var formTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;

        await formTab.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        var url = Showcode.Page.Url;
        url.ShouldContain($"{expectedKey}-codetab=");
    }
}
