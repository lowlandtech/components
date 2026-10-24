using System.Text.RegularExpressions;

namespace LowlandTech.Components.Tests.CPKG0001.Packaging.US01.Packaging;

/// <summary>
/// SC01 — Independent package consumption.
///
/// A consumer application must be able to take the Core, controls and Blocks
/// packages out of a package feed and run against them without a Foundry
/// checkout: no project reference into this repository, no file resolved from
/// outside the checkout, and no Node or toolchain input left in the output.
///
/// The packaging work is performed by
/// <c>src/test/CPKG0001.Packaging/consumer-scenario.ps1</c>, which packs the
/// three packages from this repository, assembles a self-contained offline
/// feed, writes a temporary consumer and restores that consumer against the
/// feed alone with an isolated global-packages folder. The browser half of the
/// scenario — publishing the consumer and rendering it — lives in
/// <c>src/e2e/CPKG0001.Packaging</c>.
/// </summary>
[Trait(Spec.SPEC, "CPKG0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "An isolated application consumes the Components packages without a Foundry checkout")]
public class SC01_IndependentPackageConsumption(ConsumerScenarioFixture fixture)
    : IClassFixture<ConsumerScenarioFixture>
{
    private static readonly string[] PackedPackages =
    [
        "LowlandTech.Components.Core",
        "LowlandTech.Components",
        "LowlandTech.Components.Blocks",
    ];

    private static readonly string[] RepairedFixtureInputs =
    [
        "web.App.razor",
        "shell.App.razor",
        "shell.input.css",
        "app.App.razor",
        "desktop.index.html",
    ];

    [Fact(DisplayName = "UAC01: the packed packages restore into a temporary consumer without paths outside the checkout")]
    [Trait(Spec.UAC, "01")]
    [Given("an offline feed that carries the selected upstream dependencies and nothing else")]
    [When("the Core, controls and Blocks packages are packed and restored into a temporary consumer")]
    [Then("every referenced file and dependency resolves inside the checkout")]
    public void UAC01_PackagesRestoreIntoATemporaryConsumerWithoutPathsOutsideTheCheckout()
    {
        var scenario = fixture.Scenario;

        // The three packages were produced from this repository.
        foreach (var packageId in PackedPackages)
        {
            var package = Path.Combine(
                scenario.FeedDirectory, $"{packageId}.{scenario.PackageVersion}.nupkg");

            File.Exists(package).ShouldBeTrue(
                $"Expected '{packageId}' {scenario.PackageVersion} in the offline feed at '{package}'.");

            new FileInfo(package).Length.ShouldBeGreaterThan(
                0, $"'{package}' is empty, so it carries no real package output.");
        }

        // The compatible upstream Core package travels with the feed, so the
        // consumer never reaches for a network source.
        var corePackage = Path.Combine(
            scenario.FeedDirectory, $"LowlandTech.Core.{scenario.CoreVersion}.nupkg");
        File.Exists(corePackage).ShouldBeTrue(
            $"Expected the selected Core package {scenario.CoreVersion} in the offline feed.");

        // The consumer restored and built against that feed alone.
        scenario.RestoreExitCode.ShouldBe(0,
            $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(0,
            $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        // The consumer is a package consumer: it holds no project reference
        // into this repository.
        var consumerProject = Path.Combine(scenario.ConsumerDirectory, "Consumer.csproj");
        File.ReadAllText(consumerProject).ShouldNotContain("<ProjectReference",
            customMessage: "The consumer must consume the packages, never the source tree.");

        using var assets = JsonDocument.Parse(File.ReadAllText(scenario.AssetsFile));
        var root = assets.RootElement;

        // Every package folder the restore used lives inside the checkout.
        var packageFolders = root.GetProperty("packageFolders")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToList();

        packageFolders.Count.ShouldBe(1,
            "The consumer must restore into exactly one package folder.");
        packageFolders[0].ShouldStartWith(scenario.PackagesDirectory,
            customMessage: $"'{packageFolders[0]}' resolves outside the checkout.");

        // Every resolved package sits under that folder: the asset paths are
        // relative, so no absolute path and no parent traversal can escape it.
        foreach (var library in root.GetProperty("libraries").EnumerateObject())
        {
            if (library.Value.GetProperty("type").GetString() != "package")
            {
                continue;
            }

            var path = library.Value.GetProperty("path").GetString()!;
            path.ShouldNotContain(":",
                customMessage: $"'{library.Name}' resolved through an absolute path.");
            path.ShouldNotStartWith("..",
                customMessage: $"'{library.Name}' resolved outside the package folder.");
        }

        // The restore used local sources only, and the private feed among them.
        var sources = root.GetProperty("project").GetProperty("restore").GetProperty("sources")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToList();

        sources.ShouldNotBeEmpty("The restore recorded no package source.");
        sources.ShouldContain(scenario.FeedDirectory,
            "The consumer did not restore from the offline feed.");

        foreach (var source in sources)
        {
            source.ShouldNotStartWith("http",
                customMessage: "The consumer restore reached a network source.");
        }

        // Nothing in the restore points back at this repository.
        var restore = root.GetProperty("project").GetProperty("restore");
        restore.TryGetProperty("projectReferences", out var projectReferences)
            .ShouldBeFalse("The consumer must not reference projects from the source repository.");

        File.ReadAllText(scenario.AssetsFile).ShouldNotContain(fixture.RepositoryRoot,
            customMessage: "The consumer restore resolved a path inside the source repository.");
    }

    [Fact(DisplayName = "UAC03: the repaired fixture inputs are discoverable and no exclusion hides the packaging scenario")]
    [Trait(Spec.UAC, "03")]
    [Given("the repaired acceptance fixture inputs")]
    [When("the package-consumption scenario runs")]
    [Then("it executes discoverable assertions against real package outputs without exclusions hiding it")]
    public void UAC03_RepairedFixtureInputsRunDiscoverableAssertionsAgainstRealPackageOutputs()
    {
        var assembly = typeof(SC01_IndependentPackageConsumption).Assembly;

        // The repaired fixture inputs are embedded under their original logical
        // names, so the drift guards keep reading the resources they always did.
        foreach (var logicalName in RepairedFixtureInputs)
        {
            using var stream = assembly.GetManifestResourceStream(logicalName);
            stream.ShouldNotBeNull(
                $"Expected '{logicalName}' to be embedded in the test assembly. " +
                "Wire it up in LowlandTech.Components.Tests.csproj via <EmbeddedResource>.");

            using var reader = new StreamReader(stream!);
            reader.ReadToEnd().Length.ShouldBeGreaterThan(
                0, $"The repaired fixture input '{logicalName}' is empty.");
        }

        // The repaired shell stylesheet still carries the scoped sidebar rule
        // the drift guards require, which proves it is the real host source.
        ReadEmbedded(assembly, "shell.input.css").ShouldContain(
            "html.sidebar-collapsed .app-shell .app-sidebar > .sidebar-footer",
            customMessage: "The repaired shell stylesheet lost the scoped sidebar footer rule.");

        // The scenario is compiled into the test assembly and therefore
        // discoverable by the test runner.
        var scenarioFacts = assembly.GetTypes()
            .Where(type => type.Namespace == typeof(SC01_IndependentPackageConsumption).Namespace)
            .SelectMany(type => type.GetMethods())
            .Where(method => method.GetCustomAttributes()
                .Any(attribute => attribute.GetType().Name == "FactAttribute"))
            .ToList();

        scenarioFacts.ShouldNotBeEmpty(
            "The CPKG0001 acceptance Facts are not present in the test assembly.");
        scenarioFacts.Count.ShouldBeGreaterThanOrEqualTo(2,
            "UAC01 and UAC03 must both be executable.");

        // No exclusion hides this scenario.
        var projectFile = Path.Combine(
            fixture.RepositoryRoot, "src", "test", "LowlandTech.Components.Tests.csproj");
        var projectText = File.ReadAllText(projectFile);

        var exclusions = Regex.Matches(
                projectText, "<(?:Compile|Content|EmbeddedResource|None)\\s+Remove=\"([^\"]+)\"")
            .Select(match => match.Groups[1].Value)
            .ToList();

        exclusions.ShouldAllBe(exclusion => !exclusion.Contains("US01.Packaging", StringComparison.OrdinalIgnoreCase),
            "An exclusion hides the CPKG0001 acceptance scenario.");

        var defaultItemExcludes = Regex.Match(
                projectText, "<DefaultItemExcludes>([^<]*)</DefaultItemExcludes>")
            .Groups[1].Value;

        defaultItemExcludes.ShouldNotContain("US01.Packaging",
            customMessage: "The default item excludes hide the CPKG0001 acceptance scenario.");

        // The assertions run against real package outputs: the feed carries
        // the packages this repository produced, and the consumer resolved them.
        var scenario = fixture.Scenario;
        var componentsPackage = Path.Combine(
            scenario.FeedDirectory, $"LowlandTech.Components.{scenario.PackageVersion}.nupkg");

        new FileInfo(componentsPackage).Length.ShouldBeGreaterThan(0,
            $"'{componentsPackage}' carries no package output.");

        File.ReadAllText(scenario.AssetsFile).ShouldContain(scenario.PackageVersion,
            customMessage: "The consumer did not resolve the packages this repository produced.");
    }

    private static string ReadEmbedded(Assembly assembly, string logicalName)
    {
        using var stream = assembly.GetManifestResourceStream(logicalName);
        stream.ShouldNotBeNull($"Expected '{logicalName}' to be embedded in the test assembly.");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
