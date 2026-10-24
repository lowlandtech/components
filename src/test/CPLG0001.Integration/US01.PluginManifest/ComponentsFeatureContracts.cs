using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// Accesses the Components-owned feature activation contracts by their
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
/// <c>LowlandTech.Components.Core</c>:
/// <list type="bullet">
/// <item><c>ComponentsStartupBinding(string LogicalKey, string SymbolicIdentity, Type ComponentType)</c></item>
/// <item><c>ComponentsStartupMap(IEnumerable&lt;ComponentsStartupBinding&gt;, IEnumerable&lt;string&gt;)</c> with
/// <c>TryResolveComponent(string, out Type)</c> and <c>ContainsLogicalKey(string)</c></item>
/// <item><c>ComponentsFeatureManifest(string Key, int SchemaVersion, string Target,
/// IReadOnlyList&lt;string&gt; RequiredCapabilities, IReadOnlyList&lt;string&gt; LogicalDependencies,
/// IReadOnlyList&lt;string&gt; ComponentIdentities)</c></item>
/// <item><c>ComponentsFeatureActivator(ComponentsStartupMap)</c> with
/// <c>Activate(ComponentsFeatureManifest, IReadOnlyCollection&lt;string&gt;)</c></item>
/// <item><c>ComponentsFeatureActivationResult(bool IsActivated, IReadOnlyList&lt;Type&gt; ResolvedComponents,
/// IReadOnlyList&lt;ComponentsFeatureActivationError&gt; Errors)</c></item>
/// <item><c>ComponentsFeatureActivationError(ComponentsFeatureActivationErrorCode Code, string Subject, string Message)</c></item>
/// </list>
/// </para>
/// </summary>
public static class ComponentsFeatureContracts
{
    private const string ContractNamespace = "LowlandTech.Components.Core";

    /// <summary>
    /// Activates a feature manifest against a trusted startup map and returns
    /// the outcome the production activator produced.
    /// </summary>
    /// <param name="manifestJson">The feature manifest, as published.</param>
    /// <param name="bindings">The trusted startup bindings of the host.</param>
    /// <param name="logicalKeys">
    /// The logical keys the host registered at startup, including the keys the
    /// base libraries own.
    /// </param>
    /// <param name="deployedFeatureKeys">
    /// The logical keys of the features the deployed host actually carries.
    /// </param>
    public static ComponentsFeatureActivationOutcome Activate(
        string manifestJson,
        IReadOnlyList<(string LogicalKey, string SymbolicIdentity, Type ComponentType)> bindings,
        IReadOnlyCollection<string> logicalKeys,
        IReadOnlyCollection<string> deployedFeatureKeys)
    {
        var assembly = typeof(LowlandTech.Core.Plugins.PluginPageAttribute).Assembly;

        var bindingType = RequireType(assembly, "ComponentsStartupBinding");
        var mapType = RequireType(assembly, "ComponentsStartupMap");
        var manifestType = RequireType(assembly, "ComponentsFeatureManifest");
        var activatorType = RequireType(assembly, "ComponentsFeatureActivator");
        var resultType = RequireType(assembly, "ComponentsFeatureActivationResult");
        var errorType = RequireType(assembly, "ComponentsFeatureActivationError");

        var startupBindings = bindings
            .Select(binding => Activator.CreateInstance(
                bindingType, binding.LogicalKey, binding.SymbolicIdentity, binding.ComponentType))
            .ToArray();

        var startupMap = Activator.CreateInstance(mapType, startupBindings, logicalKeys.ToArray())
                         ?? throw new InvalidOperationException(
                             $"The Components startup map '{mapType.FullName}' could not be created.");

        var manifest = Activator.CreateInstance(
            manifestType,
            ReadKey(manifestJson),
            ReadSchemaVersion(manifestJson),
            ReadTarget(manifestJson),
            ReadRequiredCapabilities(manifestJson),
            ReadLogicalDependencies(manifestJson),
            ReadComponentIdentities(manifestJson))
            ?? throw new InvalidOperationException(
                $"The Components feature manifest '{manifestType.FullName}' could not be created.");

        var activator = Activator.CreateInstance(activatorType, startupMap)
                       ?? throw new InvalidOperationException(
                           $"The Components feature activator '{activatorType.FullName}' could not be created.");

        var activate = activatorType.GetMethods()
            .FirstOrDefault(method => method.Name == "Activate" && method.GetParameters().Length == 2)
            ?? throw new MissingMethodException(
                $"The Components feature activator '{activatorType.FullName}' does not offer " +
                "Activate(ComponentsFeatureManifest, deployed feature keys).");

        var arguments = new[]
        {
            manifest,
            AdaptKeys(activate.GetParameters()[1].ParameterType, deployedFeatureKeys),
        };

        var result = activate.Invoke(activator, arguments)
                     ?? throw new InvalidOperationException(
                         $"The Components feature activator '{activatorType.FullName}' returned no outcome.");

        return ReadOutcome(result, resultType, errorType);
    }

    /// <summary>
    /// Resolves a Components contract by its documented name and fails with the
    /// exact missing contract when it is not implemented yet.
    /// </summary>
    private static Type RequireType(Assembly assembly, string name)
    {
        var fullName = $"{ContractNamespace}.{name}";
        var type = assembly.GetType(fullName, throwOnError: false);

        return type ?? throw new MissingMemberException(
            $"The Components feature activation contract '{fullName}' is not implemented in " +
            $"'{assembly.GetName().Name}'. The scope cannot be accepted until it exists.");
    }

    private static ComponentsFeatureActivationOutcome ReadOutcome(
        object result,
        Type resultType,
        Type errorType)
    {
        var isActivated = (bool)resultType.GetProperty("IsActivated")!.GetValue(result)!;

        var resolvedComponents = ReadCollection(resultType, result, "ResolvedComponents")
            .Cast<Type>()
            .ToList();

        var errors = ReadCollection(resultType, result, "Errors")
            .Cast<object>()
            .Select(error => new ComponentsFeatureActivationErrorView(
                errorType.GetProperty("Code")!.GetValue(error)!.ToString() ?? string.Empty,
                (string?)errorType.GetProperty("Subject")!.GetValue(error) ?? string.Empty,
                (string?)errorType.GetProperty("Message")!.GetValue(error) ?? string.Empty))
            .ToList();

        return new ComponentsFeatureActivationOutcome(isActivated, resolvedComponents, errors);
    }

    private static IEnumerable ReadCollection(Type resultType, object result, string propertyName)
    {
        var value = resultType.GetProperty(propertyName)?.GetValue(result);

        return value as IEnumerable
               ?? throw new InvalidOperationException(
                   $"The activation outcome '{resultType.FullName}.{propertyName}' is not a collection.");
    }

    private static object AdaptKeys(Type parameterType, IReadOnlyCollection<string> keys)
    {
        if (parameterType.IsInstanceOfType(keys))
        {
            return keys;
        }

        if (parameterType == typeof(string[]))
        {
            return keys.ToArray();
        }

        if (parameterType == typeof(List<string>))
        {
            return keys.ToList();
        }

        if (parameterType == typeof(HashSet<string>) || parameterType == typeof(ISet<string>))
        {
            return new HashSet<string>(keys, StringComparer.Ordinal);
        }

        return keys;
    }

    private static string ReadKey(string manifestJson) =>
        ReadManifest(manifestJson).GetProperty("key").GetString() ?? string.Empty;

    private static int ReadSchemaVersion(string manifestJson) =>
        ReadManifest(manifestJson).GetProperty("schemaVersion").GetInt32();

    private static string ReadTarget(string manifestJson) =>
        ReadManifest(manifestJson).GetProperty("target").GetString() ?? string.Empty;

    private static IReadOnlyList<string> ReadRequiredCapabilities(string manifestJson)
    {
        if (!ReadManifest(manifestJson).TryGetProperty("installationUnit", out var installationUnit))
        {
            return [];
        }

        if (!installationUnit.TryGetProperty("requiredCapabilities", out var capabilities))
        {
            return [];
        }

        return capabilities.EnumerateArray()
            .Select(capability => capability.GetString() ?? string.Empty)
            .ToList();
    }

    private static IReadOnlyList<string> ReadLogicalDependencies(string manifestJson)
    {
        if (!ReadManifest(manifestJson).TryGetProperty("dependencies", out var dependencies))
        {
            return [];
        }

        return dependencies.EnumerateArray()
            .Select(dependency => dependency.GetProperty("key").GetString() ?? string.Empty)
            .ToList();
    }

    private static IReadOnlyList<string> ReadComponentIdentities(string manifestJson)
    {
        if (!ReadManifest(manifestJson).TryGetProperty("contributions", out var contributions))
        {
            return [];
        }

        if (!contributions.TryGetProperty("components", out var components))
        {
            return [];
        }

        return components.EnumerateArray()
            .Select(component => component.GetString() ?? string.Empty)
            .ToList();
    }

    private static JsonElement ReadManifest(string manifestJson)
    {
        using var document = JsonDocument.Parse(manifestJson);

        return document.RootElement.Clone();
    }
}
