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
    [When("the user clicks the second code tab 'PopoverProvider.razor'")]
    [Then("the page URL contains the codetab query parameter with value 'form-service'")]
    public async Task UAC01_CodeTabSelectionWritesToQueryString()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await SelectWithFormScenarioAsync(card);

        var providerTab = ProviderCodeTab(card);
        await providerTab.ClickAsync();
        await Assertions.Expect(providerTab).ToHaveAttributeAsync("data-state", "active");
        await WaitForUrlCodeTabAsync();

        var url = Showcode.Page.Url;
        url.ShouldContain("codetab=");
        url.ShouldContain("form-service");
    }

    [Fact(DisplayName = "UAC02: reloading the page restores the previously selected code tab")]
    [Trait(Spec.UAC, "02")]
    [Given("the Popover story is displayed and the user selected 'PopoverProvider.razor'")]
    [When("the page is reloaded")]
    [Then("the 'PopoverProvider.razor' code tab is active and its content is visible")]
    public async Task UAC02_ReloadRestoresSelectedCodeTab()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await SelectWithFormScenarioAsync(card);

        var providerTab = ProviderCodeTab(card);
        await providerTab.ClickAsync();
        await Assertions.Expect(providerTab).ToHaveAttributeAsync("data-state", "active");
        await WaitForSessionStorageCodeTabAsync();

        await Showcode.Page.ReloadAsync();
        await Showcode.GoToStoryAsync("Popover");

        card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await Assertions.Expect(card.GetByRole(AriaRole.Heading, new() { Name = "Popover", Exact = true }))
            .ToBeVisibleAsync();

        var restoredTab = ProviderCodeTab(card);
        await Assertions.Expect(restoredTab).ToHaveAttributeAsync("data-state", "active");

        var activeCode = card.Locator("[data-slot='tabs-content'][data-state='active'] .story-card-code-editor").First;
        await Assertions.Expect(activeCode).ToContainTextAsync("<PopoverProvider />");
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
        await Assertions.Expect(withFormTab).ToHaveAttributeAsync("data-state", "active");

        // Select a non-first code tab in that scenario
        var formTab = FormCodeTab(card);
        await formTab.ClickAsync();
        await Assertions.Expect(formTab).ToHaveAttributeAsync("data-state", "active");

        // Switch to "Outlined" scenario
        var outlinedTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Outlined"
        }).First;
        await outlinedTab.ClickAsync();

        await Assertions.Expect(outlinedTab).ToHaveAttributeAsync("data-state", "active");

        // The first code tab of the new scenario should be active
        var firstCodeTab = card.Locator(".story-card-code-tabs [data-slot='tabs-trigger']").First;
        await Assertions.Expect(firstCodeTab).ToHaveAttributeAsync("data-state", "active");
    }

    [Fact(DisplayName = "UAC04: code tab selection is written to sessionStorage")]
    [Trait(Spec.UAC, "04")]
    [Given("the Popover story is displayed")]
    [When("the user clicks the 'PopoverProvider.razor' code tab")]
    [Then("sessionStorage contains the persisted state with CodeTab set to 'form-service'")]
    public async Task UAC04_CodeTabIsPersistedToSessionStorage()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await SelectWithFormScenarioAsync(card);

        var providerTab = ProviderCodeTab(card);
        await providerTab.ClickAsync();
        await Assertions.Expect(providerTab).ToHaveAttributeAsync("data-state", "active");
        await WaitForSessionStorageCodeTabAsync();

        var storageJson = await Showcode.Page.EvaluateAsync<string?>(@"() => {
            const keys = Object.keys(sessionStorage).filter(k => k.startsWith('storycard:'));
            return keys.length > 0 ? sessionStorage.getItem(keys[0]) : null;
        }");

        storageJson.ShouldNotBeNullOrEmpty();
        storageJson.ShouldContain("\"CodeTab\"");
        storageJson.ShouldContain("form-service");
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
        await SelectWithFormScenarioAsync(card);

        var providerTab = ProviderCodeTab(card);
        await providerTab.ClickAsync();
        await Assertions.Expect(providerTab).ToHaveAttributeAsync("data-state", "active");
        await WaitForUrlCodeTabAsync();

        var url = Showcode.Page.Url;
        url.ShouldContain($"{expectedKey}-codetab=");
    }

    private static ILocator FormCodeTab(ILocator card) =>
        card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverFormExample.razor"
        }).First;

    private static ILocator ProviderCodeTab(ILocator card) =>
        card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverProvider.razor"
        }).First;

    private static async Task SelectWithFormScenarioAsync(ILocator card)
    {
        var withFormTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "With form"
        }).First;

        await withFormTab.ClickAsync();
        await Assertions.Expect(withFormTab).ToHaveAttributeAsync("data-state", "active");
        await Assertions.Expect(FormCodeTab(card)).ToBeVisibleAsync();
    }

    private async Task WaitForUrlCodeTabAsync()
    {
        await Showcode.Page.WaitForFunctionAsync(
            "() => window.location.href.includes('codetab=') && window.location.href.includes('form-service')");
    }

    private async Task WaitForSessionStorageCodeTabAsync()
    {
        await Showcode.Page.WaitForFunctionAsync(@"() => {
            const keys = Object.keys(sessionStorage).filter(k => k.startsWith('storycard:'));
            return keys.some(k => sessionStorage.getItem(k)?.includes('""CodeTab"":""form-service""'));
        }");
    }
}
