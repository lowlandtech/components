using LowlandTech.Components.E2E.CPKG0001.Packaging;

namespace LowlandTech.Components.E2E.CBLK0002.Sessions.US01.Reachability;

/// <summary>
/// SC01 — Reach the plugin tab of the session surface, browser half.
/// </summary>
/// <remarks>
/// <para>
/// The consumer application of the session route restores the published batch
/// of the shared feed, so the browser proves the packages that are published
/// now and not a fresh pack of the sources of this repository. The route is
/// served and a real pointer clicks the tab of the plugin surface in the right
/// pane of the session surface.
/// </para>
/// <para>
/// The measurement of the tab and of the right pane repeats the measurement
/// of the Harness lane: a tab that overflows the right pane ends under the
/// close button of the pane, and a real pointer never reaches it.
/// </para>
/// </remarks>
[Trait(Spec.SPEC, "CBLK0002")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A consumer reaches the plugin tab of the right pane of the session surface with a real pointer")]
[Collection(SessionRouteCollection.Name)]
public sealed class SC01_PluginTabReachabilityE2E(ITestOutputHelper output)
{
    [Fact(DisplayName = "UAC04: a real pointer reaches the plugin tab of the right pane of the packaged session route")]
    [Trait(Spec.UAC, "04")]
    [Given("the packaged session route that restored the published batch of the shared feed")]
    [When("a real pointer clicks the plugin tab of the right pane")]
    [Then("the click reaches the tab instead of timing out")]
    public async Task UAC04_RealPointerReachesThePluginTab()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(
            repositoryRoot,
            "src/test/CBLK0002.Sessions/consumer",
            "cblk0002-e2e-",
            usePublishedBatch: true);

        scenario.UsePublishedBatch.ShouldBeTrue(
            "The scenario did not consume the published batch of the shared feed.");
        scenario.BatchVersion.ShouldNotBeNullOrWhiteSpace(
            "The scenario resolved no published batch of the shared feed.");
        scenario.RestoreExitCode.ShouldBe(
            0, $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(
            0, $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(
            0, $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        // The published batch and the Core package it pins resolve without a
        // single diagnostic, so the route serves the batch that is published
        // now and not a batch that pins another Core.
        scenario.ReadLog().Contains("NU1608").ShouldBeFalse(
            $"The published batch '{scenario.BatchVersion}' and the Core package " +
            $"'{scenario.CoreVersion}' do not resolve cleanly against each other.");

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

        // The circuit of the route has to be live before a pointer reaches the
        // surface, otherwise the click lands on the prerendered markup and no
        // handler of the surface receives it.
        await page.Locator("[data-testid='consumer-live']").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        var pluginTab = page.Locator(
            "[data-slot='session-block-right-sidebar-tab'][data-tab-key='plugins']");

        await pluginTab.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        var rightPane = page.Locator("[data-slot='session-block-right-sidebar']");

        await rightPane.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // The tab of the plugin surface lies inside the right pane. A tab that
        // overflows the pane ends under the close button of the pane, and a
        // real pointer never reaches it.
        var tabBox = await pluginTab.BoundingBoxAsync();
        var paneBox = await rightPane.BoundingBoxAsync();

        tabBox.ShouldNotBeNull("The plugin tab of the right pane has no box in the rendered route.");
        paneBox.ShouldNotBeNull("The right pane of the session surface has no box in the rendered route.");

        var tabX = (double)tabBox!.X;
        var tabWidth = (double)tabBox.Width;
        var paneX = (double)paneBox!.X;
        var paneWidth = (double)paneBox.Width;

        tabWidth.ShouldBeGreaterThan(
            0, "The plugin tab of the right pane has no box in the rendered route.");
        paneWidth.ShouldBeGreaterThan(
            0, "The right pane of the session surface has no box in the rendered route.");

        var tabRight = tabX + tabWidth;
        var paneRight = paneX + paneWidth;

        tabX.ShouldBeGreaterThanOrEqualTo(
            paneX - 0.5,
            "The plugin tab of the right pane starts outside of the right pane.");
        tabRight.ShouldBeLessThanOrEqualTo(
            paneRight + 0.5,
            "The plugin tab of the right pane overflows the right pane, so a real pointer " +
            "reaches the close button of the pane instead of the tab.");

        output.WriteLine(
            $"the plugin tab of the right pane spans x={tabX}..{tabRight} of a right pane " +
            $"that spans x={paneX}..{paneRight}; the batch of the route is " +
            $"'{scenario.BatchVersion}' against the Core '{scenario.CoreVersion}'");

        // The pointer of a real browser clicks the tab. Playwright refuses the
        // click when another element covers the point of the pointer, so a
        // timeout here is the defect of the surface.
        await pluginTab.ClickAsync(new LocatorClickOptions { Timeout = 15_000 });

        // The click reached the tab: the surface shows the panel of the plugin
        // surface and the tab itself carries the active state.
        var pluginPanel = page.Locator("[data-slot='session-block-right-sidebar-plugin-panel']");

        await pluginPanel.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        (await pluginPanel.Locator("[data-testid='consumer-plugin-surface']").CountAsync()).ShouldBe(
            1, "The click on the plugin tab did not reach the tab of the right pane.");

        var tabClass = await pluginTab.GetAttributeAsync("class");
        (tabClass ?? string.Empty).Contains("bg-accent").ShouldBeTrue(
            "The plugin tab of the right pane carries no active state after the click.");
    }
}
