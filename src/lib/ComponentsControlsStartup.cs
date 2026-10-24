using LowlandTech.Components.Core;

namespace LowlandTech.Components;

/// <summary>
/// The startup registration of the Components controls package.
/// </summary>
public static class ComponentsControlsStartup
{
    /// <summary>The logical key the controls package owns.</summary>
    public const string LogicalKey = "lowlandtech.components.controls";

    /// <summary>
    /// The registration a host adds to its trusted startup map. The controls
    /// package stays an ordinary dependency and contributes no component
    /// identity of its own.
    /// </summary>
    public static ComponentsStartupRegistration Registration { get; } = new(LogicalKey, []);
}
