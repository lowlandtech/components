namespace LowlandTech.Components.Json;

/// <summary>
/// The source-generated JSON contract of the shapes the block compositions
/// persist with the default serializer options.
/// <para>
/// A trimmed publish cannot resolve a shape by reflection, so every payload that
/// these compositions write or read is named here once and the call sites
/// serialize through the generated
/// <see cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo"/>. The
/// persisted shape is unchanged: the contracts use the default options the
/// previous reflection-based serializer used.
/// </para>
/// </summary>
[JsonSerializable(typeof(List<SessionCommandSnapshot>))]
[JsonSerializable(typeof(SessionCompletionSelection))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(SessionBlockStateSnapshot))]
internal sealed partial class BlocksJsonContext : JsonSerializerContext
{
}

/// <summary>
/// The source-generated JSON contract of the shapes that are persisted with
/// indented output, such as an event-log entry that is read by a person.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SessionStateDeltaItem[]))]
[JsonSerializable(typeof(SessionBlockStateSnapshot))]
[JsonSerializable(typeof(WorkAppState))]
internal sealed partial class BlocksIndentedJsonContext : JsonSerializerContext
{
}
