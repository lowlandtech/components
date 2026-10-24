using LowlandTech.Components.Blocks.Session;
using LowlandTech.Components.Core;

namespace LowlandTech.Components.Features.Session;

/// <summary>
/// The opt-in session feature of Components.
///
/// <para>
/// Installing this package makes the session surface selectable as a Blazor
/// plugin. The package itself carries no component: it publishes the manifest
/// that declares the feature and the trusted startup binding that resolves the
/// contributed component. The base Components packages stay ordinary NuGet
/// dependencies.
/// </para>
/// </summary>
public static class ComponentsSessionFeature
{
    /// <summary>The stable logical plugin key of the feature.</summary>
    public const string LogicalKey = "lowlandtech.components.session";

    /// <summary>The logical key of the base Blocks library the feature builds on.</summary>
    public const string LogicalDependencyKey = "lowlandtech.components.blocks";

    /// <summary>The symbolic identity of the contributed component.</summary>
    public const string SessionHostComponentIdentity =
        "LowlandTech.Components.SessionPluginHost";

    /// <summary>
    /// The registration a host adds to its trusted startup map. The binding
    /// names the same symbolic identity the published manifest declares, so the
    /// feature resolves exactly what it contributes.
    /// </summary>
    public static ComponentsStartupRegistration Registration { get; } = new(
        LogicalKey,
        [new ComponentsStartupBinding(LogicalKey, SessionHostComponentIdentity, typeof(SessionPluginHost))]);
}
