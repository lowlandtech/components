using LowlandTech.Components.Tests.CPKG0007.Trimming.US01.Trimmable.UseCases;

namespace LowlandTech.Components.Tests.CPKG0007.Trimming.US01.Trimmable;

/// <summary>
/// SC01 — A consumer trims the shared assemblies.
///
/// The packages declare themselves trimmable and the two public members that hand a
/// type across the boundary declare which members survive, so a trimmed publish of a
/// consumer trims the shared assemblies instead of copying them byte for byte. The
/// scenario packs a batch into a private feed, publishes a consumer trimmed against it
/// and reads what the trimmer did.
/// </summary>
[Trait("PKG", "0007"), Trait("US", "01"), Trait("SC", "01")]
[UserStory("01", "TrimmedSharedAssemblies")]
public sealed class SC01_TrimmedSharedAssemblies(global::LowlandTech.Components.Tests.Refinement.ScenarioFixture<TrimmedAssembliesUseCase> fixture) : global::LowlandTech.Components.Tests.Refinement.UseCaseScenario<TrimmedAssembliesUseCase>(fixture)
{
    private TrimmedAssembliesHarness? _harness;

    [Given("a private feed that packs the trimmable projects")]
    protected override Task GivenAsync()
    {
        _harness = TrimmedAssembliesHarness.Prepare();
        return Task.CompletedTask;
    }

    [When("the consumer is published trimmed and run, and the batch is restored with the current Core")]
    protected override Task WhenAsync()
    {
        var harness = _harness ?? throw new InvalidOperationException("The harness was not prepared.");
        harness.Run();
        var result = harness.Result ?? throw new InvalidOperationException("The scenario produced no result.");

        // UAC01: the pack builds the trimmable projects without a trim analyzer warning.
        UseCase.AnalyzerSilentWhenTrimmable = result.PackExit == 0 && result.PackWarnings == 0;

        // UAC02: both assemblies are trimmed, and the trimmed consumer still runs.
        var bothTrimmed =
            result.Shipped.TryGetValue("LowlandTech.Components", out var shippedComponents) &&
            result.Trimmed.TryGetValue("LowlandTech.Components", out var trimmedComponents) &&
            result.Shipped.TryGetValue("LowlandTech.Components.Blocks", out var shippedBlocks) &&
            result.Trimmed.TryGetValue("LowlandTech.Components.Blocks", out var trimmedBlocks) &&
            trimmedComponents < shippedComponents &&
            trimmedBlocks < shippedBlocks;
        UseCase.Sizes = string.Join(", ", result.Shipped.Select(pair =>
            pair.Key + " " + pair.Value + "->" + (result.Trimmed.TryGetValue(pair.Key, out var size) ? size : 0)));
        UseCase.AssembliesAreTrimmed =
            result.PublishExit == 0 &&
            result.RunExit == 0 &&
            result.RunOutput.Contains("COLUMNS=2", StringComparison.Ordinal) &&
            bothTrimmed;

        // UAC03: the trimmable metadata and the two member declarations are published.
        UseCase.ContractsAreDeclared =
            result.Annotations.TryGetValue("IsTrimmable", out var trimmable) && trimmable == "true" &&
            result.Annotations.TryGetValue("ComponentDescriptor.ComponentType", out var component) &&
            component.Contains("PublicParameterlessConstructor", StringComparison.Ordinal) &&
            result.Annotations.TryGetValue("PropertyGridAttribute.OptionsProvider", out var provider) &&
            provider.Contains("PublicProperties", StringComparison.Ordinal) &&
            provider.Contains("PublicFields", StringComparison.Ordinal);

        // UAC04: the batch restores with the current Core and reports no version conflict.
        UseCase.BatchRestoresCleanly =
            result.RestoreExit == 0 &&
            !result.RestoreText.Contains("NU1107", StringComparison.Ordinal) &&
            !result.RestoreText.Contains("NU1608", StringComparison.Ordinal);

        return Task.CompletedTask;
    }

    [Then("Result.AnalyzerSilentWhenTrimmable should be true")]
    [Trait("UAC", "01"), Fact(DisplayName = "UAC01: the trimmable projects pack without a trim analyzer warning")]
    public void UAC01_AnalyzerSilentWhenTrimmable() => Result.AnalyzerSilentWhenTrimmable.ShouldBe(true);

    [Then("Result.AssembliesAreTrimmed should be true")]
    [Trait("UAC", "02"), Fact(DisplayName = "UAC02: both shared assemblies shrink in the trimmed publish and the consumer runs")]
    public void UAC02_AssembliesAreTrimmed() => Result.AssembliesAreTrimmed.ShouldBe(true, Result.Sizes);

    [Then("Result.ContractsAreDeclared should be true")]
    [Trait("UAC", "03"), Fact(DisplayName = "UAC03: the packed assembly declares the trimmable metadata and both member contracts")]
    public void UAC03_ContractsAreDeclared() => Result.ContractsAreDeclared.ShouldBe(true);

    [Then("Result.BatchRestoresCleanly should be true")]
    [Trait("UAC", "04"), Fact(DisplayName = "UAC04: the batch restores with the current Core without NU1107 or NU1608")]
    public void UAC04_BatchRestoresCleanly() => Result.BatchRestoresCleanly.ShouldBe(true);
}
