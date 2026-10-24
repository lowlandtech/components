namespace LowlandTech.Components.E2E.CPKG0001.Packaging;

/// <summary>
/// SC01 — Independent package consumption, browser half.
///
/// The consumer application that restored the Components packages from an
/// offline feed is published and served. A real browser then checks that a
/// representative control and a representative session block render from the
/// packages, that the <c>_content</c> assets of those packages resolve over
/// HTTP, and that no Node or toolchain input survived into the served output.
/// </summary>
[Trait(Spec.SPEC, "CPKG0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "An isolated application consumes the Components packages without a Foundry checkout")]
// The packaging scenario packs, publishes and serves an application while the
// Showcode scenarios drive a real browser against the demo host. Running both
// at once starves the demo host of the resources it needs to hydrate, so this
// scenario shares the Showcode collection and runs beside it, never with it.
[Collection(ShowcodeCollection.Name)]
public sealed class SC01_IndependentPackageConsumptionE2E
{
    [Fact(DisplayName = "UAC02: the published consumer renders a control and a session block, resolves _content assets and serves no Node input")]
    [Trait(Spec.UAC, "02")]
    [Given("the consumer that restored the Components packages from the offline feed")]
    [When("it publishes and renders a representative control and session block")]
    [Then("its _content assets resolve and Node/toolchain inputs are absent from the served output")]
    public async Task UAC02_ThePublishedConsumerRendersThePackagesWithoutToolchainInputs()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(repositoryRoot);

        scenario.RestoreExitCode.ShouldBe(0,
            $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(0,
            $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.PublishExitCode.ShouldBe(0,
            $"The consumer publish failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        var webRoot = Path.Combine(scenario.PublishDirectory, "wwwroot");
        Directory.Exists(webRoot).ShouldBeTrue(
            $"The published consumer has no web root at '{webRoot}'.");

        await using var host = await ConsumerAppHost.StartAsync(scenario.PublishDirectory);

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

        // A representative control, rendered by the controls package.
        var control = page.Locator("[data-slot='button']");
        await control.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });
        (await control.CountAsync()).ShouldBeGreaterThan(0,
            "The published consumer rendered no control from the controls package.");

        // A representative session block, rendered by the Blocks package.
        var sessionBlock = page.Locator("[data-slot='session-card']");
        await sessionBlock.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });
        (await sessionBlock.CountAsync()).ShouldBeGreaterThan(0,
            "The published consumer rendered no session block from the Blocks package.");

        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var cancellation = TestContext.Current.CancellationToken;

        // The _content assets of the packages resolve over HTTP.
        var stylesheet = await http.GetAsync(
            $"{host.BaseUrl}/_content/LowlandTech.Components/css/components.css", cancellation);
        stylesheet.StatusCode.ShouldBe(HttpStatusCode.OK,
            "The _content stylesheet of the controls package did not resolve.");
        (await stylesheet.Content.ReadAsStringAsync(cancellation)).Length.ShouldBeGreaterThan(0,
            "The _content stylesheet of the controls package resolved to an empty file.");

        // Node and toolchain inputs never reach the served output.
        var contentRoot = Path.Combine(webRoot, "_content", "LowlandTech.Components");
        Directory.Exists(Path.Combine(contentRoot, "node_modules")).ShouldBeFalse(
            "The served output carries the Node modules of the controls package.");
        File.Exists(Path.Combine(contentRoot, "package.json")).ShouldBeFalse(
            "The served output carries the package.json of the controls package.");
        File.Exists(Path.Combine(contentRoot, "package-lock.json")).ShouldBeFalse(
            "The served output carries the package-lock.json of the controls package.");
        File.Exists(Path.Combine(contentRoot, "css", "input.css")).ShouldBeFalse(
            "The served output carries the Tailwind input stylesheet of the controls package.");

        var packageManifest = await http.GetAsync(
            $"{host.BaseUrl}/_content/LowlandTech.Components/package.json", cancellation);
        packageManifest.StatusCode.ShouldBe(HttpStatusCode.NotFound,
            "The served output answers with a Node package manifest.");
    }
}
