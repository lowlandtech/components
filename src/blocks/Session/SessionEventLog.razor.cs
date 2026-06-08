namespace LowlandTech.Components;

public partial class SessionEventLog : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    [Parameter] public IReadOnlyList<SessionEventLogItem> Entries { get; set; } = [];
    [Parameter] public string? InitialStateJson { get; set; }
    [Parameter] public string? CurrentStateJson { get; set; }
    [Parameter] public EventCallback OnBackRequested { get; set; }
    [Parameter] public EventCallback OnClearRequested { get; set; }

    private string GetClass()
    {
        var baseClass = "flex h-full min-h-0 w-full flex-col overflow-hidden";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
