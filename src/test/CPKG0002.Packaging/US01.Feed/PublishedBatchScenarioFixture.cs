using LowlandTech.Components.Tests.CPKG0001.Packaging;

namespace LowlandTech.Components.Tests.CPKG0002.Packaging.US01.Feed;

/// <summary>
/// Runs the packaging scenario twice for the whole acceptance class: once with
/// the switch that consumes the published batch of the shared feed and once
/// without it. Both runs share their artifacts between the Facts of the class.
/// </summary>
public sealed class PublishedBatchScenarioFixture : IDisposable
{
    /// <summary>Root of the repository the packages were packed from.</summary>
    public string RepositoryRoot { get; }

    /// <summary>The scenario that consumed the published batch of the shared feed.</summary>
    public ConsumerScenario Published { get; }

    /// <summary>The scenario that packed the packages of this repository.</summary>
    public ConsumerScenario Default { get; }

    public PublishedBatchScenarioFixture()
    {
        RepositoryRoot = ConsumerScenario.FindRepositoryRoot();
        Published = ConsumerScenario.Run(RepositoryRoot, usePublishedBatch: true);
        Default = ConsumerScenario.Run(RepositoryRoot);
    }

    public void Dispose()
    {
        Published.Dispose();
        Default.Dispose();
    }
}
