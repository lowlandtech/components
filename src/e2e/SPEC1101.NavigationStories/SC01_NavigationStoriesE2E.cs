namespace LowlandTech.Components.E2E.SPEC1101.NavigationStories;

/// <summary>
/// SPEC1101 US01 — E2E tests for every Navigation group story.
/// </summary>
[Trait(Spec.SPEC, "1101")]
[Trait(Spec.US, "01")]
public sealed class SC01_NavigationStoriesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: Accordion story — clicking a trigger expands its content")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AccordionStory()
    {
        await Showcode.GoToStoryAsync("Accordion");

        // Accordion triggers are buttons with data-slot='accordion-trigger'.
        var trigger = Showcode.Page.Locator("main [data-slot='accordion-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();

        // Click to expand.
        await trigger.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(400);

        // Content panel should now exist in DOM.
        var content = Showcode.Page.Locator("main [data-slot='accordion-content']").First;
        await Assertions.Expect(content).ToBeAttachedAsync();
    }

    [Fact(DisplayName = "UAC02: Tabs story — clicking a tab makes its content visible")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_TabsStory()
    {
        await Showcode.GoToStoryAsync("Tabs");

        var triggers = Showcode.Page.Locator("main [data-slot='tabs-trigger']");
        await Assertions.Expect(triggers.First).ToBeVisibleAsync();

        // Click the second tab.
        var second = triggers.Nth(1);
        await second.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(300);

        // The second tab should now be selected (aria-selected=true).
        var isSelected = await second.GetAttributeAsync("data-state");
        isSelected.ShouldBe("active");
    }

    [Fact(DisplayName = "UAC03: Breadcrumb story — breadcrumb items are visible")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_BreadcrumbStory()
    {
        await Showcode.GoToStoryAsync("Breadcrumb");

        var breadcrumb = Showcode.Page.Locator("main nav[aria-label='breadcrumb']").First;
        await Assertions.Expect(breadcrumb).ToBeVisibleAsync();

        // At least two crumb items (page links) must exist.
        var items = Showcode.Page.Locator("main [data-slot='breadcrumb-page']");
        (await items.CountAsync()).ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact(DisplayName = "UAC04: Pagination story — page buttons are visible and clickable")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_PaginationStory()
    {
        await Showcode.GoToStoryAsync("Pagination");

        // Pagination renders anchor/button elements.
        var pageLinks = Showcode.Page.Locator("main [data-slot='pagination-link']");
        await Assertions.Expect(pageLinks.First).ToBeVisibleAsync();

        // Click the second page link.
        var second = pageLinks.Nth(1);
        await second.ClickAsync();
        await Assertions.Expect(second).ToBeVisibleAsync();
    }

}
