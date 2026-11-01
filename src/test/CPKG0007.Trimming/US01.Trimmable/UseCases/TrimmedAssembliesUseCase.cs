namespace LowlandTech.Components.Tests.CPKG0007.Trimming.US01.Trimmable.UseCases;

/// <summary>
/// The result vocabulary of the trimmed-assembly acceptance. Every value starts as
/// <c>null</c> and is set only by the scenario that runs the trimmed publish.
/// </summary>
public sealed class TrimmedAssembliesUseCase : global::LowlandTech.Components.Tests.Refinement.IUseCase
{
    public IReadOnlyList<global::LowlandTech.Components.Tests.Refinement.ResultCase> ResultCases =>
    [
        new("AnalyzerSilentWhenTrimmable", AnalyzerSilentWhenTrimmable, "the pack of the trimmable projects reports no trim analyzer warning"),
        new("AssembliesAreTrimmed", AssembliesAreTrimmed, "both shared assemblies are smaller in the trimmed publish than in the package, and the trimmed consumer still runs"),
        new("ContractsAreDeclared", ContractsAreDeclared, "the packed assembly declares the trimmable metadata and the two public type members declare their members"),
        new("BatchRestoresCleanly", BatchRestoresCleanly, "the batch restores with the current Core and reports no NU1107 or NU1608"),
    ];

    /// <summary>UAC01 — the analyzer stays silent with the trimmable declaration.</summary>
    public bool? AnalyzerSilentWhenTrimmable { get; set; }

    /// <summary>UAC02 — the assemblies are trimmed, not copied.</summary>
    public bool? AssembliesAreTrimmed { get; set; }

    /// <summary>UAC03 — the annotations and the trimmable metadata are published.</summary>
    public bool? ContractsAreDeclared { get; set; }

    /// <summary>UAC04 — the batch restores cleanly with the current Core.</summary>
    public bool? BatchRestoresCleanly { get; set; }

    /// <summary>The measured sizes, for a failure message.</summary>
    public string Sizes { get; set; } = string.Empty;

    public Task ArrangeAsync() => Task.CompletedTask;

    public Task InvokeAsync() => Task.CompletedTask;
}
