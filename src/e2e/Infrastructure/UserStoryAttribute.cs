using Xunit.v3;

namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// Marks the user story a browser scenario belongs to, mirroring the unit
/// acceptance project so both carry the same traceability traits.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class UserStoryAttribute(string id, string description) : Attribute, ITraitAttribute
{
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
        => [new(Spec.US, id), new("Story", description)];
}
