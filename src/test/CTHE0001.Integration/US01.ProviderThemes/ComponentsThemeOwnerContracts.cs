namespace LowlandTech.Components.Tests.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// Accesses the Components-owned theme owner contracts by their documented names.
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
/// <item><c>ComponentThemeBinding(string OwnerKey, string ProviderId, string ThemeKey, IComponentTheme Theme, IReadOnlyList&lt;string&gt; Stylesheets)</c></item>
/// <item><c>ComponentThemeOwnerSnapshot(long Revision, IReadOnlyCollection&lt;string&gt; ActiveOwnerKeys)</c></item>
/// <item><c>ComponentThemeOwnerBridge</c> with
/// <c>ComponentThemeOwnerBridge(ComponentThemeService, ComponentThemeOwnerSnapshot)</c>,
/// <c>Publish(ComponentThemeBinding)</c>, <c>Deliver(ComponentThemeOwnerSnapshot)</c>,
/// <c>Revision</c>, <c>Snapshot</c>, <c>IsOwnerActive(string)</c>,
/// <c>Enumerate()</c>, <c>Select(string, string)</c> and
/// <c>StylesheetsFor(string)</c>.
/// </item>
/// <item><c>ComponentThemeBindingValidation</c> with <c>Binding</c>,
/// <c>IsValid</c> and <c>Errors</c>.</item>
/// <item><c>ComponentThemeBindingError</c> with <c>Code</c>, <c>Subject</c>
/// and <c>Message</c>.</item>
/// <item><c>ComponentThemeBindingErrorCode</c> with the members
/// <c>InactiveOwner</c>, <c>MissingThemeName</c>, <c>InvalidStyleFamily</c>,
/// <c>UndefinedMergeMode</c>, <c>InvalidStylesheetPath</c> and
/// <c>DuplicateBinding</c>.</item>
/// </list>
/// </para>
/// </summary>
public static class ComponentsThemeOwnerContracts
{
    /// <summary>The documented name of the theme binding contract.</summary>
    public const string BindingTypeName = "LowlandTech.Components.ComponentThemeBinding";

    /// <summary>The documented name of the owner snapshot contract.</summary>
    public const string SnapshotTypeName = "LowlandTech.Components.ComponentThemeOwnerSnapshot";

    /// <summary>The documented name of the theme owner bridge contract.</summary>
    public const string BridgeTypeName = "LowlandTech.Components.ComponentThemeOwnerBridge";

    /// <summary>The documented name of the binding validation contract.</summary>
    public const string ValidationTypeName = "LowlandTech.Components.ComponentThemeBindingValidation";

    /// <summary>The documented name of the binding error contract.</summary>
    public const string ErrorTypeName = "LowlandTech.Components.ComponentThemeBindingError";

    /// <summary>The documented name of the binding error code contract.</summary>
    public const string ErrorCodeTypeName = "LowlandTech.Components.ComponentThemeBindingErrorCode";

    /// <summary>
    /// The assemblies the planned contracts may live in: the Blocks package
    /// that owns the bridge and the base packages that own the theme contracts
    /// the bridge projects onto.
    /// </summary>
    private static readonly Assembly[] ContractAssemblies =
    [
        typeof(SessionPluginRegistry).Assembly,
        typeof(ComponentThemeService).Assembly,
        typeof(ComponentsStartupMap).Assembly,
    ];

    /// <summary>
    /// Creates the binding of an owner's theme to a provider boundary.
    /// </summary>
    public static object CreateBinding(
        string ownerKey,
        string providerId,
        string themeKey,
        IComponentTheme theme,
        IReadOnlyList<string>? stylesheets = null)
    {
        var bindingType = RequireType(BindingTypeName);

        return Activator.CreateInstance(
                   bindingType,
                   ownerKey,
                   providerId,
                   themeKey,
                   theme,
                   stylesheets ?? [])
               ?? throw new InvalidOperationException(
                   $"The theme binding '{bindingType.FullName}' could not be created.");
    }

    /// <summary>
    /// Creates the snapshot that carries the host authority at one revision.
    /// </summary>
    public static object CreateSnapshot(long revision, IEnumerable<string> activeOwnerKeys)
    {
        var snapshotType = RequireType(SnapshotTypeName);

        return Activator.CreateInstance(snapshotType, revision, activeOwnerKeys.ToArray())
               ?? throw new InvalidOperationException(
                   $"The theme owner snapshot '{snapshotType.FullName}' could not be created.");
    }

    /// <summary>
    /// Creates the bridge that projects the owned themes onto the resolver of
    /// the host.
    /// </summary>
    public static object CreateBridge(ComponentThemeService service, object snapshot)
    {
        var bridgeType = RequireType(BridgeTypeName);

        return Activator.CreateInstance(bridgeType, service, snapshot)
               ?? throw new InvalidOperationException(
                   $"The theme owner bridge '{bridgeType.FullName}' could not be created.");
    }

    /// <summary>Publishes a binding and returns its validation.</summary>
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

    /// <summary>The bindings the bridge publishes for the active owners.</summary>
    public static IReadOnlyList<object> Enumerate(object bridge)
    {
        var bindings = (System.Collections.IEnumerable)Invoke(bridge, "Enumerate")!;
        var result = new List<object>();

        foreach (var binding in bindings)
        {
            result.Add(binding!);
        }

        return result;
    }

    /// <summary>
    /// Selects the theme of a provider through the bridge, so an inactive
    /// owner can never be selected.
    /// </summary>
    public static bool Select(object bridge, string providerId, string themeKey) =>
        (bool)Invoke(bridge, "Select", providerId, themeKey)!;

    /// <summary>The stylesheet selection of a provider.</summary>
    public static IReadOnlyList<string> StylesheetsFor(object bridge, string providerId)
    {
        var stylesheets = (System.Collections.IEnumerable)Invoke(bridge, "StylesheetsFor", providerId)!;
        var result = new List<string>();

        foreach (var stylesheet in stylesheets)
        {
            result.Add((string)stylesheet!);
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

    private static Type RequireType(string fullName)
    {
        foreach (var assembly in ContractAssemblies)
        {
            var type = assembly.GetType(fullName, throwOnError: false);

            if (type is not null)
            {
                return type;
            }
        }

        throw new MissingMemberException(
            $"The Components theme owner contract '{fullName}' is not implemented in " +
            $"'{string.Join("', '", ContractAssemblies.Select(assembly => assembly.GetName().Name))}'. " +
            "The scope cannot be accepted until it exists.");
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
                $"The Components theme owner contract '{target.GetType().FullName}' does not offer " +
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
                $"The Components theme owner contract '{target.GetType().FullName}' does not expose " +
                $"'{propertyName}'.");
        }

        return property.GetValue(target);
    }
}
