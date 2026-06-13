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

    public void Dispose()
    {
        Outbox.Changed -= OnOutboxChanged;
    }
}
