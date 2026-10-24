namespace LowlandTech.Components.Tests.CBLK0001.Integration.US01.ActiveOwnedBlocks;

/// <summary>
/// Accesses the Components-owned session block owner contracts by their
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
/// The contracts this accessor expects, all in
/// <c>LowlandTech.Components</c> and owned by the Blocks package:
/// <list type="bullet">
/// <item><c>SessionBlockCandidate(string OwnerKey, string NodeTypeKey, string SymbolicIdentity, int Priority, string Label, string Icon)</c></item>
/// <item><c>SessionBlockOwnerSnapshot(long Revision, IReadOnlyCollection&lt;string&gt; ActiveOwnerKeys)</c></item>
/// <item><c>SessionBlockOwnerBridge</c> with
/// <c>SessionBlockOwnerBridge(ComponentsStartupMap, SessionBlockOwnerSnapshot, IEnumerable&lt;ISessionBlockRegistry&gt;)</c>,
/// <c>Publish(SessionBlockCandidate)</c>, <c>Deliver(SessionBlockOwnerSnapshot)</c>,
/// <c>Revision</c>, <c>Snapshot</c>, <c>IsOwnerActive(string)</c> and the
/// <c>ISessionBlockRegistry</c> member <c>GetDescriptors()</c>.
/// </item>
/// <item><c>SessionBlockCandidateValidation</c> with <c>Candidate</c>,
/// <c>IsValid</c> and <c>Errors</c>.</item>
/// <item><c>SessionBlockCandidateError</c> with <c>Code</c>, <c>Subject</c>
/// and <c>Message</c>.</item>
/// <item><c>SessionBlockCandidateErrorCode</c> with the members
/// <c>DuplicateNodeTypeKey</c>, <c>ConflictingNodeTypeKey</c>,
/// <c>UnresolvedComponent</c> and <c>WrongComponentType</c>.</item>
/// </list>
/// </para>
/// </summary>
public static class ComponentsBlockOwnerContracts
{
    /// <summary>The documented name of the owned candidate contract.</summary>
    public const string CandidateTypeName = "LowlandTech.Components.SessionBlockCandidate";

    /// <summary>The documented name of the owner snapshot contract.</summary>
    public const string SnapshotTypeName = "LowlandTech.Components.SessionBlockOwnerSnapshot";

    /// <summary>The documented name of the block owner bridge contract.</summary>
    public const string BridgeTypeName = "LowlandTech.Components.SessionBlockOwnerBridge";

    /// <summary>The documented name of the candidate validation contract.</summary>
    public const string ValidationTypeName = "LowlandTech.Components.SessionBlockCandidateValidation";

    /// <summary>The documented name of the candidate error contract.</summary>
    public const string ErrorTypeName = "LowlandTech.Components.SessionBlockCandidateError";

    /// <summary>The documented name of the candidate error code contract.</summary>
    public const string ErrorCodeTypeName = "LowlandTech.Components.SessionBlockCandidateErrorCode";

    /// <summary>
    /// Creates the candidate an owner contributes, before it is validated and
    /// published.
    /// </summary>
    public static object CreateCandidate(
        string ownerKey,
        string nodeTypeKey,
        string symbolicIdentity,
        int priority = 0,
        string? label = null,
        string? icon = null)
    {
        var candidateType = RequireType(CandidateTypeName);

        return Activator.CreateInstance(candidateType, ownerKey, nodeTypeKey, symbolicIdentity, priority, label, icon)
               ?? throw new InvalidOperationException(
                   $"The block candidate '{candidateType.FullName}' could not be created.");
    }

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
    /// Creates the bridge that projects the owned candidates onto the session
    /// block contracts.
    /// </summary>
    public static object CreateBridge(
        ComponentsStartupMap startupMap,
        object snapshot,
        IEnumerable<ISessionBlockRegistry> hostRegistries)
    {
        var bridgeType = RequireType(BridgeTypeName);

        return Activator.CreateInstance(bridgeType, startupMap, snapshot, hostRegistries.ToArray())
               ?? throw new InvalidOperationException(
                   $"The block owner bridge '{bridgeType.FullName}' could not be created.");
    }

    /// <summary>Validates a candidate and publishes it when it is valid.</summary>
    public static object Publish(object bridge, object candidate) =>
        Invoke(bridge, "Publish", candidate)!;

    /// <summary>Delivers a new revision of the host authority to the bridge.</summary>
    public static void Deliver(object bridge, object snapshot) =>
        Invoke(bridge, "Deliver", snapshot);

    /// <summary>The revision the bridge currently projects.</summary>
    public static long GetRevision(object bridge) =>
        (long)ReadProperty(bridge, "Revision")!;

    /// <summary>Whether the owner is active in the revision the bridge projects.</summary>
    public static bool IsOwnerActive(object bridge, string ownerKey) =>
        (bool)Invoke(bridge, "IsOwnerActive", ownerKey)!;

    /// <summary>The registry the bridge exposes to a host.</summary>
    public static ISessionBlockRegistry GetRegistry(object bridge) =>
        bridge as ISessionBlockRegistry
        ?? throw new InvalidOperationException(
            $"The block owner bridge '{bridge.GetType().FullName}' is not an {nameof(ISessionBlockRegistry)}.");

    /// <summary>Whether a validation accepted its candidate.</summary>
    public static bool IsValid(object validation) =>
        (bool)ReadProperty(validation, "IsValid")!;

    /// <summary>The named diagnostics a validation reports.</summary>
    public static IReadOnlyList<string> ErrorCodeNames() =>
        Enum.GetNames(RequireType(ErrorCodeTypeName));

    /// <summary>
    /// The named diagnostics of a validation, as the code name, the subject
    /// and the message of every refusal.
    /// </summary>
    public static IReadOnlyList<(string Code, string Subject, string Message)> ReadErrors(object validation)
    {
        var errors = (System.Collections.IEnumerable)ReadProperty(validation, "Errors")!;
        var result = new List<(string, string, string)>();

        foreach (var error in errors)
        {
            var code = ReadProperty(error!, "Code")!;
            var subject = (string)ReadProperty(error!, "Subject")!;
            var message = (string)ReadProperty(error!, "Message")!;

            result.Add((code.ToString()!, subject, message));
        }

        return result;
    }

    private static Type RequireType(string fullName)
    {
        var assembly = typeof(SessionBlockDiscovery).Assembly;
        var type = assembly.GetType(fullName, throwOnError: false);

        return type ?? throw new MissingMemberException(
            $"The Components session block owner contract '{fullName}' is not implemented in " +
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
                $"The Components session block owner contract '{target.GetType().FullName}' does not offer " +
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
                $"The Components session block owner contract '{target.GetType().FullName}' does not expose " +
                $"'{propertyName}'.");
        }

        return property.GetValue(target);
    }
}
