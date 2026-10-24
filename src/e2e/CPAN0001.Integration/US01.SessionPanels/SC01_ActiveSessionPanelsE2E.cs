using LowlandTech.Components.E2E.CPKG0001.Packaging;

namespace LowlandTech.Components.E2E.CPAN0001.Integration.US01.SessionPanels;

/// <summary>
/// SC01 — Project active session panels, browser half.
///
/// <para>
/// The consumer application that restored the Components packages from an
/// offline feed is published and served. It renders the session plugin host of
/// the Blocks package and contributes one panel for one plugin owner. Two
/// browsers open the consumer, so two circuits are live, and the host
/// withdraws the owner while both are open.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPAN0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers render session panels only for active plugin owners through a reusable optional bridge")]
// The scenario publishes and serves its own consumer application, so it never
// boots the Showcode WASM demo host: the consumer proof of this scope must not
// depend on the demo host. It carries its own collection, which disables
// parallelization, so it never runs beside the Showcode scenarios that drive
// a real browser against that demo host.
[Collection(CPAN0001Collection.Name)]
public sealed class SC01_ActiveSessionPanelsE2E
{
    [Fact(DisplayName = "UAC03: two live circuits drop the panel of a withdrawn owner and show the host fallback")]
    [Trait(Spec.UAC, "03")]
    [Given("a consumer that restored the Components packages from an offline feed and renders the session plugin host of an active owner")]
    [When("a second consumer opens the same host and the owner withdraws")]
    [Then("both consumers drop the panel of the withdrawn owner and show the host fallback")]
    public async Task UAC03_BothConsumersDropThePanelOfTheWithdrawnOwner()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(
            repositoryRoot,
            "src/test/CPAN0001.Integration/consumer",
            "cpan0001-e2e-");

        scenario.RestoreExitCode.ShouldBe(0,
            $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(0,
            $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(0,
            $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        await using var host = await ConsumerAppHost.StartAsync(
            scenario.PublishDirectory,
            "LowlandTech.Components.SessionConsumer.dll");

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
        // not live yet has no handler, and the bridge admits no handler while
        // no owner is active, so the withdrawal would never reach it.
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

        // Both circuits render the panel of the active owner.
        await PanelOf(first).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await PanelOf(second).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // The host withdraws the owner of the panel.
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var withdrawal = await http.PostAsync(
            $"{host.BaseUrl}/withdraw",
            null,
            TestContext.Current.CancellationToken);

        withdrawal.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the withdrawal of the owner.");

        // Both circuits drop the panel and show the host fallback.
        await first.Locator("[data-slot='session-plugin-host-empty']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await second.Locator("[data-slot='session-plugin-host-empty']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        (await first.Locator("[data-slot='session-plugin-tab']").CountAsync()).ShouldBe(0,
            "The first consumer still renders a panel of the withdrawn owner.");

        (await second.Locator("[data-slot='session-plugin-tab']").CountAsync()).ShouldBe(0,
            "The second consumer still renders a panel of the withdrawn owner.");

        // The host activates the owner again: the panel comes back, so the
        // withdrawal did not tear the consumer down.
        var activation = await http.PostAsync(
            $"{host.BaseUrl}/activate",
            null,
            TestContext.Current.CancellationToken);

        activation.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the reactivation of the owner.");

        await PanelOf(first).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        await PanelOf(second).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });
    }

    private static ILocator Live(IPage page) =>
        page.Locator("[data-testid='consumer-live']");

    private static ILocator PanelOf(IPage page) =>
        page.Locator("[data-slot='session-plugin-tab'][data-plugin-key='consumer.panel']");
}
