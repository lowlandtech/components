namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US09 - StoryCard design mode keeps side rails aligned and spaced.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "09")]
[Trait(Spec.SC, "01")]
public sealed class SC09_StoryCardDesignModeLayoutE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-03: Design mode keeps side panels level, separates selectable previews, and keeps selection chrome square")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC01_DesignModeKeepsRailsAlignedAndSelectablesSeparated()
    {
        await Showcode.GoToStoryAsync("Button");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var layout = card.Locator(".story-card-tabs-layout").First;
        await Assertions.Expect(card).ToBeVisibleAsync();

        await Assertions.Expect(layout).ToBeVisibleAsync();
        await card.GetByRole(AriaRole.Switch, new LocatorGetByRoleOptions
        {
            Name = "Design mode"
        }).ClickAsync();

        var tabsRail = card.Locator(".tabs-list-container").First;
        var tabsContentColumn = card.Locator(".story-card-tabs-content-column").First;
        var designSidebarContainer = card.Locator(".story-card-design-sidebar-container").First;
        var designSidebar = card.Locator(".story-card-design-sidebar").First;

        await Assertions.Expect(tabsRail).ToBeVisibleAsync();
        await Assertions.Expect(tabsContentColumn).ToBeVisibleAsync();
        await Assertions.Expect(designSidebarContainer).ToBeVisibleAsync();
        await Assertions.Expect(designSidebar).ToBeVisibleAsync();

        var layoutWidth = await layout.EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        var tabsHeight = await tabsRail.EvaluateAsync<double>("element => element.getBoundingClientRect().height");
        var contentWidth = await tabsContentColumn.EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        var sidebarHeight = await designSidebarContainer.EvaluateAsync<double>("element => element.getBoundingClientRect().height");
        Math.Abs(tabsHeight - sidebarHeight).ShouldBeLessThanOrEqualTo(1d);

        var sidebarWidth = await designSidebarContainer.EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        layoutWidth.ShouldBeGreaterThan(900d);
        contentWidth.ShouldBeGreaterThan(sidebarWidth);

        var contentFlexGrow = await tabsContentColumn.EvaluateAsync<string>("element => getComputedStyle(element).flexGrow");
        contentFlexGrow.ShouldBe("1");

        var sidebarFlexGrow = await designSidebarContainer.EvaluateAsync<string>("element => getComputedStyle(element).flexGrow");
        sidebarFlexGrow.ShouldBe("0");

        var sidebarInsideTabsContent = await designSidebar.EvaluateAsync<bool>("element => element.closest(\"[data-slot='tabs-content']\") !== null");
        sidebarInsideTabsContent.ShouldBeFalse();

        var emptyContent = card.Locator(".story-card-design-property-grid .lt-property-grid-empty-content").First;
        await Assertions.Expect(emptyContent).ToBeVisibleAsync();
        var emptyPaddingBottom = await emptyContent.EvaluateAsync<string>("element => getComputedStyle(element).paddingBottom");
        emptyPaddingBottom.ShouldBe("40px");

        var previewButtons = card.Locator("[data-slot='button'][data-design-selectable='true']");
        (await previewButtons.CountAsync()).ShouldBeGreaterThanOrEqualTo(2);

        var firstBox = await previewButtons.Nth(0).BoundingBoxAsync();
        var secondBox = await previewButtons.Nth(1).BoundingBoxAsync();

        firstBox.ShouldNotBeNull();
        secondBox.ShouldNotBeNull();

        var horizontalGap = (double)(secondBox!.X - (firstBox.X + firstBox.Width));
        horizontalGap.ShouldBeGreaterThan(0d);

        var selectionRadius = await previewButtons.Nth(0).EvaluateAsync<string>("element => getComputedStyle(element, '::after').borderRadius");
        selectionRadius.ShouldBe("0px");
    }
}