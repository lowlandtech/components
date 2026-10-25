using System.Reflection;
using LowlandTech.Components.Core;
using LowlandTech.Core.Plugins;
using LowlandTech.Plugins;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// The bridge that projects the pages of the active plugin owners onto the
/// routing and navigation contracts of the host.
/// </summary>
/// <remarks>
/// <para>
/// A host owns which plugin owners are active. It delivers that authority to
/// the bridge as a revision, and the bridge publishes exactly the pages of the
/// owners of that revision.
/// </para>
/// <para>
/// Validation gates the candidate and the owner activity gates the projection.
/// A page of an owner that is not active is a valid candidate, it is simply not
/// published. That keeps a withdrawal reversible: the next revision that names
/// the owner active publishes the page again without a new registration.
/// </para>
/// <para>
/// The bridge never resolves a component by assembly scanning. It reads the
/// host's trusted startup map, and it refuses a candidate whose identity does
/// not resolve, does not resolve to a renderable component, or carries no page
/// metadata. The adapted metadata keeps the Plugins owned shape, so a host
/// hands it to the Plugins page catalog unchanged.
/// </para>
/// </remarks>
public sealed class ComponentPageOwnerBridge
{
    private readonly ComponentsStartupMap _startupMap;
    private readonly HashSet<string> _reservedHostRoutes;
    private readonly Dictionary<(string OwnerKey, string SymbolicIdentity), PublishedPage> _published = new();
    private ComponentPageOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the bridge for a host.
    /// </summary>
    /// <param name="startupMap">The trusted startup map of the host.</param>
    /// <param name="snapshot">The host authority at its first revision.</param>
    /// <param name="reservedHostRoutes">
    /// The routes the host serves itself. A plugin page may not claim one.
    /// </param>
    public ComponentPageOwnerBridge(
        ComponentsStartupMap startupMap,
        ComponentPageOwnerSnapshot snapshot,
        IEnumerable<string> reservedHostRoutes)
    {
        ArgumentNullException.ThrowIfNull(startupMap);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(reservedHostRoutes);

        _startupMap = startupMap;
        _snapshot = snapshot;
        _reservedHostRoutes = new HashSet<string>(reservedHostRoutes, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>The revision of the host authority the bridge projects.</summary>
    public long Revision => _snapshot.Revision;

    /// <summary>The host authority the bridge projects.</summary>
    public ComponentPageOwnerSnapshot Snapshot => _snapshot;

    /// <summary>
    /// Whether the owner is active in the revision the bridge holds.
    /// </summary>
    /// <param name="ownerKey">The logical key of the owner.</param>
    public bool IsOwnerActive(string ownerKey) =>
        ownerKey is not null
        && _snapshot.ActiveOwnerKeys.Contains(ownerKey, StringComparer.Ordinal);

    /// <summary>
    /// Validates a page candidate an owner contributes. An invalid candidate
    /// never changes the published projection.
    /// </summary>
    /// <param name="binding">The candidate an owner contributes.</param>
    /// <returns>The outcome of the validation.</returns>
    public ComponentPageBindingValidation Publish(ComponentPageBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        var errors = new List<ComponentPageBindingError>();
        PluginPageAttribute? attribute = null;
        Type? componentType = null;

        if (!_startupMap.TryResolveComponent(binding.SymbolicIdentity, out componentType))
        {
            errors.Add(new ComponentPageBindingError(
                ComponentPageBindingErrorCode.UnresolvedComponent,
                binding.SymbolicIdentity,
                $"The symbolic identity '{binding.SymbolicIdentity}' does not resolve through the " +
                "trusted startup map of the host."));
        }
        else if (!IsRenderableComponent(componentType))
        {
            errors.Add(new ComponentPageBindingError(
                ComponentPageBindingErrorCode.WrongComponentType,
                binding.SymbolicIdentity,
                $"The symbolic identity '{binding.SymbolicIdentity}' resolves to " +
                $"'{componentType.FullName}', which is not a renderable Blazor component."));
        }
        else if ((attribute = componentType.GetCustomAttribute<PluginPageAttribute>(inherit: true)) is null)
        {
            errors.Add(new ComponentPageBindingError(
                ComponentPageBindingErrorCode.MissingPageMetadata,
                binding.SymbolicIdentity,
                $"The component '{componentType.FullName}' carries no page metadata, so it is not a page."));
        }

        AddBindingErrors(binding, errors);

        if (errors.Count > 0)
        {
            return new ComponentPageBindingValidation(binding, false, errors);
        }

        _published[(binding.OwnerKey, binding.SymbolicIdentity)] =
            new PublishedPage(binding, Metadata(componentType!, attribute!));

        return new ComponentPageBindingValidation(binding, true, []);
    }

    /// <summary>
    /// Delivers the next revision of the host authority. The owners of that
    /// revision decide which pages are published.
    /// </summary>
    /// <param name="snapshot">The host authority of the next revision.</param>
    public void Deliver(ComponentPageOwnerSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        _snapshot = snapshot;
    }

    /// <summary>
    /// The pages of the owners that are active in the revision the bridge holds.
    /// </summary>
    public IReadOnlyList<ComponentPageBinding> Enumerate() =>
        ActivePages().Select(page => page.Binding).ToList();

    /// <summary>
    /// The page that owns a route, or null when no active owner serves it.
    /// </summary>
    /// <param name="routePath">The route to resolve.</param>
    public ComponentPageBinding? Resolve(string routePath) =>
        routePath is null
            ? null
            : Enumerate().FirstOrDefault(binding =>
                string.Equals(binding.RoutePath, routePath, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// The navigation entries of the pages of the active owners. A page that
    /// keeps itself out of the navigation stays routable and stays out of here.
    /// </summary>
    public IReadOnlyList<ComponentPageNavigationEntry> Navigation() =>
        ActivePages()
            .Where(page => page.Metadata.ShowInNav)
            .Select(page => new ComponentPageNavigationEntry(
                page.Binding.RoutePath,
                page.Metadata.Label,
                page.Metadata.Icon,
                page.Metadata.Order,
                page.Metadata.Group,
                page.Metadata.SubGroup))
            .OrderBy(entry => entry.Order)
            .ThenBy(entry => entry.RoutePath, StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// The pages of the active owners, keyed the way the Plugins route gateway
    /// keys them.
    /// </summary>
    public ComponentPageCatalog PageCatalog() =>
        new(ActivePages()
            .Select(page => new KeyValuePair<(string PluginKey, string Component), PluginPageMetadata>(
                (page.Binding.OwnerKey, page.Binding.SymbolicIdentity),
                page.Metadata)));

    /// <summary>
    /// The published pages whose owner is active in the revision the bridge holds.
    /// </summary>
    private IReadOnlyList<PublishedPage> ActivePages() =>
        _published.Values
            .Where(page => IsOwnerActive(page.Binding.OwnerKey))
            .ToList();

    /// <summary>
    /// Adds the refusals that concern the identity and the route of a candidate:
    /// a route the host reserved, an identity the owner already published, and a
    /// route another owner already published.
    /// </summary>
    private void AddBindingErrors(ComponentPageBinding binding, List<ComponentPageBindingError> errors)
    {
        if (_reservedHostRoutes.Contains(binding.RoutePath))
        {
            errors.Add(new ComponentPageBindingError(
                ComponentPageBindingErrorCode.ReservedHostRoute,
                binding.RoutePath,
                $"The route '{binding.RoutePath}' is reserved by the host and is not available to a plugin page."));
        }

        if (_published.Values.Any(page => IsSameBinding(page.Binding, binding)))
        {
            errors.Add(new ComponentPageBindingError(
                ComponentPageBindingErrorCode.DuplicateBinding,
                binding.SymbolicIdentity,
                $"The owner '{binding.OwnerKey}' already published a page for the symbolic identity " +
                $"'{binding.SymbolicIdentity}'."));
        }

        var conflicting = _published.Values.FirstOrDefault(page =>
            !IsSameBinding(page.Binding, binding)
            && string.Equals(page.Binding.RoutePath, binding.RoutePath, StringComparison.OrdinalIgnoreCase));

        if (conflicting is not null)
        {
            errors.Add(new ComponentPageBindingError(
                ComponentPageBindingErrorCode.ConflictingRoutePath,
                binding.RoutePath,
                $"The route '{binding.RoutePath}' is already published by the owner " +
                $"'{conflicting.Binding.OwnerKey}'."));
        }
    }

    /// <summary>
    /// Whether the type is a concrete, closed Blazor component a page can render.
    /// </summary>
    private static bool IsRenderableComponent(Type componentType) =>
        componentType.IsClass
        && !componentType.IsAbstract
        && !componentType.ContainsGenericParameters
        && typeof(IComponent).IsAssignableFrom(componentType);

    /// <summary>
    /// Adapts the page metadata a component really declares.
    /// </summary>
    private static PluginPageMetadata Metadata(Type componentType, PluginPageAttribute attribute) =>
        new()
        {
            Label = string.IsNullOrWhiteSpace(attribute.Label) ? componentType.Name : attribute.Label,
            Icon = attribute.Icon,
            Order = attribute.Order,
            ShowInNav = attribute.ShowInNav,
            Group = attribute.Group,
            SubGroup = attribute.SubGroup,
        };

    /// <summary>
    /// Whether two bindings describe the same page of the same owner.
    /// </summary>
    private static bool IsSameBinding(ComponentPageBinding left, ComponentPageBinding right) =>
        string.Equals(left.OwnerKey, right.OwnerKey, StringComparison.Ordinal)
        && string.Equals(left.SymbolicIdentity, right.SymbolicIdentity, StringComparison.Ordinal);

    /// <summary>A published page and the metadata it declared.</summary>
    private sealed record PublishedPage(ComponentPageBinding Binding, PluginPageMetadata Metadata);
}
