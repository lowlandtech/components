namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// xUnit collection fixture that boots the Showcode WASM app process once
/// and shares a single Playwright browser instance across all Showcode E2E
/// tests. The app starts on an ephemeral port; all test classes in the
/// <c>[Collection(ShowcodeCollection.Name)]</c> share this instance.
/// </summary>
public sealed class ShowcodeCollection : IAsyncLifetime
{
    public const string Name = "Showcode";

    public ShowcodeProcess Process { get; private set; } = null!;
    public PlaywrightFixture Playwright { get; } = new();

    public async ValueTask InitializeAsync()
    {
        // Start the browser first (fast) then the WASM server (slow).
        await Playwright.InitializeAsync();
        Process = await ShowcodeProcess.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Process.DisposeAsync();
        await Playwright.DisposeAsync();
    }
}

[CollectionDefinition(ShowcodeCollection.Name)]
public class ShowcodeCollectionDefinition : ICollectionFixture<ShowcodeCollection>;
