namespace LowlandTech.Components.Json;

/// <summary>
/// The source-generated JSON contract of the shapes this package persists.
/// <para>
/// A trimmed publish cannot resolve a shape by reflection, so every payload that
/// this package writes or reads is named here once and the call sites serialize
/// through the generated <see cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo"/>.
/// The persisted shape of a payload stays the shape the previous reflection-based
/// serializer wrote: the member names of a payload are declared explicitly and the
/// options are the defaults, so a payload written before this contract is read the
/// same way after it.
/// </para>
/// </summary>
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(ContainerGridLayout))]
[JsonSerializable(typeof(ContainerGridOptionsPayload))]
[JsonSerializable(typeof(SessionPromptPayload))]
[JsonSerializable(typeof(ShowcodeNavigationSnapshot))]
[JsonSerializable(typeof(SessionPersistedState))]
internal sealed partial class ComponentsJsonContext : JsonSerializerContext
{
}

/// <summary>
/// The grid-level options that <c>containerGrid.js</c> reads from
/// <c>data-options</c>. The member names are the camelCase names the browser
/// script reads; they are declared here instead of a naming policy so the
/// generated contract keeps the payload of the previous serializer.
/// </summary>
internal sealed record ContainerGridOptionsPayload(
    [property: JsonPropertyName("floatMode")] bool FloatMode,
    [property: JsonPropertyName("compactOnDrop")] bool CompactOnDrop,
    [property: JsonPropertyName("resolveCollisions")] bool ResolveCollisions);

/// <summary>The prompt payload that a session publishes to the agent.</summary>
internal sealed record SessionPromptPayload(
    [property: JsonPropertyName("context")] SessionPromptEntry[] Context);

/// <summary>One entry of the session prompt payload.</summary>
internal sealed record SessionPromptEntry(
    [property: JsonPropertyName("blockId")] Guid BlockId,
    [property: JsonPropertyName("blockType")] string BlockType,
    [property: JsonPropertyName("entity")] JsonElement Entity);
