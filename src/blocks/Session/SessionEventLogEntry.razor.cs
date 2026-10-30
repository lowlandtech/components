namespace LowlandTech.Components;

public partial class SessionEventLogEntry
{
    [Parameter, EditorRequired] public SessionEventLogItem Entry { get; set; } = null!;

    private string GetDeltaJson() =>
        JsonSerializer.Serialize(Entry.Deltas.ToArray(), BlocksIndentedJsonContext.Default.SessionStateDeltaItemArray);
}
