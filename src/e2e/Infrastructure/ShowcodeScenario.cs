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
        Showcode = new ShowcodePage(page, _collection.Process.BaseUrl);

        // Load the app for the first time.
        await page.GotoAsync(_collection.Process.BaseUrl);
        await page.WaitForSelectorAsync("[data-slot='sidebar-menu-button']",
            new PageWaitForSelectorOptions { Timeout = 30_000 });
    }

    public async ValueTask DisposeAsync()
    {
        await Context.CloseAsync();
    }
}
