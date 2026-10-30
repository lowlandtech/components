using LowlandTech.Components.Tests.CPKG0003.Packing.US01.Declaration.UseCases;

namespace LowlandTech.Components.Tests.CPKG0003.Packing.US01.Declaration;

/// <summary>
/// SC01 — The batch declares its dependency range exactly.
///
/// A batch of this repository must name the contracts generation it was built
/// against as an exact range, so a consumer that resolves it with the Core that
/// pins the same generation resolves one pair. The scenario packs a batch into a
/// private feed through the native tool, reads the declaration it wrote, and drives
/// the refusals the tool owes.
/// </summary>
[Trait("PKG", "0003"), Trait("US", "01"), Trait("SC", "01")]
[UserStory("01", "ExactBatchDeclaration")]
public sealed class SC01_ExactBatchDeclaration(global::LowlandTech.Components.Tests.Refinement.ScenarioFixture<ExactDeclarationUseCase> fixture) : global::LowlandTech.Components.Tests.Refinement.UseCaseScenario<ExactDeclarationUseCase>(fixture)
{
    private PackingDeclarationHarness? _packing;

    [Given("a private feed that holds the contracts generation the batch has to declare")]
    protected override Task GivenAsync()
    {
        _packing = PackingDeclarationHarness.Prepare();
        return Task.CompletedTask;
    }

    [When("the packing tool packs the batch, repacks its version and is asked for an empty selection")]
    protected override Task WhenAsync()
    {
        var packing = _packing ?? throw new InvalidOperationException("The private feed was not prepared.");
        packing.Pack();
        packing.Repack();
        packing.PackWithoutProjects();

        var declarations = packing.ReadDeclarations();

        // UAC01: both dependencies are declared as exact ranges.
        UseCase.ExactDeclaration =
            packing.PackExit == 0 &&
            declarations.Contains("[" + PackingDeclarationHarness.ContractsVersion + "]") &&
            declarations.Contains("[" + PackingDeclarationHarness.CoreVersion + "]");

        // UAC02: the manifest prefix sorts above the batches the feed already holds.
        var prefix = Version.Parse(packing.ReadVersionPrefix());
        UseCase.PrefixSortsAbovePublished = prefix > new Version(2026, 10, 2);

        // UAC03: the version the feed already holds is refused.
        UseCase.ExistingVersionRefused =
            packing.RepackExit != 0 &&
            (!string.IsNullOrWhiteSpace(packing.RepackOutput));

        // UAC04: the explicitly empty selection is refused, and not by the generic
        // missing-input path of a project the caller never named.
        UseCase.EmptySelectionRefused =
            packing.EmptySelectionExit != 0 &&
            packing.EmptySelectionOutput.Contains("Select at least one packable project", StringComparison.Ordinal);

        return Task.CompletedTask;
    }

    [Then("Result.ExactDeclaration should be true")]
    [Trait("UAC", "01"), Fact(DisplayName = "UAC01: the packed nuspec declares the contracts generation and the Core as exact ranges")]
    public void UAC01_ExactDeclaration() => Result.ExactDeclaration.ShouldBe(true);

    [Then("Result.PrefixSortsAbovePublished should be true")]
    [Trait("UAC", "02"), Fact(DisplayName = "UAC02: the manifest prefix sorts above the published batches")]
    public void UAC02_PrefixSortsAbovePublished() => Result.PrefixSortsAbovePublished.ShouldBe(true);

    [Then("Result.ExistingVersionRefused should be true")]
    [Trait("UAC", "03"), Fact(DisplayName = "UAC03: a version the feed already holds is refused")]
    public void UAC03_ExistingVersionRefused() => Result.ExistingVersionRefused.ShouldBe(true);

    [Then("Result.EmptySelectionRefused should be true")]
    [Trait("UAC", "04"), Fact(DisplayName = "UAC04: an explicitly empty project selection is refused with a named diagnostic")]
    public void UAC04_EmptySelectionRefused() => Result.EmptySelectionRefused.ShouldBe(true);
}
