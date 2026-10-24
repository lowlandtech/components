namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// Base class for all Showcode E2E test classes. Owns one
/// <see cref="IBrowserContext"/> and <see cref="ShowcodePage"/> per test
/// class instance (xUnit creates a new instance per test method).
/// </summary>
[Collection(ShowcodeCollection.Name)]
public abstract class ShowcodeScenario : IAsyncLifetime
{
    private readonly ShowcodeCollection _collection;
    protected IBrowserContext Context = null!;
    protected ShowcodePage Showcode = null!;

    protected ShowcodeScenario(ShowcodeCollection collection)
    {
        _collection = collection;
    }

    public async ValueTask InitializeAsync()
    {
        Context = await _collection.Playwright.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            // Viewport wide enough for the sidebar + content.
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
        });

        var page = await Context.NewPageAsync();

        // Navigate directly to the Showcode page — the root URL renders the
        // home page which has no sidebar, causing WaitForSelector to timeout.
        var showcodeUrl = _collection.Process.BaseUrl + "/components";
        Showcode = new ShowcodePage(page, showcodeUrl);

        // Load the Showcode page and wait for Blazor WASM to hydrate.
        // NetworkIdle gives the WASM bundle time to download and boot before
        // we check for DOM elements — without it the selector times out on
        // cold starts (NCrunch, first run, slow machines).
        await page.GotoAsync(showcodeUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
            new PageWaitForLoadStateOptions { Timeout = 120_000 });
        await Showcode.EnsureSidebarExpandedAsync();
        await page.WaitForSelectorAsync("[data-slot='sidebar-menu-button']",
            new PageWaitForSelectorOptions { Timeout = 60_000 });
    }

    public async ValueTask DisposeAsync()
    {
        await Context.CloseAsync();
    }
}
