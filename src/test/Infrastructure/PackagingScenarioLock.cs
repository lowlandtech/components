namespace LowlandTech.Components.Tests.Infrastructure;

/// <summary>
/// Holds the packaging scenarios of the acceptance inventory apart from each
/// other.
///
/// <para>
/// Two packaging scenarios pack the same Components projects in the Release
/// configuration, and the test host runs the acceptance classes of one
/// assembly in parallel. Two concurrent packs write the same intermediate and
/// output files, so the second build fails with <c>CS2012</c> or
/// <c>MSB3026</c> because the file is locked by the first one. The scenarios
/// therefore take the same lock, and the acceptance gate stays deterministic.
/// </para>
/// </summary>
public static class PackagingScenarioLock
{
    private static readonly SemaphoreSlim Gate = new(1, 1);

    /// <summary>
    /// Runs an action while the other packaging scenarios are held back.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <typeparam name="T">The result of the action.</typeparam>
    public static T Run<T>(Func<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        Gate.Wait();

        try
        {
            return action();
        }
        finally
        {
            Gate.Release();
        }
    }
}
