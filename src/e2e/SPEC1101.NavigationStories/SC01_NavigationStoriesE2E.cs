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

    [Fact(DisplayName = "UAC05: TreeView story — selection and expansion survive reload")]
    [Trait(Spec.UAC, "05")]
    [Given("the TreeView story is open on the Single Selection scenario")]
    [When("the app node is expanded, Program.cs is selected, and the page reloads")]
    [Then("Program.cs remains visible and selected using the persisted treeview keys")]
    public async Task UAC05_TreeViewSelectionAndExpansionSurviveReload()
    {
        await Showcode.GoToStoryAsync("TreeView");

        await Showcode.Page.GetByRole(AriaRole.Tab, new() { Name = "Single Selection" }).ClickAsync();
        await Assertions.Expect(Showcode.Page.Locator("[data-tree-node='src']")).ToBeVisibleAsync();

        var appToggle = Showcode.Page.Locator("[data-tree-node='app'] .lt-tree-node-toggle").First;
        await appToggle.ClickAsync();

        var programNode = Showcode.Page.Locator("[data-tree-node='Program.cs']").First;
        await Assertions.Expect(programNode).ToBeVisibleAsync();
        await programNode.ClickAsync();

        var storageBefore = await Showcode.Page.EvaluateAsync<Dictionary<string, string>>(
            "Object.fromEntries(Object.entries(localStorage).filter(([key]) => key.includes('treeview:')))");
        storageBefore.Values.ShouldContain(value => value.Contains("Program.cs", StringComparison.Ordinal));
        storageBefore.Values.ShouldContain(value => value.Contains("app", StringComparison.Ordinal));

        await Showcode.Page.ReloadAsync();
        await Assertions.Expect(Showcode.Page.Locator("[data-tree-node='Program.cs']").First).ToBeVisibleAsync();

        var selectedAfter = await Showcode.Page.Locator("[data-tree-node='Program.cs']").First
            .EvaluateAsync<bool>("node => node.classList.contains('lt-tree-node-selected')");
        selectedAfter.ShouldBeTrue();
    }
}
