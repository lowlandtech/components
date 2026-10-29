namespace LowlandTech.Components.Tests.CTRM0001.Trimming.US01.PublishedConsumer;

[Trait("CTRM", "0001"), Trait("US", "01"), Trait("SC", "01")]
[UserStory("01", "TrimSafety")]
public sealed class SC01_TrimSafety(global::LowlandTech.Components.Tests.Refinement.ScenarioFixture<UseCases.TrimSafetyUseCase> fixture) : global::LowlandTech.Components.Tests.Refinement.UseCaseScenario<UseCases.TrimSafetyUseCase>(fixture)
{
    [Given("the source-owned fixtures and current accepted package or host checkpoint")]
    protected override Task GivenAsync() => UseCase.ArrangeAsync();

    [When("the TrimSafety use case observes its declared success and refusal boundaries")]
    protected override Task WhenAsync() => UseCase.InvokeAsync();

    [Then("Result.CanonicalWarningsAbsent should be true")]
    [Trait("UAC", "01"), Fact(DisplayName = "UAC01: captured result should be true", Skip = "TODO: CTRM0001-US01-SC01: implement runnable trimmed browser consumer of exact published package set and diagnostic provenance excluding generated summary duplicates.")]
    public void UAC01_CanonicalWarningsAbsent() => Result.CanonicalWarningsAbsent.ShouldBe(true);

    [Then("Result.PersistedShapeRetained should be true")]
    [Trait("UAC", "02"), Fact(DisplayName = "UAC02: captured result should be true", Skip = "TODO: CTRM0001-US01-SC01: implement real existing DataGrid/ContainerGrid persistence adapters reading both pre-change and new payloads without reflection.")]
    public void UAC02_PersistedShapeRetained() => Result.PersistedShapeRetained.ShouldBe(true);

    [Then("Result.DesignMetadataRetained should be true")]
    [Trait("UAC", "03"), Fact(DisplayName = "UAC03: captured result should be true", Skip = "TODO: CTRM0001-US01-SC01: implement actual ComponentDescriptorRegistry discovery preserving PropertiesAttribute and resolved options.")]
    public void UAC03_DesignMetadataRetained() => Result.DesignMetadataRetained.ShouldBe(true);

    [Then("Result.DataFormBehaviorRetained should be true")]
    [Trait("UAC", "04"), Fact(DisplayName = "UAC04: captured result should be true", Skip = "TODO: CTRM0001-US01-SC01: implement actual model getter/setter and DataAnnotations validation without broad trim suppression.")]
    public void UAC04_DataFormBehaviorRetained() => Result.DataFormBehaviorRetained.ShouldBe(true);

    [Then("Result.SuiteAndPackagePinRetained should be true")]
    [Trait("UAC", "05"), Fact(DisplayName = "UAC05: captured result should be true", Skip = "TODO: CTRM0001-US01-SC01: implement native full suite and exact compatible Core/Components pin checks; no library-as-app trimmed publish.")]
    public void UAC05_SuiteAndPackagePinRetained() => Result.SuiteAndPackagePinRetained.ShouldBe(true);
}
