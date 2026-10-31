using LowlandTech.Components.Tests.CPKG0004.Packing.US01.Invocation.UseCases;

namespace LowlandTech.Components.Tests.CPKG0004.Packing.US01.Invocation;

/// <summary>
/// SC01 — The packing tool diagnoses a project selection it cannot bind.
///
/// A caller that selects a subset of projects either binds the selection it meant or
/// is told, by name, how to pass it. The scenario runs the tool in the three shapes a
/// caller produces and reads what the tool answered and published.
/// </summary>
[Trait("PKG", "0004"), Trait("US", "01"), Trait("SC", "01")]
[UserStory("01", "ProjectSelectionDiagnosis")]
public sealed class SC01_ProjectSelectionDiagnosis(global::LowlandTech.Components.Tests.Refinement.ScenarioFixture<ProjectSelectionUseCase> fixture) : global::LowlandTech.Components.Tests.Refinement.UseCaseScenario<ProjectSelectionUseCase>(fixture)
{
    private ProjectSelectionHarness? _packing;

    [Given("a private feed that carries the contracts generation of the packages")]
    protected override Task GivenAsync()
    {
        _packing = ProjectSelectionHarness.Prepare();
        return Task.CompletedTask;
    }

    [When("the packing tool receives a joined selection, a stray positional and the supported array")]
    protected override Task WhenAsync()
    {
        var packing = _packing ?? throw new InvalidOperationException("The private feed was not prepared.");
        packing.RunJoinedSelection();
        packing.RunStrayPositional();
        packing.RunSupportedSelection();
        var publishedAfterSupported = packing.PublishedPackages();
        packing.RunTwoBatchesConcurrently();

        // UAC01: the joined string is refused, and the diagnostic names the invocation.
        UseCase.JoinedSelectionRefused =
            packing.JoinedExit != 0 &&
            packing.JoinedOutput.Contains("is a joined string, not a list", StringComparison.Ordinal) &&
            packing.JoinedOutput.Contains("-Command", StringComparison.Ordinal);

        // UAC02: the stray positional is refused with the same invitation, not with a
        // positional binding error.
        UseCase.StrayPositionalRefused =
            packing.StrayExit != 0 &&
            packing.StrayOutput.Contains("is not a parameter of this tool", StringComparison.Ordinal) &&
            packing.StrayOutput.Contains("-Command", StringComparison.Ordinal);

        // UAC03: the supported array packs exactly the two selected projects.
        var published = packing.PublishedPackages();
        UseCase.JoinedDiagnostic = "joined exit " + packing.JoinedExit + ": " + packing.JoinedOutput;
        UseCase.StrayDiagnostic = "stray exit " + packing.StrayExit + ": " + packing.StrayOutput;
        UseCase.SupportedDiagnostic = "supported exit " + packing.SupportedExit + ": " + string.Join(",", published);
        UseCase.ConcurrentDiagnostic = "concurrent exits " + string.Join(",", packing.ConcurrentExits) + ": " + string.Join(" || ", packing.ConcurrentOutputs);
        UseCase.SupportedSelectionPacks =
            packing.SupportedExit == 0 &&
            publishedAfterSupported.Count == 2 &&
            publishedAfterSupported.All(name => !name.Contains(".Features.", StringComparison.Ordinal));

        // UAC04: two batches started together both publish: the tool serializes the
        // build and the pack, so neither reports the IO error of a shared output.
        UseCase.ConcurrentBatchesPublish =
            packing.ConcurrentExits.Count == 2 &&
            packing.ConcurrentExits.All(exit => exit == 0) &&
            published.Count == 6 &&
            packing.ConcurrentOutputs.All(output =>
                !output.Contains("MSB4018", StringComparison.Ordinal) &&
                !output.Contains("cannot access the file", StringComparison.OrdinalIgnoreCase));

        return Task.CompletedTask;
    }

    [Then("Result.JoinedSelectionRefused should be true")]
    [Trait("UAC", "01"), Fact(DisplayName = "UAC01: a comma-joined selection is refused with the diagnostic that names the supported invocation")]
    public void UAC01_JoinedSelectionRefused() => Result.JoinedSelectionRefused.ShouldBe(true, Result.JoinedDiagnostic);

    [Then("Result.StrayPositionalRefused should be true")]
    [Trait("UAC", "02"), Fact(DisplayName = "UAC02: a project left as a positional argument is refused with the same named diagnostic")]
    public void UAC02_StrayPositionalRefused() => Result.StrayPositionalRefused.ShouldBe(true, Result.StrayDiagnostic);

    [Then("Result.ConcurrentBatchesPublish should be true")]
    [Trait("UAC", "04"), Fact(DisplayName = "UAC04: two batches started at the same time both publish without a build-output collision")]
    public void UAC04_ConcurrentBatchesPublish() => Result.ConcurrentBatchesPublish.ShouldBe(true, Result.ConcurrentDiagnostic);

    [Then("Result.SupportedSelectionPacks should be true")]
    [Trait("UAC", "03"), Fact(DisplayName = "UAC03: the supported array invocation publishes exactly the two selected packages")]
    public void UAC03_SupportedSelectionPacks() => Result.SupportedSelectionPacks.ShouldBe(true, Result.SupportedDiagnostic);
}
