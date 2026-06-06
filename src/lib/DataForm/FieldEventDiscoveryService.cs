namespace LowlandTech.Components;

/// <summary>
/// Discovers form field event intents from registered registries.
/// Part of the DRD (Descriptor, Registry, Discovery) pattern.
/// </summary>
public class FieldEventDiscoveryService
{
    private readonly IReadOnlyList<IFormFieldRegistry> _registries;

    public FieldEventDiscoveryService(IEnumerable<IFormFieldRegistry> registries)
    {
        _registries = registries.OrderByDescending(r => r.Priority).ToList();
    }

    /// <summary>
    /// Resolves the intent key for a field event (e.g. "email_Changing_intent").
    /// </summary>
    public string? GetIntentKey(string fieldName, string eventName)
    {
        foreach (var registry in _registries)
        {
            var behavior = registry.GetBehavior(fieldName);
            if (behavior?.EventMap.TryGetValue(eventName, out var intent) == true)
                return intent;
        }
        return $"{fieldName}_{eventName}_intent";
    }

    /// <summary>
    /// Builds a full FormFieldIntent for the given field and event.
    /// </summary>
    public FormFieldIntent BuildIntent(string fieldName, string eventName, object? payload = null)
    {
        var intentKey = GetIntentKey(fieldName, eventName) ?? $"{fieldName}_{eventName}_intent";
        return new FormFieldIntent(fieldName, eventName, intentKey, payload);
    }
}
