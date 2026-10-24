using LowlandTech.Components.Core;

namespace LowlandTech.Components;

/// <summary>
/// The startup registration of the Components Blocks package.
/// </summary>
public static class ComponentsBlocksStartup
{
    /// <summary>The logical key the Blocks package owns.</summary>
    public const string LogicalKey = "lowlandtech.components.blocks";

    /// <summary>
    /// The registration a host adds to its trusted startup map. The Blocks
    /// package stays an ordinary dependency; an opt-in feature declares a
    /// logical dependency on this key.
    /// </summary>
    public static ComponentsStartupRegistration Registration { get; } = new(LogicalKey, []);
}
