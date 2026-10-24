using LowlandTech.Components.E2E.CPKG0001.Packaging;

namespace LowlandTech.Components.E2E.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// SC01 — Resolve active provider-scoped themes, browser half.
///
/// <para>
/// The consumer application that restored the Components packages from an
/// offline feed is published and served. It registers a host theme and a theme
/// a plugin owner contributes, publishes the owned theme through the bridge,
/// selects it in a provider boundary and renders the stylesheet selection of
/// that boundary. Two browsers open the consumer, so two circuits are live, and
/// the owner withdraws while both are open.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CTHE0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers resolve only the themes of active plugin owners, per provider boundary, and restore the host defaults after a withdrawal")]
// The scenario publishes and serves its own consumer application, so it never
// boots the Showcode WASM demo host: the consumer proof of this scope must not
// depend on the demo host. It carries its own collection, which disables
// parallelization, so it never runs beside the Showcode scenarios that drive
// a real browser against that demo host.
[Collection(CTHE0001Collection.Name)]
public sealed class SC01_ProviderThemesE2E
{
    private const string ProviderId = "consumer.provider";

    private const string HostThemeName = "consumer-host";
    private const string PluginThemeName = "consumer-plugin";

    private const string HostStyleFamily = "default";
    private const string PluginStyleFamily = "brand";

    private const string HostClass = "host-theme-class";
    private const string PluginClass = "plugin-theme-class";
    private const string ConsumerClass = "consumer-class";

    private const string PluginStylesheet = "_content/LowlandTech.Components/css/variables.css";
    private const string DeployedStylesheet = "_content/LowlandTech.Components/css/components.css";

    [Fact(DisplayName = "UAC03: the withdrawal of the owner restores the host defaults in two live circuits, detaches the plugin stylesheet and leaves the deployed stylesheet intact")]
    [Trait(Spec.UAC, "03")]
    [Given("a published consumer that renders a provider boundary with the theme of an active plugin owner and the stylesheet selection of that boundary")]
    [When("a second consumer opens the same host and the owner withdraws")]
    [Then("both circuits render the host defaults, the plugin stylesheet is detached and the deployed stylesheet is still served")]
    public async Task UAC03_TheWithdrawalRestoresTheHostDefaultsInTwoCircuits()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(
            repositoryRoot,
            "src/test/CTHE0001.Integration/consumer",
            "cthe0001-e2e-");

        scenario.RestoreExitCode.ShouldBe(0,
            $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(0,
            $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(0,
            $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        await using var host = await ConsumerAppHost.StartAsync(
            scenario.PublishDirectory,
            "LowlandTech.ThemeConsumer.dll");

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

        // Both circuits resolve the theme of the active owner, and both carry
        // the stylesheet selection of that boundary.
        await WaitUntilAsync(
            () => ShowsPluginThemeAsync(first),
            "The first consumer did not resolve the theme of the active owner.");

        await WaitUntilAsync(
            () => ShowsPluginThemeAsync(second),
            "The second consumer did not resolve the theme of the active owner.");

        (await ThemeNameAsync(first)).ShouldBe(PluginThemeName,
            "The first consumer does not render the name of the owned theme.");
        (await ThemeStyleAsync(first)).ShouldBe(PluginStyleFamily,
            "The first consumer does not render the style family of the owned theme.");
        (await ThemeStyleAsync(second)).ShouldBe(PluginStyleFamily,
            "The second consumer does not render the style family of the owned theme.");

        (await ButtonClassAsync(first)).ShouldContain(PluginClass, customMessage: "The first consumer does not resolve the class of the owned theme.");
        (await ButtonClassAsync(first)).ShouldContain(ConsumerClass, customMessage: "The first consumer lost the class of the consumer.");
        (await ButtonClassAsync(second)).ShouldContain(PluginClass, customMessage: "The second consumer does not resolve the class of the owned theme.");
        (await ButtonClassAsync(second)).ShouldContain(ConsumerClass, customMessage: "The second consumer lost the class of the consumer.");

        // The stylesheet selection of the boundary is attached, and the browser
        // really loaded the deployed file it names.
        (await PluginStylesheetCountAsync(first)).ShouldBe(1,
            "The first consumer does not carry the stylesheet selection of the owned theme.");
        (await PluginStylesheetCountAsync(second)).ShouldBe(1,
            "The second consumer does not carry the stylesheet selection of the owned theme.");

        (await PluginStylesheetHrefAsync(first)).ShouldBe(PluginStylesheet,
            "The first consumer renders another stylesheet than the owned theme names.");

        (await PluginStylesheetLoadedAsync(first)).ShouldBeTrue(
            "The browser did not load the deployed stylesheet of the owned theme.");
        (await PluginStylesheetLoadedAsync(second)).ShouldBeTrue(
            "The browser did not load the deployed stylesheet of the owned theme.");

        // The host withdraws the owner of the theme.
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var withdrawal = await http.PostAsync(
            $"{host.BaseUrl}/withdraw",
            null,
            TestContext.Current.CancellationToken);

        withdrawal.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the withdrawal of the owner.");

        // Both circuits render the host defaults again and lose the class of
        // the owned theme.
        await WaitUntilAsync(
            () => ShowsHostThemeAsync(first),
            "The first consumer did not restore the host defaults.");

        await WaitUntilAsync(
            () => ShowsHostThemeAsync(second),
            "The second consumer did not restore the host defaults.");

        (await ThemeNameAsync(first)).ShouldBe(HostThemeName,
            "The first consumer does not render the host theme again.");
        (await ThemeStyleAsync(first)).ShouldBe(HostStyleFamily,
            "The first consumer does not render the host style family again.");
        (await ThemeNameAsync(second)).ShouldBe(HostThemeName,
            "The second consumer does not render the host theme again.");
        (await ThemeStyleAsync(second)).ShouldBe(HostStyleFamily,
            "The second consumer does not render the host style family again.");

        (await ButtonClassAsync(first)).ShouldContain(HostClass, customMessage: "The first consumer does not resolve the class of the host theme.");
        (await ButtonClassAsync(first)).ShouldNotContain(PluginClass, customMessage: "The first consumer still resolves the class of the withdrawn theme.");
        (await ButtonClassAsync(second)).ShouldContain(HostClass, customMessage: "The second consumer does not resolve the class of the host theme.");
        (await ButtonClassAsync(second)).ShouldNotContain(PluginClass, customMessage: "The second consumer still resolves the class of the withdrawn theme.");

        // The stylesheet selection of the boundary is detached in both
        // circuits, while the deployed file of the host stays served and stays
        // linked.
        (await PluginStylesheetCountAsync(first)).ShouldBe(0,
            "The first consumer still carries the stylesheet selection of the withdrawn owner.");
        (await PluginStylesheetCountAsync(second)).ShouldBe(0,
            "The second consumer still carries the stylesheet selection of the withdrawn owner.");

        (await HostStylesheetCountAsync(first)).ShouldBe(1,
            "The first consumer lost the stylesheet of the host.");
        (await HostStylesheetCountAsync(second)).ShouldBe(1,
            "The second consumer lost the stylesheet of the host.");

        var deployed = await http.GetAsync(
            $"{host.BaseUrl}/{DeployedStylesheet}",
            TestContext.Current.CancellationToken);

        deployed.StatusCode.ShouldBe(HttpStatusCode.OK,
            "The deployed stylesheet of the package is no longer served after the withdrawal.");

        // The host activates the owner again: the theme and its stylesheet
        // selection come back, so the withdrawal did not tear the consumer
        // down.
        var activation = await http.PostAsync(
            $"{host.BaseUrl}/activate",
            null,
            TestContext.Current.CancellationToken);

        activation.StatusCode.ShouldBe(HttpStatusCode.NoContent,
            "The consumer did not accept the reactivation of the owner.");

        await WaitUntilAsync(
            () => ShowsPluginThemeAsync(first),
            "The first consumer did not restore the theme of the reactivated owner.");

        await WaitUntilAsync(
            () => ShowsPluginThemeAsync(second),
            "The second consumer did not restore the theme of the reactivated owner.");

        (await PluginStylesheetCountAsync(first)).ShouldBe(1,
            "The first consumer did not restore the stylesheet selection of the reactivated owner.");
        (await PluginStylesheetCountAsync(second)).ShouldBe(1,
            "The second consumer did not restore the stylesheet selection of the reactivated owner.");
    }

    private static ILocator Live(IPage page) =>
        page.Locator("[data-testid='consumer-live']");

    private static ILocator Provider(IPage page) =>
        page.Locator($"[data-theme-provider='{ProviderId}']");

    private static ILocator Button(IPage page) =>
        page.Locator("[data-slot='button']");

    private static ILocator PluginStylesheetLink(IPage page) =>
        page.Locator("link[data-slot='theme-stylesheet']");

    private static ILocator HostStylesheetLink(IPage page) =>
        page.Locator("link[data-slot='host-stylesheet']");

    private static async Task<string?> ThemeNameAsync(IPage page) =>
        await Provider(page).GetAttributeAsync("data-theme-name");

    private static async Task<string?> ThemeStyleAsync(IPage page) =>
        await Provider(page).GetAttributeAsync("data-theme-style");

    private static async Task<string> ButtonClassAsync(IPage page) =>
        await Button(page).GetAttributeAsync("class") ?? string.Empty;

    private static async Task<int> PluginStylesheetCountAsync(IPage page) =>
        await PluginStylesheetLink(page).CountAsync();

    private static async Task<int> HostStylesheetCountAsync(IPage page) =>
        await HostStylesheetLink(page).CountAsync();

    private static async Task<string?> PluginStylesheetHrefAsync(IPage page) =>
        await PluginStylesheetLink(page).GetAttributeAsync("href");

    /// <summary>
    /// Whether the browser really loaded the deployed stylesheet the selection
    /// of the boundary names. A link element whose sheet is missing was never
    /// fetched, so a link alone is no proof that the deployed file was served.
    /// </summary>
    private static Task<bool> PluginStylesheetLoadedAsync(IPage page) =>
        page.EvaluateAsync<bool>(
            "() => { const link = document.querySelector(\"link[data-slot='theme-stylesheet']\"); return !!link && !!link.sheet; }");

    private static async Task<bool> ShowsPluginThemeAsync(IPage page)
    {
        var name = await ThemeNameAsync(page);
        var style = await ThemeStyleAsync(page);
        var buttonClass = await ButtonClassAsync(page);

        return name == PluginThemeName
            && style == PluginStyleFamily
            && buttonClass.Contains(PluginClass, StringComparison.Ordinal)
            && buttonClass.Contains(ConsumerClass, StringComparison.Ordinal);
    }

    private static async Task<bool> ShowsHostThemeAsync(IPage page)
    {
        var name = await ThemeNameAsync(page);
        var style = await ThemeStyleAsync(page);
        var buttonClass = await ButtonClassAsync(page);

        return name == HostThemeName
            && style == HostStyleFamily
            && buttonClass.Contains(HostClass, StringComparison.Ordinal)
            && !buttonClass.Contains(PluginClass, StringComparison.Ordinal)
            && buttonClass.Contains(ConsumerClass, StringComparison.Ordinal);
    }

    /// <summary>
    /// Waits until the browser shows the expected state. The theme of a
    /// provider boundary reaches the document in the render batch that follows
    /// the withdrawal, so the proof waits for the state instead of reading it
    /// once.
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
