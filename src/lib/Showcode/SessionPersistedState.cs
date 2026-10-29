namespace LowlandTech.Components;

/// <summary>
/// The persisted state of a <see cref="StoryCard"/> across a session reload.
/// <para>
/// The type lives outside the component so the generated JSON contract can name
/// it; the component reads and writes the same payload as before.
/// </para>
/// </summary>
internal sealed class SessionPersistedState
{
    public string? Scenario { get; set; }

    public string? CodeTab { get; set; }

    public bool DesignMode { get; set; }

    public string? TypeName { get; set; }

    public string? InstanceId { get; set; }

    public string? DesignKey { get; set; }

    public Dictionary<string, string>? Values { get; set; }
}
