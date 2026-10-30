namespace LowlandTech.Components;

public partial class SessionOutboxList : TinyComponentBase, IDisposable
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    [Parameter, EditorRequired] public ISessionOutbox Outbox { get; set; } = default!;

    protected override void OnInitialized()
    {
        Outbox.Changed += OnOutboxChanged;
    }

    private void OnOutboxChanged() => InvokeAsync(StateHasChanged);

    private string GetClass()
    {
        const string baseClass = "flex h-full min-h-0 w-full flex-col overflow-hidden bg-background text-foreground";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The entity or component is supplied by the host and a DynamicallyAccessedMembers requirement cannot flow from object.GetType(); the host preserves the reflected members, which the trimmed-host preservation probe checks.")]
    private static string SerializePayload(object payload)
    {
        try
        {
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return payload?.ToString() ?? string.Empty;
        }
    }

    private static string GetDisplayType(string type) => type switch
    {
        "ScheduleCreateEffect" => "Create",
        "ScheduleUpdateEffect" => "Update",
        "ScheduleDeleteEffect" => "Delete",
        "SendMailEffect" => "SendMail",
        _ => type
    };

    public void Dispose()
    {
        Outbox.Changed -= OnOutboxChanged;
    }
}
