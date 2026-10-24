namespace LowlandTech.Components.Core;

/// <summary>
/// The startup registration of the Components Core package.
/// </summary>
public static class ComponentsCoreStartup
{
    /// <summary>The logical key the Core package owns.</summary>
    public const string LogicalKey = "lowlandtech.components.core";

    /// <summary>
    /// The registration a host adds to its trusted startup map. The Core
    /// package owns shared contracts and contributes no component identity of
    /// its own.
    /// </summary>
    public static ComponentsStartupRegistration Registration { get; } = new(LogicalKey, []);
}
