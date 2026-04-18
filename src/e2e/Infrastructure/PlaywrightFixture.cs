using Microsoft.Playwright;

namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// xUnit class fixture that owns a single Playwright <see cref="IBrowser"/>
/// instance shared across every test in a collection. Each test gets its
/// own <see cref="IBrowserContext"/> for isolation.
/// Chromium is downloaded automatically on first use.
/// </summary>
public sealed class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        // Idempotent — skips download when the binary is already cached.
        Microsoft.Playwright.Program.Main(["install", "chromium", "--with-deps"]);

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null)
            await Browser.CloseAsync();
        _playwright?.Dispose();
    }
}
