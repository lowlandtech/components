using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;
using LowlandTech.Components;
using LowlandTech.Components.Blocks;
using LowlandTech.Components.Tests.CPKG0001.Packaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LowlandTech.Components.Tests.CLOC0001.Packaging.US01.Localization;

/// <summary>
/// SC01 — The packaged localization of the Components packages resolves.
///
/// <para>
/// The manifest name of an embedded resource decides whether
/// <c>IStringLocalizer&lt;T&gt;</c> resolves its strings.
/// <c>ResourceManagerStringLocalizerFactory</c> asks for the full name of the
/// type, so the resource of <c>LowlandTech.Components.DataFormStrings</c> has
/// to carry the manifest name
/// <c>LowlandTech.Components.DataFormStrings.resources</c>. A host that
/// configures a resources path asks for another name: the path is inserted
/// between the assembly and the type, so the packages carry that name as well.
/// </para>
///
/// <para>
/// The English resource of a type is the neutral resource of that type and
/// travels in the assembly of the package itself, so a client that asks for a
/// culture without a satellite of its own reads the English resource. A
/// resource of a culture travels in the satellite of that culture under the
/// name that carries the culture.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CLOC0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A consumer of the Components packages reads the localized labels of the package")]
public class SC01_PackagedLocalization
{
    private const string ControlsResource = "LowlandTech.Components.DataFormStrings";
    private const string ControlsResourceWithPath = "LowlandTech.Components.Resources.DataFormStrings";
    private const string BlocksResource = "LowlandTech.Components.Blocks.SessionBlockStrings";

    [Fact(DisplayName = "UAC01: a host without a resources path reads the English label of a type")]
    [Trait(Spec.UAC, "01")]
    [Given("a host that registers the localization without a resources path")]
    [When("it reads the label of a type of the controls package")]
    [Then("it reads the English label of the package instead of the key")]
    public void UAC01_HostWithoutResourcesPathReadsTheEnglishLabel()
    {
        var label = Read(resourcesPath: null, culture: null, resourceType: typeof(DataFormStrings), key: "ResetButton");

        label.ShouldBe("Reset");
        label.ShouldNotBe("ResetButton");
    }

    [Fact(DisplayName = "UAC02: a host without a resources path reads the translated label of a culture")]
    [Trait(Spec.UAC, "02")]
    [Given("a host that registers the localization without a resources path")]
    [When("it reads the label of a type of the controls package in Danish")]
    [Then("it reads the Danish label of the satellite of that culture")]
    public void UAC02_HostWithoutResourcesPathReadsTheTranslatedLabel()
    {
        var label = Read(resourcesPath: null, culture: "da-DK", resourceType: typeof(DataFormStrings), key: "ResetButton");

        label.ShouldBe("Nulstil");
        label.ShouldNotBe("ResetButton");
    }

    [Fact(DisplayName = "UAC03: a host that configures a resources path reads the labels as well")]
    [Trait(Spec.UAC, "03")]
    [Given("a host that registers the localization with the resources path of this workspace")]
    [When("it reads the labels of a type of the controls package in English and in Danish")]
    [Then("it reads the English and the Danish label instead of the key")]
    public void UAC03_HostWithResourcesPathReadsTheLabels()
    {
        Read(resourcesPath: "Resources", culture: null, resourceType: typeof(DataFormStrings), key: "ResetButton")
            .ShouldBe("Reset");

        Read(resourcesPath: "Resources", culture: "da-DK", resourceType: typeof(DataFormStrings), key: "ResetButton")
            .ShouldBe("Nulstil");
    }

    [Fact(DisplayName = "UAC04: the blocks package reads its labels for a host without a resources path")]
    [Trait(Spec.UAC, "04")]
    [Given("a host that registers the localization without a resources path")]
    [When("it reads the labels of a type of the blocks package in English and in Danish")]
    [Then("it reads the English and the Danish label instead of the key")]
    public void UAC04_BlocksPackageReadsItsLabels()
    {
        Read(resourcesPath: null, culture: null, resourceType: typeof(SessionBlockStrings), key: "CommandSubmitTitle")
            .ShouldBe("Send");

        Read(resourcesPath: null, culture: "da-DK", resourceType: typeof(SessionBlockStrings), key: "CommandSubmitTitle")
            .ShouldBe("Send");
    }

    [Fact(DisplayName = "UAC05: the English label is the neutral resource of the package itself")]
    [Trait(Spec.UAC, "05")]
    [Given("the controls package and the blocks package")]
    [When("the manifest names of their assemblies are read")]
    [Then("each assembly carries the neutral resource of every type it owns")]
    public void UAC05_EnglishLabelIsTheNeutralResourceOfTheAssembly()
    {
        var controls = typeof(DataFormStrings).Assembly;
        var blocks = typeof(SessionBlockStrings).Assembly;

        controls.GetManifestResourceNames().ShouldContain($"{ControlsResource}.resources");
        controls.GetManifestResourceNames().ShouldContain($"{ControlsResourceWithPath}.resources");

        blocks.GetManifestResourceNames().ShouldContain($"{BlocksResource}.resources");
        blocks.GetManifestResourceNames().ShouldContain($"LowlandTech.Components.Blocks.Resources.SessionBlockStrings.resources");
    }

    [Fact(DisplayName = "UAC06: a satellite carries the resource of its culture under the name with the culture")]
    [Trait(Spec.UAC, "06")]
    [Given("the controls package")]
    [When("the manifest names of its Danish satellite are read")]
    [Then("the satellite carries the resource of every type under the name with the culture")]
    public void UAC06_SatelliteCarriesTheResourceOfItsCulture()
    {
        var satellite = LoadSatellite(typeof(DataFormStrings).Assembly, "da");

        satellite.GetManifestResourceNames().ShouldContain($"{ControlsResource}.da.resources");
        satellite.GetManifestResourceNames().ShouldContain($"{ControlsResourceWithPath}.da.resources");

        using var stream = satellite.GetManifestResourceStream($"{ControlsResource}.da.resources");
        stream.ShouldNotBeNull();
        using var reader = new ResourceReader(stream!);
        var values = reader.Cast<System.Collections.DictionaryEntry>()
            .ToDictionary(entry => entry.Key.ToString()!, entry => entry.Value?.ToString() ?? string.Empty);

        values["ResetButton"].ShouldBe("Nulstil");
    }

    [Fact(DisplayName = "UAC07: the packed package carries the resources of a host without a resources path")]
    [Trait(Spec.UAC, "07")]
    [Given("the controls package as it is packed into a feed")]
    [When("the payload of the package is read")]
    [Then("it carries the neutral resource of every type and the satellite of every culture")]
    public void UAC07_PackedPackageCarriesTheResources()
    {
        var repositoryRoot = ConsumerScenario.FindRepositoryRoot();

        using var scenario = ConsumerScenario.Run(repositoryRoot);

        var package = Path.Combine(
            scenario.FeedDirectory, $"LowlandTech.Components.{scenario.PackageVersion}.nupkg");

        File.Exists(package).ShouldBeTrue($"Expected the packed package at '{package}'.");

        var extractRoot = Path.Combine(Path.GetTempPath(), $"cloc0001-{Guid.NewGuid():N}");
        Directory.CreateDirectory(extractRoot);

        try
        {
            using (var archive = ZipFile.OpenRead(package))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.StartsWith("lib/net10.0/", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var destination = Path.Combine(
                        extractRoot, entry.FullName.Replace('/', Path.DirectorySeparatorChar));

                    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                    entry.ExtractToFile(destination, overwrite: true);
                }
            }

            var assemblyPath = Path.Combine(extractRoot, "lib", "net10.0", "LowlandTech.Components.dll");
            File.Exists(assemblyPath).ShouldBeTrue(
                $"Expected the controls assembly in the package at '{assemblyPath}'.");

            // The English resource of a type is the neutral resource of that
            // type, so the package carries it in the assembly itself and needs
            // no satellite to answer in English.
            var satellitePath = Path.Combine(
                extractRoot, "lib", "net10.0", "da", "LowlandTech.Components.resources.dll");

            File.Exists(satellitePath).ShouldBeTrue(
                $"Expected the Danish satellite in the package at '{satellitePath}'.");

            var context = new AssemblyLoadContext($"cloc0001-{Guid.NewGuid():N}", isCollectible: true);

            try
            {
                var assembly = context.LoadFromAssemblyPath(assemblyPath);

                assembly.GetManifestResourceNames().ShouldContain($"{ControlsResource}.resources");
                assembly.GetManifestResourceNames().ShouldContain($"{ControlsResourceWithPath}.resources");

                var satellite = context.LoadFromAssemblyPath(satellitePath);
                satellite.GetManifestResourceNames().ShouldContain($"{ControlsResource}.da.resources");
                satellite.GetManifestResourceNames().ShouldContain($"{ControlsResourceWithPath}.da.resources");

                // A host that registers the localization without a resources
                // path reads the English label out of the package itself.
                var manager = new ResourceManager(ControlsResource, assembly);
                manager.GetString("ResetButton", CultureInfo.InvariantCulture).ShouldBe("Reset");
            }
            finally
            {
                context.Unload();
            }
        }
        finally
        {
            try
            {
                Directory.Delete(extractRoot, recursive: true);
            }
            catch (IOException)
            {
                // A leftover temporary directory must never fail an acceptance run.
            }
            catch (UnauthorizedAccessException)
            {
                // The collected context can still hold the payload for a moment.
            }
        }
    }

    private static string Read(string? resourcesPath, string? culture, Type resourceType, string key)
    {
        var services = new ServiceCollection();

        // The localizer factory of the framework logs the resources it misses,
        // so it needs a logger factory.
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);

        if (resourcesPath is null)
        {
            services.AddLocalization();
        }
        else
        {
            services.AddLocalization(options => options.ResourcesPath = resourcesPath);
        }

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IStringLocalizerFactory>();

        var originalCulture = CultureInfo.CurrentUICulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            if (culture is not null)
            {
                var requested = new CultureInfo(culture);
                CultureInfo.CurrentCulture = requested;
                CultureInfo.CurrentUICulture = requested;
            }

            var localizer = factory.Create(resourceType);
            return localizer[key].Value;
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private static Assembly LoadSatellite(Assembly assembly, string culture)
    {
        var directory = Path.GetDirectoryName(assembly.Location);
        var satellitePath = Path.Combine(directory!, culture, $"{assembly.GetName().Name}.resources.dll");

        File.Exists(satellitePath).ShouldBeTrue(
            $"Expected the satellite of the culture '{culture}' at '{satellitePath}'.");

        return Assembly.LoadFrom(satellitePath);
    }
}
