namespace LowlandTech.Components.E2E.CPGS0001.Integration;

/// <summary>
/// The collection of the CPGS0001 browser scenarios.
/// </summary>
/// <remarks>
/// <para>
/// The CPGS0001 browser proof publishes and serves its own consumer
/// application, so it carries no collection fixture and never boots the
/// Showcode WASM demo host: the consumer proof of this scope must not depend
/// on the demo host.
/// </para>
/// <para>
/// The collection disables parallelization, so the scenario never runs beside
/// the Showcode scenarios that drive a browser against that demo host. Both
/// drive a real browser, and running them at once starves the demo host of
/// the resources it needs to hydrate.
/// </para>
/// </remarks>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class CPGS0001Collection
{
    /// <summary>The name of the collection.</summary>
    public const string Name = "CPGS0001";
}
