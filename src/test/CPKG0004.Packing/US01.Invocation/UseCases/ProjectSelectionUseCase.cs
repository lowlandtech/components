namespace LowlandTech.Components.Tests.CPKG0004.Packing.US01.Invocation.UseCases;

/// <summary>
/// The result vocabulary of the project-selection diagnosis. Every value starts as
/// <c>null</c> and is set only by the scenario that runs the packing tool.
/// </summary>
public sealed class ProjectSelectionUseCase : global::LowlandTech.Components.Tests.Refinement.IUseCase
{
    public IReadOnlyList<global::LowlandTech.Components.Tests.Refinement.ResultCase> ResultCases =>
    [
        new("JoinedSelectionRefused", JoinedSelectionRefused, "a comma-joined selection is refused with the diagnostic that names the supported array invocation"),
        new("StrayPositionalRefused", StrayPositionalRefused, "a project left as a positional argument is refused with the same named diagnostic"),
        new("SupportedSelectionPacks", SupportedSelectionPacks, "the supported array invocation publishes exactly the two selected packages"),
        new("ConcurrentBatchesPublish", ConcurrentBatchesPublish, "two batches started at the same time both publish without a build-output collision"),
    ];

    /// <summary>UAC01 — the joined selection is refused by name.</summary>
    public bool? JoinedSelectionRefused { get; set; }

    /// <summary>UAC02 — the stray positional is refused by name.</summary>
    public bool? StrayPositionalRefused { get; set; }

    /// <summary>UAC03 — the supported selection packs the two projects.</summary>
    public bool? SupportedSelectionPacks { get; set; }

    /// <summary>UAC04 — two concurrent batches both publish.</summary>
    public bool? ConcurrentBatchesPublish { get; set; }

    /// <summary>The captured output of each invocation, for a failure message.</summary>
    public string JoinedDiagnostic { get; set; } = string.Empty;

    /// <summary>The captured output of each invocation, for a failure message.</summary>
    public string StrayDiagnostic { get; set; } = string.Empty;

    /// <summary>The captured output of each invocation, for a failure message.</summary>
    public string SupportedDiagnostic { get; set; } = string.Empty;

    /// <summary>The captured output of the concurrent pair, for a failure message.</summary>
    public string ConcurrentDiagnostic { get; set; } = string.Empty;

    public Task ArrangeAsync() => Task.CompletedTask;

    public Task InvokeAsync() => Task.CompletedTask;
}
