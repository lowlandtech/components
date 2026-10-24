using LowlandTech.Components.Core;
using LowlandTech.Components.Features.Session;

namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// SC01 — Publish an opt-in UI feature manifest.
///
/// <para>
/// One small Components feature becomes selectable as a Blazor plugin while the
/// base UI libraries stay ordinary dependencies. The Facts below are the
/// executable acceptance for the three acceptance criteria of this scope.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPLG0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A Components feature is selectable as a Blazor plugin while the base libraries stay ordinary dependencies")]
public sealed class SC01_UiFeatureManifest(FeaturePackageFixture fixture)
    : IClassFixture<FeaturePackageFixture>
{
    [Fact(DisplayName = "UAC01: the feature package carries exactly one valid plugin manifest and the base libraries stay ordinary dependencies")]
    [Trait(Spec.UAC, "01")]
    [Given("the locally packed feature package and the base Components packages")]
    [When("the Plugins inspector inspects the whole package set")]
    [Then("exactly one package carries a valid lowlandtech/plugin.json for a blazor target and the base libraries carry none")]
    public void UAC01_TheFeaturePackageCarriesExactlyOneValidPluginManifest() =>
        fixture.Inspection.ShouldSatisfyAllConditions(
            () => fixture.Inspection.IsValid.ShouldBeTrue(
                $"The Plugins inspector rejected the package set: {Describe(fixture.Inspection)}."),
            () => fixture.ManifestPackages.Count.ShouldBe(
                1,
                "Exactly one package of the set may carry a plugin manifest."),
            () => fixture.ManifestPackages[0].PackageId.ShouldBe(
                FeaturePackageFixture.FeaturePackageId,
                "The manifest was found in a package that is not the opt-in feature package."),
            () => fixture.ManifestPackages[0].RequiresBlazor.ShouldBeTrue(
                "The feature manifest does not target blazor."),
            () => fixture.ManifestPackages[0].Manifest!.Key.ShouldBe(
                ComponentsSessionFeature.LogicalKey,
                "The feature manifest does not declare the stable logical plugin key."),
            () => fixture.ManifestPackages[0].Manifest!.SchemaVersion.ShouldBe(
                2,
                "The feature manifest does not declare the versioned manifest schema."),
            () => fixture.ManifestPackages[0].Manifest!.Contributions.Components.ShouldContain(
                ComponentsSessionFeature.SessionHostComponentIdentity,
                "The feature manifest does not declare the contributed component identity."),
            () => fixture.ManifestPackages[0].Manifest!.Dependencies
                .Select(dependency => dependency.Key)
                .ShouldContain(
                    ComponentsSessionFeature.LogicalDependencyKey,
                    "The feature manifest declares no logical dependency on the base Blocks library."),
            () => fixture.Inspection.Packages
                .Where(package => package.PackageId != FeaturePackageFixture.FeaturePackageId)
                .ShouldAllBe(
                    package => !package.IsPlugin,
                    "A base library carries a plugin manifest, so it is no longer an ordinary dependency."),
            () => fixture.FeaturePackageDependencies.ShouldContain(
                "LowlandTech.Components.Blocks",
                "The feature package does not depend on the base Blocks package as an ordinary dependency."));

    [Fact(DisplayName = "UAC02: the declared component resolves through the trusted startup map and undeclared or inactive components do not")]
    [Trait(Spec.UAC, "02")]
    [Given("an explicit compatible deployed host that registered the base logical keys and the feature startup binding")]
    [When("the feature activates")]
    [Then("its declared component resolves through the trusted startup map while inactive or undeclared components do not resolve")]
    public void UAC02_TheDeclaredComponentResolvesThroughTheTrustedStartupMap() =>
        fixture.ActiveOutcome.ShouldSatisfyAllConditions(
            () => fixture.ActiveOutcome.IsActivated.ShouldBeTrue(
                $"The feature did not activate in a compatible deployed host: {Describe(fixture.ActiveOutcome)}."),
            () => fixture.ActiveOutcome.ResolvedComponents.ShouldContain(
                typeof(SessionPluginHost),
                "The declared component did not resolve through the trusted startup map."),
            () => fixture.ActiveOutcome.ResolvedComponents.ShouldAllBe(
                component => component == typeof(SessionPluginHost),
                "The activation resolved a component the manifest does not declare."),
            () => fixture.InactiveOutcome.IsActivated.ShouldBeFalse(
                "An inactive feature activated in a host that does not carry it."),
            () => fixture.InactiveOutcome.ResolvedComponents.ShouldBeEmpty(
                "An inactive feature resolved a component through the trusted startup map."),
            () => fixture.UndeclaredOutcome.ResolvedComponents.ShouldBeEmpty(
                "A component the manifest does not declare resolved through the trusted startup map."),
            () => typeof(SessionPluginHost).FullName.ShouldBe(
                ComponentsSessionFeature.SessionHostComponentIdentity,
                "The declared identity does not name the contributed component type."),
            () => fixture.StartupMap.ContainsLogicalKey(ComponentsSessionFeature.LogicalKey).ShouldBeTrue(
                "The feature does not register its own logical key at startup."),
            () => fixture.StartupMap.ContainsLogicalKey(ComponentsSessionFeature.LogicalDependencyKey).ShouldBeTrue(
                "The base Blocks library does not register the logical key the feature depends on."));

    [Fact(DisplayName = "UAC03: validation fails by name without mutating the project or the deployment snapshot")]
    [Trait(Spec.UAC, "03")]
    [Given("a feature manifest that targets a backend only host and a manifest whose logical dependency is not installed")]
    [When("feature activation is validated")]
    [Then("it fails by name without mutating the project or the deployment snapshot and a missing deployment reports rebuild and restart")]
    public void UAC03_ValidationFailsByNameWithoutMutatingTheHost() =>
        fixture.BackendTargetOutcome.ShouldSatisfyAllConditions(
            () => fixture.BackendTargetOutcome.IsActivated.ShouldBeFalse(
                "A backend only target activated in a blazor host."),
            () => fixture.BackendTargetOutcome.Errors.ShouldContain(
                error => error.Subject == ComponentsSessionFeature.LogicalKey,
                "The backend only target failure does not name the feature."),
            () => fixture.BackendTargetOutcome.Errors.ShouldContain(
                error => error.Code == ComponentsFeatureActivationErrorCode.UnsupportedTarget,
                "The backend only target failure is not reported as an unsupported target."),
            () => fixture.MissingDependencyOutcome.IsActivated.ShouldBeFalse(
                "A feature with a missing logical dependency activated."),
            () => fixture.MissingDependencyOutcome.Errors.ShouldContain(
                error => error.Subject == "lowlandtech.components.absent",
                "The missing logical dependency failure does not name the missing dependency."),
            () => fixture.MissingDependencyOutcome.Errors.ShouldContain(
                error => error.Message.Contains("lowlandtech.components.absent", StringComparison.Ordinal),
                "The missing logical dependency failure does not report the missing dependency by name."),
            () => fixture.InactiveOutcome.Errors.ShouldContain(
                error => error.Subject == ComponentsSessionFeature.LogicalKey,
                "The missing deployment failure does not name the feature."),
            () => fixture.InactiveOutcome.Errors.ShouldContain(
                error => error.Code == ComponentsFeatureActivationErrorCode.MissingDeployment,
                "The missing deployment failure is not reported as a missing deployment."),
            () => fixture.InactiveOutcome.Errors.ShouldContain(
                error => error.Message.Contains("rebuild", StringComparison.OrdinalIgnoreCase)
                         && error.Message.Contains("restart", StringComparison.OrdinalIgnoreCase),
                "The missing deployment failure does not report that a rebuild and a restart are required."),
            () => fixture.ActivationLeftHostUnchanged.ShouldBeTrue(
                "The validation mutated the project or the deployment snapshot of the host."));

    private static string Describe(PluginInspectionResult inspection) =>
        string.Join(
            "; ",
            inspection.Errors.Select(error => $"{error.Code}: {error.Message}"));

    private static string Describe(ComponentsFeatureActivationResult outcome) =>
        string.Join(
            "; ",
            outcome.Errors.Select(error => $"{error.Code} [{error.Subject}]: {error.Message}"));
}
