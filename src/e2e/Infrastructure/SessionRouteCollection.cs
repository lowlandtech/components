namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// The collection of the browser proofs of the packaged session route.
/// </summary>
/// <remarks>
/// <para>
/// The proofs of the session route publish and serve their own consumer
/// application, so they carry no collection fixture and never boot the
/// Showcode WASM demo host: the consumer proofs must not depend on the demo
/// host.
/// </para>
/// <para>
/// The proofs share one collection, so they never run beside each other. Both
/// of them restore the published batch of the shared feed, publish a consumer
/// and drive a real browser against it, and a machine that restores and
/// publishes two consumers at once starves the route of the resources it needs
/// to hydrate.
/// </para>
/// <para>
/// The collection disables parallelization, so the proofs never run beside the
/// Showcode scenarios that drive a browser against that demo host. Both drive a
/// real browser, and running them at once starves the demo host of the
/// resources it needs to hydrate.
/// </para>
/// </remarks>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SessionRouteCollection
{
    /// <summary>The name of the collection.</summary>
    public const string Name = "session-route";
}
