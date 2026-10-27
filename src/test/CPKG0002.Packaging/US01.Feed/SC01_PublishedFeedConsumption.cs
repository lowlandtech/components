using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using LowlandTech.Components.Tests.CPKG0001.Packaging;

namespace LowlandTech.Components.Tests.CPKG0002.Packaging.US01.Feed;

/// <summary>
/// SC01 — A scenario consumes the published batch of the shared feed.
///
/// <para>
/// The CPKG0001 packaging harness packs the packages of this repository into a
/// private feed and restores a temporary consumer against that feed alone. The
/// switch of this scope copies the published identities of the shared feed into
/// the scenario feed instead of packing them, so that a scenario consumes the
/// published batch rather than a fresh pack. The dependency closure of the
/// copied packages travels with them, so the consumer still resolves everything
/// from the scenario feed alone.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPKG0002")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A scenario consumes the published batch of the shared feed instead of a fresh pack")]
public class SC01_PublishedFeedConsumption(PublishedBatchScenarioFixture fixture)
    : IClassFixture<PublishedBatchScenarioFixture>
{
    private static readonly string[] BasePackages =
    [
        "LowlandTech.Components.Core",
        "LowlandTech.Components",
        "LowlandTech.Components.Blocks",
    ];

    [Fact(DisplayName = "UAC01: the packaging harness offers the switch that consumes the published batch")]
    [Trait(Spec.UAC, "01")]
    [Given("the packaging harness of this repository")]
    [When("its parameters are read")]
    [Then("it offers a switch that copies the published identities of the shared feed into the scenario feed instead of packing them")]
    public void UAC01_HarnessOffersThePublishedBatchSwitch()
    {
        var parameters = ReadHarnessParameters(fixture.RepositoryRoot);

        parameters.ShouldContainKey(
            "UsePublishedBatch",
            "The packaging harness has no switch that selects the published identities of the shared feed.");
        parameters["UsePublishedBatch"].ShouldBe(
            "System.Management.Automation.SwitchParameter",
            "The published batch has to be a switch, so the default path of the harness stays unchanged.");
    }

    [Fact(DisplayName = "UAC02: a scenario consumes the published identities of the shared feed")]
    [Trait(Spec.UAC, "02")]
    [Given("a scenario that selects the published batch of the shared feed")]
    [When("the scenario runs and the consumer restores")]
    [Then("the consumer resolves the published versions of the packages and not a fresh pack")]
    public void UAC02_ScenarioConsumesThePublishedIdentities()
    {
        var scenario = fixture.Published;

        scenario.UsePublishedBatch.ShouldBeTrue(
            "The scenario did not select the published batch of the shared feed.");
        scenario.PackageSource.ShouldBe(
            "published",
            "The scenario did not take the packages under test from the shared feed.");

        scenario.RestoreExitCode.ShouldBe(
            0, $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(
            0, $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        foreach (var packageId in BasePackages)
        {
            var consumed = Path.Combine(
                scenario.FeedDirectory, $"{packageId}.{scenario.PackageVersion}.nupkg");
            var published = Path.Combine(
                scenario.LocalFeed, $"{packageId}.{scenario.PackageVersion}.nupkg");

            File.Exists(published).ShouldBeTrue(
                $"The shared feed carries no published '{packageId}' {scenario.PackageVersion}.");
            File.Exists(consumed).ShouldBeTrue(
                $"The scenario feed carries no '{packageId}' {scenario.PackageVersion}.");

            // The consumed package is the published identity itself and not a
            // fresh pack of the sources of this repository.
            Hash(consumed).ShouldBe(
                Hash(published),
                $"The consumed '{packageId}' is not the published identity of the shared feed.");
        }

        // The consumer resolved the published versions of the packages.
        using var assets = JsonDocument.Parse(File.ReadAllText(scenario.AssetsFile));
        var libraries = assets.RootElement.GetProperty("libraries").EnumerateObject().ToList();

        foreach (var packageId in BasePackages)
        {
            var resolved = libraries
                .Where(library => library.Name.StartsWith(
                    $"{packageId}/", StringComparison.OrdinalIgnoreCase))
                .Select(library => library.Name)
                .ToList();

            resolved.ShouldNotBeEmpty($"The consumer did not resolve '{packageId}'.");
            resolved.ShouldAllBe(
                name => name.EndsWith($"/{scenario.PackageVersion}", StringComparison.OrdinalIgnoreCase),
                $"The consumer resolved '{packageId}' in a version that is not the published one.");
        }
    }

    [Fact(DisplayName = "UAC03: the default path of the harness still packs the packages of this repository")]
    [Trait(Spec.UAC, "03")]
    [Given("a scenario that does not select the published batch")]
    [When("the scenario runs")]
    [Then("the packages of this repository are packed into the scenario feed and the gate of the closed CPKG0001 scope is unchanged")]
    public void UAC03_DefaultPathStillPacksThePackagesOfThisRepository()
    {
        var scenario = fixture.Default;

        scenario.UsePublishedBatch.ShouldBeFalse(
            "The default path of the harness must not select the published batch.");
        scenario.PackageSource.ShouldBe(
            "packed",
            "The default path of the harness must pack the packages of this repository.");

        scenario.RestoreExitCode.ShouldBe(
            0, $"The consumer restore failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");
        scenario.BuildExitCode.ShouldBe(
            0, $"The consumer build failed. Scenario log:{Environment.NewLine}{scenario.ReadLog()}");

        foreach (var packageId in BasePackages)
        {
            var package = Path.Combine(
                scenario.FeedDirectory, $"{packageId}.{scenario.PackageVersion}.nupkg");

            File.Exists(package).ShouldBeTrue(
                $"The scenario feed carries no packed '{packageId}' {scenario.PackageVersion}.");
            new FileInfo(package).Length.ShouldBeGreaterThan(
                0, $"The packed '{packageId}' is empty, so it carries no real package output.");
        }
    }

    [Fact(DisplayName = "UAC04: the copied published batch travels with its dependency closure")]
    [Trait(Spec.UAC, "04")]
    [Given("a scenario that selects the published batch of the shared feed")]
    [When("the scenario feed is assembled")]
    [Then("the dependency closure of the copied packages travels with them and the consumer never reaches outside the scenario feed")]
    public void UAC04_CopiedBatchTravelsWithItsDependencyClosure()
    {
        var scenario = fixture.Published;

        // The Core package that the copied packages depend on travels with them,
        // so the consumer resolves it from the scenario feed alone.
        var core = Path.Combine(
            scenario.FeedDirectory, $"LowlandTech.Core.{scenario.CoreVersion}.nupkg");
        File.Exists(core).ShouldBeTrue(
            "The dependency closure of the copied batch did not travel with it.");

        using var assets = JsonDocument.Parse(File.ReadAllText(scenario.AssetsFile));
        var root = assets.RootElement;

        // The consumer restored into exactly one package folder, inside the
        // scenario root.
        var packageFolders = root.GetProperty("packageFolders")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToList();

        packageFolders.Count.ShouldBe(1, "The consumer must restore into exactly one package folder.");
        packageFolders[0].ShouldStartWith(
            scenario.PackagesDirectory,
            customMessage: $"'{packageFolders[0]}' resolves outside the scenario root.");

        // Every resolved package sits under that folder.
        foreach (var library in root.GetProperty("libraries").EnumerateObject())
        {
            if (library.Value.GetProperty("type").GetString() != "package")
            {
                continue;
            }

            var path = library.Value.GetProperty("path").GetString()!;
            path.ShouldNotStartWith(
                "..", customMessage: $"'{library.Name}' resolved outside the package folder.");
        }

        // The restore used local sources only, and the scenario feed among them.
        var sources = root.GetProperty("project").GetProperty("restore").GetProperty("sources")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToList();

        sources.ShouldNotBeEmpty("The restore recorded no package source.");
        sources.ShouldContain(
            scenario.FeedDirectory, "The consumer did not restore from the scenario feed.");

        foreach (var source in sources)
        {
            source.ShouldNotStartWith(
                "http", customMessage: "The consumer restore reached a network source.");
        }
    }

    private static IReadOnlyDictionary<string, string> ReadHarnessParameters(string repositoryRoot)
    {
        var script = Path.Combine(
            repositoryRoot, "src", "test", "CPKG0001.Packaging", "consumer-scenario.ps1");
        var command =
            "$c = Get-Command -Name '" + script.Replace("'", "''") + "'; " +
            "$c.Parameters.GetEnumerator() | ForEach-Object { $_.Key + '=' + $_.Value.ParameterType.FullName }";

        var startInfo = new ProcessStartInfo
        {
            FileName = "pwsh",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(command);

        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException("Could not start 'pwsh'.");
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Trim().Split('=', 2);
            if (parts.Length == 2)
            {
                parameters[parts[0]] = parts[1];
            }
        }

        return parameters;
    }

    private static string Hash(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}
