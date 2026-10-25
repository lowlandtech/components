using LowlandTech.Components.E2E.CPKG0001.Packaging;

namespace LowlandTech.Components.E2E.CBLK0001.Integration.US01.ActiveOwnedBlocks;

/// <summary>
/// SC01 — Resolve active owned session blocks, browser half.
///
/// <para>
/// The consumer application that restored the Components packages from an
/// offline feed is published and served. It registers a host default
/// registry and the assembly-scanning fallback, publishes the block of an
/// active plugin owner through the bridge and resolves the blocks of the
/// circuit. Two browsers open the consumer, so two circuits are live, and the
/// owner withdraws while both are open.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CBLK0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers resolve only the session blocks of active plugin owners through a reusable optional bridge")]
// The scenario publishes and serves its own consumer application, so it never
// boots the Showcode WASM demo host: the consumer proof of this scope must not
// depend on the demo host. It carries its own collection, which disables
// parallelization, so it never runs beside the Showcode scenarios that drive
// a real browser against that demo host.
[Collection(CBLK0001Collection.Name)]
public sealed class SC01_ActiveOwnedBlocksE2E
{
    private const string OwnedNodeTypeKey = "plugin.order";
    private const string HostDefaultTypeName = "SessionCard";

    [Fact(DisplayName = "UAC03: the withdrawal of the owner removes the owned block in two live circuits, the host default stays available and the deployed stylesheet is still served")]
    [Trait(Spec.UAC, "03")]
    [Given("a published consumer that renders the block of an active plugin owner beside the host default of the host")]
    [When("a second consumer opens the same host and the owner withdraws")]
    [Then("both circuits fall back to the host default, the owned block cannot reappear under its type name and the deployed stylesheet is still served")]
    public async Task UAC03_TheWithdrawalRemovesTheOwnedBlockInTwoCircuits()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(
            repositoryRoot,
            "src/test/CBLK0001.Integration/consumer",
            "cblk0001-e2e-");

        scenario.RestoreExitCode.ShouldBe(0,
            $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(0,
            $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(0,
            $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        await using var host = await ConsumerAppHost.StartAsync(
            scenario.PublishDirectory,
            "LowlandTech.BlockConsumer.dll");

        Microsoft.Playwright.Program.Main(["install", "chromium"]);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        await using var firstContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
        });

        await using var secondContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
        });

        var first = await firstContext.NewPageAsync();
        var second = await secondContext.NewPageAsync();

        await first.GotoAsync(host.BaseUrl);
        await second.GotoAsync(host.BaseUrl);

        // Both circuits are live before the owner withdraws. A circuit that is
        // not live yet has no handler, so the withdrawal would never reach it.
        await Live(first).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await Live(second).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // Both circuits resolve the block of the active owner beside the host
        // default, and the assembly-scanning shape of the owned type is closed.
        await WaitUntilAsync(
            () => OwnedBlockVisibleAsync(first),
            "The first consumer did not render the block of the active owner.");

        await WaitUntilAsync(
            () => OwnedBlockVisibleAsync(second),
            "The second consumer did not render the block of the active owner.");

        (await HostDefaultAsync(first)).ShouldBe(HostDefaultTypeName,
            "The first consumer does not resolve the host default beside the owned block.");
        (await HostDefaultAsync(second)).ShouldBe(HostDefaultTypeName,
            "The second consumer does not resolve the host default beside the owned block.");

        (await TypeNamePathAsync(first)).ShouldBe("none",
            "The first consumer resolves the owned type under its own type name.");
        (await TypeNamePathAsync(second)).ShouldBe("none",
            "The second consumer resolves the owned type under its own type name.");

        // The host withdraws the owner of the block.
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var withdrawal = await http.PostAsync(
            $"{host.BaseUrl}/withdraw",
            null,
            TestContext.Current.CancellationToken);

        withdrawal.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the withdrawal of the owner.");

        // Both circuits fall back to the host default and the owned block is
        // gone.
        await WaitUntilAsync(
            () => FallbackVisibleAsync(first),
            "The first consumer did not fall back to the host default.");

        await WaitUntilAsync(
            () => FallbackVisibleAsync(second),
            "The second consumer did not fall back to the host default.");

        (await OwnedBlockVisibleAsync(first)).ShouldBeFalse(
            "The first consumer still renders the block of the withdrawn owner.");
        (await OwnedBlockVisibleAsync(second)).ShouldBeFalse(
            "The second consumer still renders the block of the withdrawn owner.");

        (await HostDefaultAsync(first)).ShouldBe(HostDefaultTypeName,
            "The first consumer lost the host default after the withdrawal.");
        (await HostDefaultAsync(second)).ShouldBe(HostDefaultTypeName,
            "The second consumer lost the host default after the withdrawal.");

        (await TypeNamePathAsync(first)).ShouldBe("none",
            "The first consumer resolves the withdrawn type under its own type name.");
        (await TypeNamePathAsync(second)).ShouldBe("none",
            "The second consumer resolves the withdrawn type under its own type name.");

        // The deployed stylesheet of the package stays served and stays linked.
        (await HostStylesheetCountAsync(first)).ShouldBe(1,
            "The first consumer lost the stylesheet of the host.");
        (await HostStylesheetCountAsync(second)).ShouldBe(1,
            "The second consumer lost the stylesheet of the host.");

        var deployed = await http.GetAsync(
            $"{host.BaseUrl}/_content/LowlandTech.Components/css/components.css",
            TestContext.Current.CancellationToken);

        deployed.StatusCode.ShouldBe(HttpStatusCode.OK,
            "The deployed stylesheet of the package is no longer served after the withdrawal.");

        // The host activates the owner again: the block comes back, so the
        // withdrawal did not tear the consumer down.
        var activation = await http.PostAsync(
            $"{host.BaseUrl}/activate",
            null,
            TestContext.Current.CancellationToken);

        activation.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the reactivation of the owner.");

        await WaitUntilAsync(
            () => OwnedBlockVisibleAsync(first),
            "The first consumer did not restore the block of the reactivated owner.");

        await WaitUntilAsync(
            () => OwnedBlockVisibleAsync(second),
            "The second consumer did not restore the block of the reactivated owner.");
    }

    private static ILocator Live(IPage page) =>
        page.Locator("[data-testid='consumer-live']");

    private static ILocator OwnedBlock(IPage page) =>
        page.Locator("[data-testid='consumer-owned-block']");

    private static ILocator Fallback(IPage page) =>
        page.Locator("[data-testid='owned-block-fallback']");

    private static ILocator HostStylesheetLink(IPage page) =>
        page.Locator("link[data-slot='host-stylesheet']");

    private static async Task<bool> OwnedBlockVisibleAsync(IPage page) =>
        await OwnedBlock(page).CountAsync() > 0;

    private static async Task<bool> FallbackVisibleAsync(IPage page) =>
        await Fallback(page).CountAsync() > 0;

    private static async Task<string> HostDefaultAsync(IPage page) =>
        (await page.Locator("[data-testid='host-default']").InnerTextAsync()).Trim();

    private static async Task<string> TypeNamePathAsync(IPage page) =>
        (await page.Locator("[data-testid='type-name-path']").InnerTextAsync()).Trim();

    private static async Task<int> HostStylesheetCountAsync(IPage page) =>
        await HostStylesheetLink(page).CountAsync();

    /// <summary>
    /// Waits until the browser shows the expected state. The block of a
    /// provider reaches the document in the render batch that follows the
    /// withdrawal, so the proof waits for the state instead of reading it once.
    /// </summary>
    private static async Task WaitUntilAsync(
        Func<Task<bool>> condition,
        string failureMessage,
        int timeoutMilliseconds = 30_000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

        while (DateTime.UtcNow < deadline)
        {
            if (await condition())
            {
                return;
            }

            await Task.Delay(250);
        }

        throw new TimeoutException(failureMessage);
    }
}
