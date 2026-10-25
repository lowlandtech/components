using LowlandTech.Components.Core;

namespace LowlandTech.Components;

/// <summary>
/// The bridge that projects the session blocks of the active plugin owners onto
/// the session block contracts of the host.
/// </summary>
/// <remarks>
/// <para>
/// A loaded type is not proof of active ownership. The bridge closes that path:
/// an owned type is reachable only through the validated node type key of an
/// owner that is active, never under the type name the assembly-scanning
/// fallback publishes it under.
/// </para>
/// <para>
/// The bridge wraps the host registries and publishes their descriptors beside
/// its own, so a host default keeps its own node type key and becomes the
/// fallback again once the owner that overrode it withdraws. The base DI-only
/// mode keeps registering <see cref="DefaultSessionBlockRegistry"/> and
/// <see cref="AssemblyScanningSessionBlockRegistry"/> directly and stays
/// unchanged.
/// </para>
/// <para>
/// The bridge holds no retained fragment and needs no subscriber contract:
/// every lookup reads the revision the bridge holds.
/// </para>
/// </remarks>
public sealed class SessionBlockOwnerBridge : ISessionBlockRegistry
{
    private readonly ComponentsStartupMap _startupMap;
    private readonly IReadOnlyList<ISessionBlockRegistry> _hostRegistries;
    private readonly Dictionary<string, PublishedCandidate> _published = new(StringComparer.Ordinal);
    private SessionBlockOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the bridge over the trusted startup map, the host authority and
    /// the registries of the host.
    /// </summary>
    /// <param name="startupMap">The trusted startup map the symbolic identities resolve through.</param>
    /// <param name="snapshot">The host authority at its first revision.</param>
    /// <param name="hostRegistries">The registries of the host the bridge wraps.</param>
    public SessionBlockOwnerBridge(
        ComponentsStartupMap startupMap,
        SessionBlockOwnerSnapshot snapshot,
        IEnumerable<ISessionBlockRegistry> hostRegistries)
    {
        ArgumentNullException.ThrowIfNull(startupMap);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(hostRegistries);

        _startupMap = startupMap;
        _snapshot = snapshot;
        _hostRegistries = hostRegistries.ToArray();
    }

    /// <summary>The revision of the host authority the bridge projects.</summary>
    public long Revision => _snapshot.Revision;

    /// <summary>The host authority the bridge projects.</summary>
    public SessionBlockOwnerSnapshot Snapshot => _snapshot;

    /// <summary>
    /// Whether the owner is active in the revision the bridge projects.
    /// </summary>
    /// <param name="ownerKey">The logical key of the owner.</param>
    public bool IsOwnerActive(string ownerKey) =>
        ownerKey is not null
        && _snapshot.ActiveOwnerKeys.Contains(ownerKey, StringComparer.Ordinal);

    /// <summary>
    /// Validates a candidate and publishes it when it is valid. A refused
    /// candidate changes nothing: the published lookup stays exactly as it was.
    /// </summary>
    /// <param name="candidate">The candidate an owner contributes.</param>
    /// <returns>The outcome of the validation.</returns>
    public SessionBlockCandidateValidation Publish(SessionBlockCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        var errors = new List<SessionBlockCandidateError>();

        if (!_startupMap.TryResolveComponent(candidate.SymbolicIdentity, out var componentType))
        {
            errors.Add(new SessionBlockCandidateError(
                SessionBlockCandidateErrorCode.UnresolvedComponent,
                candidate.SymbolicIdentity,
                $"The symbolic identity '{candidate.SymbolicIdentity}' does not resolve through the " +
                "trusted startup map of the host."));
        }
        else if (!IsRenderableComponent(componentType))
        {
            errors.Add(new SessionBlockCandidateError(
                SessionBlockCandidateErrorCode.WrongComponentType,
                candidate.SymbolicIdentity,
                $"The symbolic identity '{candidate.SymbolicIdentity}' resolves to " +
                $"'{componentType.FullName}', which is not a concrete Blazor component."));
        }

        // The node type key stays reserved by the owner that claimed it,
        // whether that owner is active or not, so a second owner cannot take
        // the key over while the first one may return.
        if (_published.TryGetValue(candidate.NodeTypeKey, out var existing))
        {
            var sameOwner = string.Equals(
                existing.Candidate.OwnerKey,
                candidate.OwnerKey,
                StringComparison.Ordinal);

            errors.Add(new SessionBlockCandidateError(
                sameOwner
                    ? SessionBlockCandidateErrorCode.DuplicateNodeTypeKey
                    : SessionBlockCandidateErrorCode.ConflictingNodeTypeKey,
                candidate.NodeTypeKey,
                $"The node type key '{candidate.NodeTypeKey}' is already claimed by the owner " +
                $"'{existing.Candidate.OwnerKey}'."));
        }

        if (errors.Count > 0)
        {
            return new SessionBlockCandidateValidation(candidate, false, errors);
        }

        _published[candidate.NodeTypeKey] = new PublishedCandidate(candidate, componentType!);

        return new SessionBlockCandidateValidation(candidate, true, []);
    }

    /// <summary>
    /// Delivers the next revision of the host authority. The owners of that
    /// revision decide which blocks are published.
    /// </summary>
    /// <param name="snapshot">The host authority of the next revision.</param>
    public void Deliver(SessionBlockOwnerSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        _snapshot = snapshot;
    }

    /// <summary>
    /// The descriptors of the host beside the blocks of the active owners. The
    /// assembly-scanning shape of an owned type is suppressed, so an owned type
    /// is reachable only through its validated node type key.
    /// </summary>
    public IReadOnlyList<ISessionBlockDescriptor> GetDescriptors()
    {
        var ownedTypes = _published.Values
            .Select(entry => entry.ComponentType)
            .ToHashSet();

        var descriptors = new List<ISessionBlockDescriptor>();

        foreach (var registry in _hostRegistries)
        {
            foreach (var descriptor in registry.GetDescriptors())
            {
                if (descriptor.ComponentType is not null
                    && ownedTypes.Contains(descriptor.ComponentType)
                    && string.Equals(
                        descriptor.NodeTypeKey,
                        descriptor.ComponentType.Name,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                descriptors.Add(descriptor);
            }
        }

        foreach (var entry in _published.Values)
        {
            if (!IsOwnerActive(entry.Candidate.OwnerKey))
            {
                continue;
            }

            descriptors.Add(new SessionBlockDescriptor(
                entry.Candidate.NodeTypeKey,
                entry.ComponentType,
                entry.Candidate.Label,
                entry.Candidate.Icon,
                entry.Candidate.Priority));
        }

        return descriptors;
    }

    /// <summary>
    /// Whether the type is a concrete, closed Blazor component a session block
    /// can render.
    /// </summary>
    private static bool IsRenderableComponent(Type type) =>
        typeof(IComponent).IsAssignableFrom(type)
        && type.IsClass
        && type.IsVisible
        && !type.IsAbstract
        && !type.ContainsGenericParameters;

    /// <summary>A published candidate and the type its identity resolved to.</summary>
    private sealed record PublishedCandidate(SessionBlockCandidate Candidate, Type ComponentType);
}
