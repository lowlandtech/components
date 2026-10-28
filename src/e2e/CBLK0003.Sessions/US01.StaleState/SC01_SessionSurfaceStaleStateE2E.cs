using LowlandTech.Components.E2E.CPKG0001.Packaging;

namespace LowlandTech.Components.E2E.CBLK0003.Sessions.US01.StaleState;

/// <summary>
/// SC01 — Reset a stale state of the session surface, browser half.
/// </summary>
/// <remarks>
/// <para>
/// The consumer application of the session route restores the published batch
/// of the shared feed, so the browser proves the packages that are published
/// now. The route is served, the stale collapse state of the two side panes
/// is seeded in the storage of the browser, and the route is reloaded.
/// </para>
/// <para>
/// A stale state is a state that leaves the surface in a single panel view.
/// The surface recovers its three panels and its two handles, and it repairs
/// the stored state, so the recovery holds after the next reload as well.
/// </para>
/// </remarks>
[Trait(Spec.SPEC, "CBLK0003")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A consumer recovers the session surface from a stale persisted state")]
[Collection(SessionRouteCollection.Name)]
public sealed class SC01_SessionSurfaceStaleStateE2E
{
    private const string SidebarPersistKey = "session-block:sidebar-open";
    private const string RightSidebarPersistKey = "session-block:right-sidebar-open";

    [Fact(DisplayName = "UAC04: the packaged session route recovers its panels and its handles from a stale state")]
    [Trait(Spec.UAC, "04")]
    [Given("the packaged session route that restored the published batch of the shared feed")]
    [When("the route is reloaded with the stale state seeded")]
    [Then("it renders the three panels and the two handles and every linked asset answers 200")]
    public async Task UAC04_TheRouteRecoversFromTheStaleState()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(
            repositoryRoot,
            "src/test/CBLK0002.Sessions/consumer",
            "cblk0003-e2e-",
            usePublishedBatch: true);

        scenario.UsePublishedBatch.ShouldBeTrue(
            "The scenario did not consume the published batch of the shared feed.");
        scenario.RestoreExitCode.ShouldBe(
            0, $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(
            0, $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(
            0, $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        await using var host = await ConsumerAppHost.StartAsync(
            scenario.PublishDirectory,
            "LowlandTech.Components.SessionRouteConsumer.dll");

        Microsoft.Playwright.Program.Main(["install", "chromium"]);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync(host.BaseUrl);

        // The route is live before the stale state is seeded, so the seed
        // reaches the storage of the browser and not a page that never ran.
        await Surface(page).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await page.Locator("[data-testid='consumer-live']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // The stale state: the collapse state of the two side panes together
        // would leave the surface in a single panel view.
        await page.EvaluateAsync(
            $"() => {{ localStorage.setItem('{SidebarPersistKey}', 'false'); " +
            $"localStorage.setItem('{RightSidebarPersistKey}', 'false'); }}");

        // The route of the acceptance holds a live circuit open, so the reload
        // waits for the document and not for a network that never goes idle.
        await page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.Load });

        // The surface repairs the stored state when it recovers from the stale
        // state, and the repair is what the next reload reads.
        await page.WaitForFunctionAsync(
            $"() => localStorage.getItem('{SidebarPersistKey}') === 'true' " +
            $"&& localStorage.getItem('{RightSidebarPersistKey}') === 'true'",
            null,
            new PageWaitForFunctionOptions { Timeout = 60_000 });

        // The surface recovers its three panels and its two handles.
        await AssertSurfaceIsRecoveredAsync(page);

        // The stored state is repaired, so the recovery holds after the next
        // reload without the user clearing the storage of the browser.
        var stored = await page.EvaluateAsync<string[]>(
            $"() => [localStorage.getItem('{SidebarPersistKey}'), localStorage.getItem('{RightSidebarPersistKey}')]");

        stored[0].ShouldBe("true",
            "The surface did not repair the stored collapse state of the left pane.");
        stored[1].ShouldBe("true",
            "The surface did not repair the stored collapse state of the right pane.");

        // The route of the acceptance holds a live circuit open, so the reload
        // waits for the document and not for a network that never goes idle.
        await page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.Load });

        await page.Locator("[data-testid='consumer-live']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await AssertSurfaceIsRecoveredAsync(page);

        // Every linked asset of the route answers 200.
        var assets = await page.EvaluateAsync<string[]>(
            "() => Array.from(document.querySelectorAll('link[href], script[src]'))" +
            ".map(element => element.getAttribute('href') ?? element.getAttribute('src') ?? '')");

        assets.ShouldNotBeEmpty("The packaged session route links no asset.");

        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        foreach (var asset in assets)
        {
            var response = await http.GetAsync(
                new Uri(new Uri(host.BaseUrl), asset),
                TestContext.Current.CancellationToken);

            response.StatusCode.ShouldBe(
                HttpStatusCode.OK, $"The linked asset '{asset}' of the packaged session route did not answer 200.");
        }
    }

    private static async Task AssertSurfaceIsRecoveredAsync(IPage page)
    {
        foreach (var panel in new[]
                 {
                     "session-block-sidebar-panel",
                     "session-block-main-panel",
                     "session-block-sections-panel",
                 })
        {
            var locator = page.Locator($"[data-slot='resizable-panel'][data-panel='{panel}']");

            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 30_000,
            });
        }

        var handles = page.Locator("[data-slot='resizable-handle']");

        // The surface carries one handle per recovered pane, so the first of
        // them is awaited and both of them are counted.
        await handles.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        (await handles.CountAsync()).ShouldBe(
            2, "The session surface does not render the two handles of its recovered panels.");
    }

    private static ILocator Surface(IPage page) =>
        page.Locator("[data-testid='consumer-session-surface']");
}
