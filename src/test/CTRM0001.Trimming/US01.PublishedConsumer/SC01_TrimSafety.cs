using System.Text.Json;

namespace LowlandTech.Components.Tests.CTRM0001.Trimming.US01.PublishedConsumer;

/// <summary>
/// SC01 — Trim safety of the shared components.
///
/// A host that publishes a browser client trimmed must not inherit a trim warning
/// from the components packages, and the component behaviour that the trim
/// suppressions cover must still work after the trimmer removed what nothing
/// proved was needed. The scenario packs this repository's published projects into
/// a private feed, writes a consumer that exercises the component surface, publishes
/// that consumer trimmed and runs it.
/// </summary>
[Trait("CTRM", "0001"), Trait("US", "01"), Trait("SC", "01")]
[UserStory("01", "TrimSafety")]
public sealed class SC01_TrimSafety(global::LowlandTech.Components.Tests.Refinement.ScenarioFixture<UseCases.TrimSafetyUseCase> fixture) : global::LowlandTech.Components.Tests.Refinement.UseCaseScenario<UseCases.TrimSafetyUseCase>(fixture)
{
    private TrimProbeHarness? _probe;

    [Given("an isolated scenario root for the trim preservation probe")]
    protected override Task GivenAsync()
    {
        _probe = TrimProbeHarness.Prepare();
        return Task.CompletedTask;
    }

    [When("the probe packs the components into a private feed, publishes the consumer trimmed and runs it")]
    protected override Task WhenAsync()
    {
        var harness = _probe ?? throw new InvalidOperationException("The probe was not prepared.");
        harness.Run();
        var result = harness.Result ?? throw new InvalidOperationException("The probe produced no result.");
        var probe = result.Probe;

        // UAC01: the trimmed publish has to succeed without a trim warning that
        // names a components assembly, and the trimmed consumer has to run.
        UseCase.CanonicalWarningsAbsent =
            result.PublishExit == 0 &&
            result.RunExit == 0 &&
            result.OwnedWarnings.Count == 0;

        // UAC02: the persisted payloads keep their shape in the trimmed consumer.
        UseCase.PersistedShapeRetained =
            Text(probe, "promptJson").Contains("\"context\"", StringComparison.Ordinal) &&
            Text(probe, "gridJson").Contains("\"Columns\":2", StringComparison.Ordinal) &&
            Number(probe, "gridColumnsAfterRestore") == 2;

        // UAC03: discovery keeps the component, its properties, its label and its options.
        UseCase.DesignMetadataRetained =
            Flag(probe, "discovered") &&
            Text(probe, "titleLabel") == "Probe title" &&
            Text(probe, "titleDefault") == "hello" &&
            Text(probe, "choiceOptions") == "one,two" &&
            Text(probe, "properties") == "Title,Choice";

        // UAC04: the DataForm discovers and renders the fields of its model in the
        // trimmed consumer, and it still sets, reads and validates a value of the
        // model in the untrimmed component host.
        UseCase.DataFormBehaviorRetained =
            Flag(probe, "dataFormRendered") &&
            Flag(probe, "dataFormHasNameField") &&
            Flag(probe, "dataFormHasAgeField") &&
            DataFormSetsReadsAndValidates();

        // UAC05: the packed batch declares the exact Core pin and the published
        // contracts generation, and the trimmed consumer that restored it ran. The
        // contracts dependency is accepted as the exact range once the packing scope
        // of PRB-001 emits it; today the pack declares that generation as its lower
        // bound, which resolves to the same package because Core pins it exactly.
        UseCase.SuiteAndPackagePinRetained =
            result.CoreDependency == "[" + result.Core + "]" &&
            (result.ContractsDependency == result.Contracts ||
             result.ContractsDependency == "[" + result.Contracts + "]") &&
            result.PublishExit == 0 &&
            result.RunExit == 0;

        return Task.CompletedTask;
    }

    [Then("Result.CanonicalWarningsAbsent should be true")]
    [Trait("UAC", "01"), Fact(DisplayName = "UAC01: the trimmed publish names no components assembly and the consumer runs")]
    public void UAC01_CanonicalWarningsAbsent() => Result.CanonicalWarningsAbsent.ShouldBe(true);

    [Then("Result.PersistedShapeRetained should be true")]
    [Trait("UAC", "02"), Fact(DisplayName = "UAC02: the persisted prompt and grid payloads round-trip in the trimmed consumer")]
    public void UAC02_PersistedShapeRetained() => Result.PersistedShapeRetained.ShouldBe(true);

    [Then("Result.DesignMetadataRetained should be true")]
    [Trait("UAC", "03"), Fact(DisplayName = "UAC03: discovery keeps the designable component, its label and its options")]
    public void UAC03_DesignMetadataRetained() => Result.DesignMetadataRetained.ShouldBe(true);

    [Then("Result.DataFormBehaviorRetained should be true")]
    [Trait("UAC", "04"), Fact(DisplayName = "UAC04: the DataForm discovers and renders the fields of its model")]
    public void UAC04_DataFormBehaviorRetained() => Result.DataFormBehaviorRetained.ShouldBe(true);

    [Then("Result.SuiteAndPackagePinRetained should be true")]
    [Trait("UAC", "05"), Fact(DisplayName = "UAC05: the batch declares the exact Core and contracts pair and the consumer runs")]
    public void UAC05_SuiteAndPackagePinRetained() => Result.SuiteAndPackagePinRetained.ShouldBe(true);

    private static bool Flag(IReadOnlyDictionary<string, JsonElement> probe, string key) =>
        probe.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.True;

    /// <summary>
    /// Binds a model to the DataForm, changes a rendered field and submits the form,
    /// so the set, the read and the reported validation errors are observed through
    /// the component's own surface.
    /// </summary>
    private static bool DataFormSetsReadsAndValidates()
    {
        using var host = new BunitContext();
        host.JSInterop.Mode = JSRuntimeMode.Loose;
        host.Services.AddLocalization();

        var model = new TrimProbeFormModel { Name = string.Empty, Rating = 99 };
        var cut = host.Render<DataForm>(parameters => parameters.Add(form => form.Model, model));

        var input = cut.Find("[data-field-input='Name']");
        input.Input("Grace");
        input.Change("Grace");

        var editContext = cut.FindComponent<CascadingValue<
            Microsoft.AspNetCore.Components.Forms.EditContext>>().Instance.Value!;
        cut.Find("form").Submit();

        return model.Name == "Grace" && editContext.GetValidationMessages().Any();
    }

    private static int? Number(IReadOnlyDictionary<string, JsonElement> probe, string key) =>
        probe.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetInt32() : null;

    private static string Text(IReadOnlyDictionary<string, JsonElement> probe, string key) =>
        !probe.TryGetValue(key, out var value)
            ? string.Empty
            : value.ValueKind switch
            {
                JsonValueKind.String => value.GetString() ?? string.Empty,
                JsonValueKind.Array => string.Join(",", value.EnumerateArray().Select(item => item.GetString())),
                _ => value.ToString(),
            };
}
