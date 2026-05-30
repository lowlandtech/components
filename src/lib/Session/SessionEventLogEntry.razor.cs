namespace LowlandTech.Components.Session;

public partial class SessionEventLogEntry
{
    [Parameter, EditorRequired] public SessionEventLogItem Entry { get; set; } = null!;

    private static readonly JsonSerializerOptions DeltaJsonOptions = new() { WriteIndented = true };

    private string GetDeltaJson() => JsonSerializer.Serialize(Entry.Deltas, DeltaJsonOptions);
}
