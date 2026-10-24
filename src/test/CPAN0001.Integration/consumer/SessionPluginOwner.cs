namespace LowlandTech.Components.SessionConsumer;

/// <summary>
/// The plugin owner of the consumer and the panel it contributes.
/// </summary>
/// <param name="Key">The owner key the host authority names.</param>
/// <param name="Panel">The panel the owner contributes.</param>
public sealed record SessionPluginOwner(string Key, ISessionPlugin Panel);
