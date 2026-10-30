namespace LowlandTech.Components.Tests.CPKG0003.Packing.US01.Declaration.UseCases;

/// <summary>
/// The result vocabulary of the exact batch declaration. Every value starts as
/// <c>null</c> and is set only by the scenario that runs the packing tool.
/// </summary>
public sealed class ExactDeclarationUseCase : global::LowlandTech.Components.Tests.Refinement.IUseCase
{
    public IReadOnlyList<global::LowlandTech.Components.Tests.Refinement.ResultCase> ResultCases =>
    [
        new("ExactDeclaration", ExactDeclaration, "the packed nuspec declares the contracts generation and the Core as exact ranges"),
        new("PrefixSortsAbovePublished", PrefixSortsAbovePublished, "the manifest prefix sorts above the batches the feed already holds"),
        new("ExistingVersionRefused", ExistingVersionRefused, "a version the feed already holds is refused"),
        new("EmptySelectionRefused", EmptySelectionRefused, "an explicitly empty project selection is refused with a named diagnostic"),
    ];

    /// <summary>UAC01 — the dependency declarations are exact.</summary>
    public bool? ExactDeclaration { get; set; }

    /// <summary>UAC02 — the prefix sorts above the published batches.</summary>
    public bool? PrefixSortsAbovePublished { get; set; }

    /// <summary>UAC03 — an existing version is refused.</summary>
    public bool? ExistingVersionRefused { get; set; }

    /// <summary>UAC04 — an empty selection is refused.</summary>
    public bool? EmptySelectionRefused { get; set; }

    public Task ArrangeAsync() => Task.CompletedTask;

    public Task InvokeAsync() => Task.CompletedTask;
}
