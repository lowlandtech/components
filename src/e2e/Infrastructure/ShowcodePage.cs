using Microsoft.Playwright;

namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// Page Object Model for the Showcode app. Provides helpers for navigating
/// to stories via the sidebar and locating component elements on the page.
/// </summary>
public sealed class ShowcodePage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public ShowcodePage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    /// <summary>Underlying Playwright page for direct access.</summary>
    public IPage Page => _page;

    /// <summary>
    /// Navigate to the Showcode root and click the sidebar button for the
    /// named story. Waits for the sidebar and content area to be visible.
    /// </summary>
    public async Task GoToStoryAsync(string storyName)
    {
        // Only navigate to root if not already there (avoids full reload per story).
        if (!_page.Url.StartsWith(_baseUrl, StringComparison.OrdinalIgnoreCase))
            await _page.GotoAsync(_baseUrl);

        // Wait for the sidebar to hydrate.
        await _page.WaitForSelectorAsync("[data-slot='sidebar-menu-button']",
            new PageWaitForSelectorOptions { Timeout = 30_000 });

        // Click the exact sidebar entry for this story (exact match avoids
        // "Button" inadvertently matching "Button Group" etc.).
        var btn = _page.Locator("[data-slot='sidebar-menu-button']")
                       .GetByText(storyName, new() { Exact = true });
        await btn.First.ClickAsync();

        // Wait for the story card heading to appear — state-based wait instead of a
        // fixed delay that is too short on slow WASM startup.
        await Assertions.Expect(
            _page.Locator("[data-slot='story-card']")
                 .GetByRole(AriaRole.Heading, new() { Name = storyName, Exact = true })
        ).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 15_000 });
    }

    // ── Locator helpers ──────────────────────────────────────────────────────

    public ILocator Main => _page.Locator("main").First;

    /// <summary>Buttons rendered by the LT Button component (not sidebar).</summary>
    public ILocator ComponentButtons =>
        _page.Locator("main [data-slot='button']");

    public ILocator WithText(string text) =>
        _page.Locator($"text={text}");

    public ILocator ByTestId(string testId) =>
        _page.GetByTestId(testId);

    /// <summary>Checks or checkboxes inside the story main area.</summary>
    public ILocator Checkboxes =>
        _page.Locator("main [data-slot='checkbox']");

    /// <summary>Radio group items inside the story main area.</summary>
    public ILocator RadioInputs =>
        _page.Locator("main [data-slot='radio-group-item']");

    /// <summary>Sliders inside the story main area.</summary>
    public ILocator RangeInputs =>
        _page.Locator("main [data-slot='slider']");

    /// <summary>Text inputs inside the story main area.</summary>
    public ILocator TextInputs =>
        _page.Locator("main input[type='text'], main input:not([type])");

    /// <summary>Textareas inside the story main area.</summary>
    public ILocator Textareas => _page.Locator("main textarea");

    /// <summary>All data-slot elements anywhere on the page (component markers).</summary>
    public ILocator DataSlot(string slot) =>
        _page.Locator($"[data-slot='{slot}']");
}
