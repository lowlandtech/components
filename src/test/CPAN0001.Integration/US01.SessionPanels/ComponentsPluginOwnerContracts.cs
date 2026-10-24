using System.Reflection;

namespace LowlandTech.Components.Tests.CPAN0001.Integration.US01.SessionPanels;

/// <summary>
/// Accesses the Components-owned plugin owner bridge contracts by their
/// documented names.
///
/// <para>
/// The scope is refined before it is executed, so the acceptance inventory has
/// to compile while the production contracts it accepts do not exist yet. The
/// contracts are therefore reached by name instead of by a compile-time
/// reference. Every lookup fails with the exact missing contract name, so an
/// unimplemented contract can never turn into a silent pass: the Fact fails
/// with the contract it still owes.
/// </para>
/// <para>
/// The contract this accessor expects, all in
/// <c>LowlandTech.Components</c>:
/// <list type="bullet">
/// <item><c>SessionPluginOwnerSnapshot(long Revision, IReadOnlyCollection&lt;string&gt; ActiveOwnerKeys)</c></item>
/// <item><c>SessionPluginOwnerBridge</c> with
/// <c>SessionPluginOwnerBridge(SessionPluginOwnerSnapshot, IEnumerable&lt;ISessionPlugin&gt;, ISessionPluginSettingsStore)</c>,
/// <c>Register(string ownerKey, ISessionPlugin plugin)</c>, <c>Deliver(SessionPluginOwnerSnapshot)</c>,
/// <c>Subscribe(Action&lt;long&gt; onRevision)</c>, <c>Revision</c>, <c>Snapshot</c>,
/// <c>IsOwnerActive(string ownerKey)</c> and the <c>ISessionPluginRegistry</c>
/// members <c>GetAllPlugins()</c> and <c>ListAsync</c>.
/// </list>
/// </para>
/// </summary>
public static class ComponentsPluginOwnerContracts
{
    /// <summary>The documented name of the owner snapshot contract.</summary>
    public const string SnapshotTypeName = "LowlandTech.Components.SessionPluginOwnerSnapshot";

    /// <summary>The documented name of the owner bridge contract.</summary>
    public const string BridgeTypeName = "LowlandTech.Components.SessionPluginOwnerBridge";

    /// <summary>
    /// Creates the snapshot that carries the host authority at one revision.
    /// </summary>
    public static object CreateSnapshot(long revision, IEnumerable<string> activeOwnerKeys)
    {
        var snapshotType = RequireType(SnapshotTypeName);

        return Activator.CreateInstance(snapshotType, revision, activeOwnerKeys.ToArray())
               ?? throw new InvalidOperationException(
                   $"The owner snapshot '{snapshotType.FullName}' could not be created.");
    }

    /// <summary>
    /// Creates the bridge that projects the active owners onto the session
    /// panel contracts.
    /// </summary>
    public static object CreateBridge(
        object snapshot,
        IEnumerable<ISessionPlugin> plugins,
        ISessionPluginSettingsStore? settings)
    {
        var bridgeType = RequireType(BridgeTypeName);

        return Activator.CreateInstance(bridgeType, snapshot, plugins.ToArray(), settings)
               ?? throw new InvalidOperationException(
                   $"The owner bridge '{bridgeType.FullName}' could not be created.");
    }

    /// <summary>Registers the panel an owner contributes.</summary>
    public static void Register(object bridge, string ownerKey, ISessionPlugin plugin) =>
        Invoke(bridge, "Register", ownerKey, plugin);

    /// <summary>Delivers a new revision to the bridge.</summary>
    public static void Deliver(object bridge, object snapshot) =>
        Invoke(bridge, "Deliver", snapshot);

    /// <summary>Subscribes to the revisions the bridge delivers.</summary>
    public static IDisposable Subscribe(object bridge, Action<long> onRevision) =>
        (IDisposable)Invoke(bridge, "Subscribe", onRevision)!;

    /// <summary>The revision the bridge currently projects.</summary>
    public static long GetRevision(object bridge) =>
        (long)ReadProperty(bridge, "Revision")!;

    /// <summary>Whether the owner is active in the revision the bridge projects.</summary>
    public static bool IsOwnerActive(object bridge, string ownerKey) =>
        (bool)Invoke(bridge, "IsOwnerActive", ownerKey)!;

    /// <summary>The registry the bridge exposes to a host.</summary>
    public static ISessionPluginRegistry GetRegistry(object bridge) =>
        bridge as ISessionPluginRegistry
        ?? throw new InvalidOperationException(
            $"The owner bridge '{bridge.GetType().FullName}' is not an {nameof(ISessionPluginRegistry)}.");

    /// <summary>
    /// Whether the session plugin host exposes the host placement as a
    /// parameter, so a host can select its placement instead of always taking
    /// the right sidebar.
    /// </summary>
    public static bool HostDeclaresPlacementParameter() =>
        typeof(SessionPluginHost).GetProperty("Placement")?.PropertyType == typeof(SessionPluginPlacement);

    private static Type RequireType(string fullName)
    {
        var assembly = typeof(SessionPluginRegistry).Assembly;
        var type = assembly.GetType(fullName, throwOnError: false);

        return type ?? throw new MissingMemberException(
            $"The Components plugin owner contract '{fullName}' is not implemented in " +
            $"'{assembly.GetName().Name}'. The scope cannot be accepted until it exists.");
    }

    private static object? Invoke(object target, string methodName, params object?[] arguments)
    {
        var method = target.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(candidate =>
                candidate.Name == methodName &&
                candidate.GetParameters().Length == arguments.Length);

        if (method is null)
        {
            throw new MissingMethodException(
                $"The Components plugin owner contract '{target.GetType().FullName}' does not offer " +
                $"'{methodName}' with {arguments.Length} argument(s).");
        }

        return method.Invoke(target, arguments);
    }

    private static object? ReadProperty(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        if (property is null)
        {
            throw new MissingMemberException(
                $"The Components plugin owner contract '{target.GetType().FullName}' does not expose " +
                $"'{propertyName}'.");
        }

        return property.GetValue(target);
    }
}
