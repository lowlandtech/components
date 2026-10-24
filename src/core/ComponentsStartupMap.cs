namespace LowlandTech.Components.Core;

/// <summary>
/// The trusted startup map of a host: the symbolic identities the host can
/// resolve and the logical keys the host registered.
///
/// <para>
/// The map is the only place a symbolic identity becomes a CLR type. A feature
/// that is not activated resolves nothing, and an identity the manifest does
/// not declare is never resolved, whatever the map holds.
/// </para>
/// </summary>
public sealed class ComponentsStartupMap
{
    private readonly Dictionary<string, ComponentsStartupBinding> _bindings;
    private readonly HashSet<string> _logicalKeys;

    /// <summary>
    /// Creates a startup map from the bindings and logical keys a host
    /// registered.
    /// </summary>
    /// <param name="bindings">The trusted startup bindings of the host.</param>
    /// <param name="logicalKeys">The logical keys the host registered.</param>
    public ComponentsStartupMap(
        IEnumerable<ComponentsStartupBinding> bindings,
        IEnumerable<string> logicalKeys)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(logicalKeys);

        _bindings = new Dictionary<string, ComponentsStartupBinding>(StringComparer.Ordinal);

        foreach (var binding in bindings)
        {
            ArgumentNullException.ThrowIfNull(binding);

            if (!_bindings.TryAdd(binding.SymbolicIdentity, binding))
            {
                throw new ArgumentException(
                    $"The symbolic identity '{binding.SymbolicIdentity}' is bound twice.",
                    nameof(bindings));
            }
        }

        _logicalKeys = new HashSet<string>(logicalKeys, StringComparer.Ordinal);
    }

    /// <summary>
    /// Composes a startup map from the registrations of the packages a host
    /// uses.
    /// </summary>
    /// <param name="registrations">The registrations of the host packages.</param>
    public static ComponentsStartupMap From(IEnumerable<ComponentsStartupRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        var bindings = new List<ComponentsStartupBinding>();
        var logicalKeys = new List<string>();

        foreach (var registration in registrations)
        {
            ArgumentNullException.ThrowIfNull(registration);

            logicalKeys.Add(registration.LogicalKey);

            if (registration.Bindings is null)
            {
                continue;
            }

            bindings.AddRange(registration.Bindings);
        }

        return new ComponentsStartupMap(bindings, logicalKeys);
    }

    /// <summary>
    /// Resolves a symbolic identity through the trusted startup map.
    /// </summary>
    /// <param name="symbolicIdentity">The symbolic identity to resolve.</param>
    /// <param name="componentType">The type the identity resolves to.</param>
    public bool TryResolveComponent(string symbolicIdentity, out Type componentType)
    {
        if (symbolicIdentity is not null && _bindings.TryGetValue(symbolicIdentity, out var binding))
        {
            componentType = binding.ComponentType;
            return true;
        }

        componentType = null!;
        return false;
    }

    /// <summary>
    /// Whether the host registered the logical key.
    /// </summary>
    /// <param name="logicalKey">The logical key to look for.</param>
    public bool ContainsLogicalKey(string logicalKey) =>
        logicalKey is not null && _logicalKeys.Contains(logicalKey);

    /// <summary>The logical keys the host registered.</summary>
    public IReadOnlyCollection<string> LogicalKeys => _logicalKeys.ToArray();

    /// <summary>The symbolic identities the host bound.</summary>
    public IReadOnlyCollection<string> ComponentIdentities => _bindings.Keys.ToArray();
}
