namespace LowlandTech.Components.E2E.SPEC1110.ShowcodeStoryCard;

/// <summary>
/// SPEC1110 US01 - StoryCard renders reusable documentation chrome.
/// </summary>
[Trait(Spec.SPEC, "1110")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
public sealed class SC01_StoryCardRenderingE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01-06: StoryCard renders chrome, active preview, and editor code")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    [Trait(Spec.UAC, "05")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC01_RendersStoryCardChromePreviewAndCode()
    {
        await Showcode.GoToStoryAsync("Popover");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await Assertions.Expect(card).ToBeVisibleAsync();

        await Assertions.Expect(card.Locator("h2").Filter(new LocatorFilterOptions { HasText = "Popover" }).First).ToBeVisibleAsync();

        var trigger = card.Locator("[data-slot='popover-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();

        var codeTab = card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "PopoverBasicExample.razor"
        }).First;
        await Assertions.Expect(codeTab).ToBeVisibleAsync();

        var editor = card.Locator(".story-card-code-editor").First;
        await Assertions.Expect(editor).ToContainTextAsync("<Popover>");
        (await editor.GetAttributeAsync("data-language")).ShouldBe("razor");
        await Assertions.Expect(card.Locator(".story-card-line-number").First).ToHaveTextAsync("1");
    }

    [Fact(DisplayName = "UAC07: Story preview expands to the browser viewport and restores")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_StoryPreviewExpandsToViewportAndRestores()
    {
        await Showcode.GoToStoryAsync("SessionBlock");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        var preview = card.Locator("[data-slot='story-preview-container'][data-expanded='false']").First;
        var expandButton = preview.Locator("[data-slot='story-preview-expand-button']");

        await Assertions.Expect(expandButton).ToHaveCSSAsync("opacity", "0");
        await expandButton.HoverAsync();
        await Assertions.Expect(expandButton).ToHaveCSSAsync("opacity", "1");
        await expandButton.ClickAsync();

        var expandedPreview = card.Locator("[data-slot='story-preview-container'][data-expanded='true']");
        await Assertions.Expect(expandedPreview).ToBeVisibleAsync();
        var restoreButton = expandedPreview.Locator("[data-slot='story-preview-expand-button']");
        await Assertions.Expect(restoreButton)
            .ToHaveAttributeAsync("aria-expanded", "true");

        var box = await expandedPreview.BoundingBoxAsync();
        box.ShouldNotBeNull();
        box.X.ShouldBe(0, 1);
        box.Y.ShouldBe(0, 1);
        box.Width.ShouldBe(1280, 1);
        box.Height.ShouldBe(900, 1);

        await Showcode.Page.Mouse.MoveAsync(0, 0);
        await Assertions.Expect(restoreButton).ToHaveCSSAsync("opacity", "0");
        await restoreButton.HoverAsync();
        await Assertions.Expect(restoreButton).ToHaveCSSAsync("opacity", "1");
        await restoreButton.ClickAsync();

        await Assertions.Expect(preview).ToHaveAttributeAsync("data-expanded", "false");
        await Assertions.Expect(preview.Locator("[data-slot='story-preview-expand-button']"))
            .ToHaveAttributeAsync("aria-expanded", "false");
    }

    [Fact(DisplayName = "UAC08: Selecting a session and context node renders without an unhandled error")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_SelectingSessionRendersContextTreeWithoutError()
    {
        var pageErrors = new List<string>();
        Showcode.Page.PageError += (_, error) => pageErrors.Add(error);

        await Showcode.GoToStoryAsync("SessionBlock");

        var card = Showcode.Page.Locator("main [data-slot='story-card']").First;
        await card.Locator("[data-slot='tabs-trigger']").Filter(new LocatorFilterOptions
        {
            HasText = "Standalone"
        }).First.ClickAsync();

        var sessionBlock = card.Locator("[data-slot='session-block'][data-variant='none']");
        await Assertions.Expect(sessionBlock).ToBeVisibleAsync();
        await Assertions.Expect(sessionBlock).ToHaveClassAsync(new Regex(@"\bh-full\b"));
        await Assertions.Expect(sessionBlock).Not.ToHaveClassAsync(new Regex(@"\bborder\b"));

        var ordersSession = card.Locator("[data-testid='session-list-item'][data-node-id='orders-session']");
        await Assertions.Expect(ordersSession).ToBeVisibleAsync();
        await ordersSession.ClickAsync();

        var ordersContext = card.Locator("[data-slot='session-context-node'][data-node-id='orders']");
        await Assertions.Expect(ordersContext).ToBeVisibleAsync();
        await ordersContext.ClickAsync();

        await Assertions.Expect(
            card.Locator("[data-slot='session-context-block']")
        ).ToBeVisibleAsync();
        await Assertions.Expect(
            card.Locator("[role='tab'][aria-selected='true']")
                .Filter(new LocatorFilterOptions { HasText = "Orders" })
                .First
        ).ToBeVisibleAsync();
        pageErrors.ShouldBeEmpty();
    }
}
