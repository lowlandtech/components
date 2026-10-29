namespace LowlandTech.Components.Tests.CTRM0001.Trimming.US01.PublishedConsumer.UseCases;

/// <summary>Refinement only: null and NotObserved mean no behavior has been measured.</summary>
public sealed class TrimSafetyUseCase : global::LowlandTech.Components.Tests.Refinement.IUseCase
{
    public IReadOnlyList<global::LowlandTech.Components.Tests.Refinement.ResultCase> ResultCases =>
    [
        new("CanonicalWarningsAbsent", "NotObserved", "TODO: runnable trimmed browser consumer of exact published package set and diagnostic provenance excluding generated summary duplicates"),
        new("PersistedShapeRetained", "NotObserved", "TODO: real existing DataGrid/ContainerGrid persistence adapters reading both pre-change and new payloads without reflection"),
        new("DesignMetadataRetained", "NotObserved", "TODO: actual ComponentDescriptorRegistry discovery preserving PropertiesAttribute and resolved options"),
        new("DataFormBehaviorRetained", "NotObserved", "TODO: actual model getter/setter and DataAnnotations validation without broad trim suppression"),
        new("SuiteAndPackagePinRetained", "NotObserved", "TODO: native full suite and exact compatible Core/Components pin checks; no library-as-app trimmed publish"),
    ];

    public bool? CanonicalWarningsAbsent { get; private set; }

    public bool? PersistedShapeRetained { get; private set; }

    public bool? DesignMetadataRetained { get; private set; }

    public bool? DataFormBehaviorRetained { get; private set; }

    public bool? SuiteAndPackagePinRetained { get; private set; }

    public Task ArrangeAsync() => Task.CompletedTask;

    public Task InvokeAsync() => Task.CompletedTask;
}
