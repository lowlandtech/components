namespace LowlandTech.Components.Tests.CPKG0001.Packaging.US01.Packaging;

/// <summary>
/// Runs the CPKG0001 packaging scenario once for the whole acceptance class
/// and shares the produced artifacts between the Facts of that class.
/// </summary>
public sealed class ConsumerScenarioFixture : IDisposable
{
    /// <summary>Root of the repository the packages were packed from.</summary>
    public string RepositoryRoot { get; }

    /// <summary>The artifacts of one packaging scenario run.</summary>
    public ConsumerScenario Scenario { get; }

    public ConsumerScenarioFixture()
    {
        RepositoryRoot = ConsumerScenario.FindRepositoryRoot();
        Scenario = ConsumerScenario.Run(RepositoryRoot);
    }

    public void Dispose()
    {
        Scenario.Dispose();
    }
}
