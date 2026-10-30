namespace LowlandTech.Components.Tests.CTRM0001.Trimming.US01.PublishedConsumer.UseCases;

/// <summary>
/// The result vocabulary of the trim preservation probe. Every value starts as
/// <c>null</c> — not observed — and is set only by the scenario that runs the probe,
/// so an unexecuted Fact can never read as a pass.
/// </summary>
public sealed class TrimSafetyUseCase : global::LowlandTech.Components.Tests.Refinement.IUseCase
{
    public IReadOnlyList<global::LowlandTech.Components.Tests.Refinement.ResultCase> ResultCases =>
    [
        new("CanonicalWarningsAbsent", CanonicalWarningsAbsent, "the trimmed publish of the consumer reports no trim warning that names a components assembly"),
        new("PersistedShapeRetained", PersistedShapeRetained, "the persisted session prompt and container-grid layout payloads round-trip in the trimmed consumer"),
        new("DesignMetadataRetained", DesignMetadataRetained, "the descriptor registry still discovers the designable component with its properties, label and options in the trimmed consumer"),
        new("DataFormBehaviorRetained", DataFormBehaviorRetained, "the DataForm still discovers and renders the fields of its model in the trimmed consumer"),
        new("SuiteAndPackagePinRetained", SuiteAndPackagePinRetained, "the packed batch declares the exact Core and contracts pair, and the trimmed consumer restores and runs against it"),
    ];

    /// <summary>UAC01 — the trimmed publish names no components assembly.</summary>
    public bool? CanonicalWarningsAbsent { get; set; }

    /// <summary>UAC02 — the persisted shapes survive the trim.</summary>
    public bool? PersistedShapeRetained { get; set; }

    /// <summary>UAC03 — discovery keeps the design metadata.</summary>
    public bool? DesignMetadataRetained { get; set; }

    /// <summary>UAC04 — the DataForm still reads its model.</summary>
    public bool? DataFormBehaviorRetained { get; set; }

    /// <summary>UAC05 — the batch pins the exact pair and the consumer runs.</summary>
    public bool? SuiteAndPackagePinRetained { get; set; }

    public Task ArrangeAsync() => Task.CompletedTask;

    public Task InvokeAsync() => Task.CompletedTask;
}
