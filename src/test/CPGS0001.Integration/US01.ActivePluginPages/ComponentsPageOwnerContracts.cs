namespace LowlandTech.Components.Tests.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// Accesses the Components-owned plugin page contracts by their documented
/// names.
/// </summary>
/// <para>
/// The scope is refined before it is executed, so the acceptance inventory has
/// to compile while the production contracts it accepts do not exist yet. The
/// contracts are therefore reached by name instead of by a compile-time
/// reference. Every lookup fails with the exact missing contract name, so an
/// unimplemented contract can never turn into a silent pass: the Fact fails
/// with the contract it still owes.
/// </para>
/// <para>
/// The contracts this accessor expects, owned by the optional page feature
/// package of Components:
/// <list type="bullet">
/// <item><c>ComponentPageBinding(string OwnerKey, string SymbolicIdentity, string RoutePath)</c></item>
/// <item><c>ComponentPageOwnerSnapshot(long Revision, IReadOnlyCollection&lt;string&gt; ActiveOwnerKeys)</c></item>
/// <item><c>ComponentPageOwnerBridge</c> with
/// <c>ComponentPageOwnerBridge(ComponentsStartupMap, ComponentPageOwnerSnapshot, IEnumerable&lt;string&gt;)</c>,
/// <c>Publish(ComponentPageBinding)</c>, <c>Deliver(ComponentPageOwnerSnapshot)</c>,
/// <c>Revision</c>, <c>Snapshot</c>, <c>IsOwnerActive(string)</c>,
/// <c>Enumerate()</c>, <c>Resolve(string)</c>, <c>Navigation()</c> and
/// <c>PageCatalog()</c>.
/// </item>
/// <item><c>ComponentPageBindingValidation</c> with <c>Binding</c>,
/// <c>IsValid</c> and <c>Errors</c>.</item>
/// <item><c>ComponentPageBindingError</c> with <c>Code</c>, <c>Subject</c>
/// and <c>Message</c>.</item>
/// <item><c>ComponentPageBindingErrorCode</c> with the members
/// <c>DuplicateBinding</c>, <c>UnresolvedComponent</c>,
/// <c>WrongComponentType</c>, <c>MissingPageMetadata</c>,
/// <c>ConflictingRoutePath</c> and <c>ReservedHostRoute</c>.</item>
/// <item><c>ComponentPageNavigationEntry(string RoutePath, string Label, string Icon, int Order, string? Group, string? SubGroup)</c></item>
/// </list>
/// </para>
/// <para>
/// The page adapter may be hosted by the optional feature package
/// <c>LowlandTech.Components.Features.Pages</c> or by the base Blocks
/// package. The accessor accepts both documented homes, so the execution
/// phase may place the contracts without changing this inventory.
/// </para>
/// </summary>
public static class ComponentsPageOwnerContracts
{
    /// <summary>The documented name of the page binding contract.</summary>
    public const string BindingTypeName = "ComponentPageBinding";

    /// <summary>The documented name of the owner snapshot contract.</summary>
    public const string SnapshotTypeName = "ComponentPageOwnerSnapshot";

    /// <summary>The documented name of the page owner bridge contract.</summary>
    public const string BridgeTypeName = "ComponentPageOwnerBridge";

    /// <summary>The documented name of the binding validation contract.</summary>
    public const string ValidationTypeName = "ComponentPageBindingValidation";

    /// <summary>The documented name of the binding error contract.</summary>
    public const string ErrorTypeName = "ComponentPageBindingError";

    /// <summary>The documented name of the binding error code contract.</summary>
    public const string ErrorCodeTypeName = "ComponentPageBindingErrorCode";

    /// <summary>The documented name of the navigation entry contract.</summary>
    public const string NavigationEntryTypeName = "ComponentPageNavigationEntry";

    /// <summary>The optional feature package that may host the contracts.</summary>
    public const string FeatureAssemblyName = "LowlandTech.Components.Features.Pages";

    /// <summary>The namespaces the contracts may live in, most specific first.</summary>
    private static readonly string[] ContractNamespaces =
    [
        "LowlandTech.Components.Features.Pages",
        "LowlandTech.Components",
    ];

    /// <summary>
    /// The assemblies the planned contracts may live in: the base packages
    /// that own the startup map and the attribute, and the optional feature
    /// package that owns the adapter.
    /// </summary>
    private static IEnumerable<Assembly> ContractAssemblies()
    {
        yield return typeof(ComponentsStartupMap).Assembly;
        yield return typeof(PluginPageAttribute).Assembly;
        yield return typeof(ComponentsStartupBinding).Assembly;

        Assembly? feature = null;

        try
        {
            feature = Assembly.Load(FeatureAssemblyName);
        }
        catch (Exception exception) when (exception is System.IO.FileNotFoundException
            or System.IO.FileLoadException
            or BadImageFormatException)
        {
            // The optional feature package does not exist yet. The contract
            // lookup reports the missing contract by name instead.
        }

        if (feature is not null)
        {
            yield return feature;
        }
    }

    /// <summary>
    /// Creates the binding an owner contributes, before it is validated and
    /// published.
    /// </summary>
    public static object CreateBinding(string ownerKey, string symbolicIdentity, string routePath)
    {
        var bindingType = RequireType(BindingTypeName);

        return Activator.CreateInstance(bindingType, ownerKey, symbolicIdentity, routePath)
               ?? throw new InvalidOperationException(
                   $"The page binding '{bindingType.FullName}' could not be created.");
    }

    /// <summary>
    /// Creates the snapshot that carries the host authority at one revision.
    /// </summary>
    public static object CreateSnapshot(long revision, IEnumerable<string> activeOwnerKeys)
    {
        var snapshotType = RequireType(SnapshotTypeName);

        return Activator.CreateInstance(snapshotType, revision, activeOwnerKeys.ToArray())
               ?? throw new InvalidOperationException(
                   $"The page owner snapshot '{snapshotType.FullName}' could not be created.");
    }

    /// <summary>
    /// Creates the bridge that projects the owned pages onto the Plugins route
    /// and page primitives.
    /// </summary>
    public static object CreateBridge(
        ComponentsStartupMap startupMap,
        object snapshot,
        IEnumerable<string> reservedHostRoutes)
    {
        var bridgeType = RequireType(BridgeTypeName);

        return Activator.CreateInstance(bridgeType, startupMap, snapshot, reservedHostRoutes.ToArray())
               ?? throw new InvalidOperationException(
                   $"The page owner bridge '{bridgeType.FullName}' could not be created.");
    }

    /// <summary>Validates a binding and publishes it when it is valid.</summary>
    public static object Publish(object bridge, object binding) =>
        Invoke(bridge, "Publish", binding)!;

    /// <summary>Delivers a new revision of the host authority to the bridge.</summary>
    public static void Deliver(object bridge, object snapshot) =>
        Invoke(bridge, "Deliver", snapshot);

    /// <summary>The revision the bridge currently projects.</summary>
    public static long GetRevision(object bridge) =>
        (long)ReadProperty(bridge, "Revision")!;

    /// <summary>Whether the owner is active in the revision the bridge projects.</summary>
    public static bool IsOwnerActive(object bridge, string ownerKey) =>
        (bool)Invoke(bridge, "IsOwnerActive", ownerKey)!;

    /// <summary>
    /// The bindings the bridge publishes for the active owners, as the owner
    /// key, the symbolic identity and the route path of every binding.
    /// </summary>
    public static IReadOnlyList<(string OwnerKey, string SymbolicIdentity, string RoutePath)> Enumerate(object bridge)
    {
        var bindings = (System.Collections.IEnumerable)Invoke(bridge, "Enumerate")!;
        var result = new List<(string, string, string)>();

        foreach (var binding in bindings)
        {
            result.Add((
                (string)ReadProperty(binding!, "OwnerKey")!,
                (string)ReadProperty(binding!, "SymbolicIdentity")!,
                (string)ReadProperty(binding!, "RoutePath")!));
        }

        return result;
    }

    /// <summary>
    /// The binding that owns a route path, or <c>null</c> when no active owner
    /// owns it.
    /// </summary>
    public static object? Resolve(object bridge, string routePath) =>
        Invoke(bridge, "Resolve", routePath);

    /// <summary>
    /// The navigation projection of the active owners, as the route path, the
    /// label, the icon, the order, the group and the subgroup of every entry.
    /// </summary>
    public static IReadOnlyList<(string RoutePath, string Label, string Icon, int Order, string? Group, string? SubGroup)> Navigation(object bridge)
    {
        var entries = (System.Collections.IEnumerable)Invoke(bridge, "Navigation")!;
        var result = new List<(string, string, string, int, string?, string?)>();

        foreach (var entry in entries)
        {
            result.Add((
                (string)ReadProperty(entry!, "RoutePath")!,
                (string)ReadProperty(entry!, "Label")!,
                (string)ReadProperty(entry!, "Icon")!,
                (int)ReadProperty(entry!, "Order")!,
                (string?)ReadProperty(entry!, "Group"),
                (string?)ReadProperty(entry!, "SubGroup")));
        }

        return result;
    }

    /// <summary>
    /// The Plugins page catalog of the active owners, as the owner key, the
    /// component identity and the adapted metadata of every page.
    /// </summary>
    public static IReadOnlyList<(string PluginKey, string Component, string Label, string Icon, int Order, bool ShowInNav, string? Group, string? SubGroup)> PageEntries(object bridge)
    {
        var catalog = Invoke(bridge, "PageCatalog")!;
        var entries = (System.Collections.IEnumerable)ReadProperty(catalog, "Entries")!;
        var result = new List<(string, string, string, string, int, bool, string?, string?)>();

        foreach (var entry in entries)
        {
            if (entry is not KeyValuePair<(string PluginKey, string Component), PluginPageMetadata> pair)
            {
                continue;
            }

            result.Add((
                pair.Key.PluginKey,
                pair.Key.Component,
                pair.Value.Label,
                pair.Value.Icon,
                pair.Value.Order,
                pair.Value.ShowInNav,
                pair.Value.Group,
                pair.Value.SubGroup));
        }

        return result;
    }

    /// <summary>Whether a validation accepted its binding.</summary>
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

    private static Type RequireType(string simpleName)
    {
        var assemblies = ContractAssemblies().ToArray();

        foreach (var namespaceName in ContractNamespaces)
        {
            var fullName = $"{namespaceName}.{simpleName}";

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(fullName, throwOnError: false);

                if (type is not null)
                {
                    return type;
                }
            }
        }

        throw new MissingMemberException(
            $"The Components plugin page contract '{simpleName}' is not implemented in " +
            $"'{string.Join("', '", ContractNamespaces)}'. The scope cannot be accepted until it exists.");
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
                $"The Components plugin page contract '{target.GetType().FullName}' does not offer " +
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
                $"The Components plugin page contract '{target.GetType().FullName}' does not expose " +
                $"'{propertyName}'.");
        }

        return property.GetValue(target);
    }
}
