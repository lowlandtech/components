using LowlandTech.Components.E2E.CPKG0001.Packaging;

namespace LowlandTech.Components.E2E.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// SC01 — Resolve active plugin pages, browser half.
///
/// <para>
/// The consumer application that restored the Components packages and the
/// optional page feature package from an offline feed is published and served.
/// It declares the route of a plugin owner in its activation snapshot, publishes
/// the page of that owner through the Components page owner bridge and renders
/// the navigation of the shell route gateway. Two browsers open the consumer, so
/// two circuits are live, and the owner withdraws while both are open.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPGS0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers expose the plugin pages and the navigation of active plugin owners only, and keep the host routes reserved")]
// The scenario publishes and serves its own consumer application, so it never
// boots the Showcode WASM demo host: the consumer proof of this scope must not
// depend on the demo host. It carries its own collection, which disables
// parallelization, so it never runs beside the Showcode scenarios that drive
// a real browser against that demo host.
[Collection(CPGS0001Collection.Name)]
public sealed class SC01_ActivePluginPagesE2E
{
    private const string SymbolicIdentity = "Consumer.PluginPage";

    /// <summary>
    /// The consumer application the scenario serves. Its output belongs to
    /// every failure message, so a failing browser state can be read against
    /// what the host wrote.
    /// </summary>
    private static ConsumerAppHost? _host;

    /// <summary>
    /// The messages the browser wrote. They belong to every failure message,
    /// so a failed browser state can be read against what the page reported.
    /// </summary>
    private static readonly List<string> _browserMessages = [];

    [Fact(DisplayName = "UAC03: the withdrawal of the owner removes the navigation entry and the direct route of the page in two live circuits, the reserved host route stays available and a stale projection cannot authorize the protected action")]
    [Trait(Spec.UAC, "03")]
    [Given("a published consumer that renders the page of an active plugin owner through the shell route gateway, beside the navigation of that gateway and a reserved host route")]
    [When("a second consumer opens the same host and the owner withdraws")]
    [Then("the navigation entry and the direct route of the page are gone in both circuits, the reserved host route stays available and the request the stale projection holds is refused")]
    public async Task UAC03_TheWithdrawalRemovesThePageInTwoCircuits()
    {
        try
        {
            await ProveAsync();
        }
        catch (Exception exception)
        {
            // The output of the consumer belongs to every failure: it names the
            // exception the host wrote while the browser drove it.
            throw new InvalidOperationException(
                $"{exception.Message}{Environment.NewLine}{_host?.Output ?? string.Empty}",
                exception);
        }
    }

    private async Task ProveAsync()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(
            repositoryRoot,
            "src/test/CPGS0001.Integration/consumer",
            "cpgs0001-e2e-",
            additionalPackages:
            [
                "src/features/LowlandTech.Components.Features.Pages/LowlandTech.Components.Features.Pages.csproj",
            ],
            feedPackages:
            [
                "LowlandTech.Plugins.Blazor",
            ]);

        scenario.RestoreExitCode.ShouldBe(0,
            $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(0,
            $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(0,
            $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        await using var host = await ConsumerAppHost.StartAsync(
            scenario.PublishDirectory,
            "LowlandTech.PageConsumer.dll");

        _host = host;

        Microsoft.Playwright.Program.Main(["install", "chromium"]);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        await using var pageContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
        });

        await using var navigationContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
        });

        // The first circuit opens the retained route of the plugin page, the
        // second one opens the host page that carries the navigation.
        var page = await pageContext.NewPageAsync();
        var navigation = await navigationContext.NewPageAsync();

        page.Console += (_, message) =>
            _browserMessages.Add($"console {message.Type}: {message.Text}");

        page.PageError += (_, error) =>
            _browserMessages.Add($"page error: {error}");

        await page.GotoAsync($"{host.BaseUrl}/plugin/reports");
        await navigation.GotoAsync(host.BaseUrl);

        // Both circuits are live before the owner withdraws. A circuit that is
        // not live yet has no handler, so the withdrawal would never reach it.
        await Live(page).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await Live(navigation).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // The route of the active owner resolves to its page, and the metadata
        // the page declares reaches the navigation of the gateway.
        await WaitUntilAsync(
            () => PluginPageVisibleAsync(page),
            "The first consumer did not render the page of the active owner.");

        (await PluginPageLabelAsync(page)).ShouldBe("Reports",
            "The page of the active owner does not carry the label it declares.");

        await WaitUntilAsync(
            () => NavigationEntryVisibleAsync(navigation),
            "The second consumer did not offer the page of the active owner in the navigation.");

        (await NavigationEntryComponentAsync(navigation)).ShouldBe(SymbolicIdentity,
            "The navigation entry does not belong to the page of the active owner.");

        // The protected action of the page is authorized by the server while
        // the owner is active. The request is captured, so the same request
        // can be replayed after the withdrawal.
        var actionRequest = page.WaitForRequestAsync("**/plugin-action");

        await PluginPageAction(page).ClickAsync();

        await WaitUntilAsync(
            () => ActionReportedAsync(page),
            "The protected action of the page did not report its outcome.");

        (await ActionResultAsync(page)).ShouldBe("granted",
            "The protected action of the page was not authorized while the owner was active.");

        var captured = await actionRequest;

        // The host withdraws the owner of the page.
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var withdrawal = await http.PostAsync(
            $"{host.BaseUrl}/withdraw",
            null,
            TestContext.Current.CancellationToken);

        withdrawal.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the withdrawal of the owner.");

        // The direct route of the withdrawn owner reaches the host fallback.
        await WaitUntilAsync(
            () => FallbackVisibleAsync(page),
            "The first consumer did not fall back to the host after the withdrawal.");

        (await PluginPageVisibleAsync(page)).ShouldBeFalse(
            "The first consumer still renders the page of the withdrawn owner.");

        // The navigation of the gateway no longer offers the page.
        await WaitUntilAsync(
            () => NavigationEntryGoneAsync(navigation),
            "The second consumer still offers the page of the withdrawn owner in the navigation.");

        // The route the host reserved for itself stays available.
        (await ReservedRouteAsync(navigation)).ShouldBe("/",
            "The second consumer lost the reserved host route after the withdrawal.");

        var reserved = await http.GetAsync(
            host.BaseUrl,
            TestContext.Current.CancellationToken);

        reserved.StatusCode.ShouldBe(HttpStatusCode.OK,
            "The reserved host route is no longer served after the withdrawal.");

        // The request the stale browser projection holds is refused by the
        // server: the owner is no longer active, so no projection authorizes
        // the action.
        var replay = await page.APIRequest.PostAsync(captured.Url);

        replay.Status.ShouldBe((int)HttpStatusCode.Forbidden,
            "The request of the stale browser projection was authorized after the withdrawal.");

        // The browser itself replays the action from its own projection and is
        // refused as well.
        var replayed = await page.EvaluateAsync<string>(
            "async () => await window.consumerPluginAction()");

        replayed.ShouldBe("denied",
            "The browser replayed the action of the stale projection and was authorized.");

        // The host activates the owner again: the page and its navigation entry
        // come back, so the withdrawal did not tear the consumer down.
        var activation = await http.PostAsync(
            $"{host.BaseUrl}/activate",
            null,
            TestContext.Current.CancellationToken);

        activation.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the reactivation of the owner.");

        await WaitUntilAsync(
            () => PluginPageVisibleAsync(page),
            "The first consumer did not restore the page of the reactivated owner.");

        await WaitUntilAsync(
            () => NavigationEntryVisibleAsync(navigation),
            "The second consumer did not restore the navigation entry of the reactivated owner.");
    }

    private static ILocator Live(IPage page) =>
        page.Locator("[data-testid='consumer-live']");

    private static ILocator PluginPage(IPage page) =>
        page.Locator("[data-testid='consumer-plugin-page']");

    private static ILocator PluginPageAction(IPage page) =>
        page.Locator("[data-testid='plugin-page-action']");

    private static ILocator Fallback(IPage page) =>
        page.Locator("[data-testid='host-fallback']");

    private static ILocator NavigationEntry(IPage page) =>
        page.Locator("[data-slot='plugins-navigation-link']");

    private static async Task<bool> PluginPageVisibleAsync(IPage page) =>
        await PluginPage(page).CountAsync() > 0;

    private static async Task<bool> FallbackVisibleAsync(IPage page) =>
        await Fallback(page).CountAsync() > 0;

    private static async Task<bool> NavigationEntryVisibleAsync(IPage page) =>
        await NavigationEntry(page).CountAsync() > 0;

    private static async Task<bool> NavigationEntryGoneAsync(IPage page) =>
        await NavigationEntry(page).CountAsync() == 0;

    private static async Task<string> PluginPageLabelAsync(IPage page) =>
        (await page.Locator("[data-testid='plugin-page-label']").InnerTextAsync()).Trim();

    private static async Task<string> NavigationEntryComponentAsync(IPage page) =>
        await NavigationEntry(page).First.GetAttributeAsync("data-component") ?? string.Empty;

    private static async Task<string> ActionResultAsync(IPage page) =>
        (await page.Locator("[data-testid='plugin-page-action-result']").InnerTextAsync()).Trim();

    private static async Task<bool> ActionReportedAsync(IPage page) =>
        await ActionResultAsync(page) is not "none";

    private static async Task<string> ReservedRouteAsync(IPage page) =>
        (await page.Locator("[data-testid='host-reserved-route']").InnerTextAsync()).Trim();

    /// <summary>
    /// Waits until the browser shows the expected state. The page of a plugin
    /// owner reaches the document in the render batch that follows the
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

        throw new TimeoutException(
            $"{failureMessage}{Environment.NewLine}{string.Join(Environment.NewLine, _browserMessages)}" +
            $"{Environment.NewLine}{_host?.Output ?? string.Empty}");
    }
}
