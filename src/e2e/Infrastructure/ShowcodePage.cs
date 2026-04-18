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

        // Click the sidebar entry for this story.
        var btn = _page.Locator("[data-slot='sidebar-menu-button']")
                       .Filter(new LocatorFilterOptions { HasText = storyName });
        await btn.First.ClickAsync();

        // Wait for Blazor to re-render.
        await _page.WaitForTimeoutAsync(600);
    }

    // ── Locator helpers ──────────────────────────────────────────────────────

    public ILocator Main => _page.Locator("main");

    /// <summary>Buttons rendered by the LT Button component (not sidebar).</summary>
    public ILocator ComponentButtons =>
        _page.Locator("main [data-slot='button']");

    public ILocator WithText(string text) =>
        _page.Locator($"text={text}");

    public ILocator ByTestId(string testId) =>
        _page.GetByTestId(testId);

    /// <summary>Checks or checkboxes inside the story main area.</summary>
    public ILocator Checkboxes =>
        _page.Locator("main input[type='checkbox']");

    /// <summary>Radio inputs inside the story main area.</summary>
    public ILocator RadioInputs =>
        _page.Locator("main input[type='radio']");

    /// <summary>Range inputs (sliders) inside the story main area.</summary>
    public ILocator RangeInputs =>
        _page.Locator("main input[type='range']");

    /// <summary>Text inputs inside the story main area.</summary>
    public ILocator TextInputs =>
        _page.Locator("main input[type='text'], main input:not([type])");

    /// <summary>Textareas inside the story main area.</summary>
    public ILocator Textareas => _page.Locator("main textarea");

    /// <summary>All data-slot elements anywhere on the page (component markers).</summary>
    public ILocator DataSlot(string slot) =>
        _page.Locator($"[data-slot='{slot}']");
}
