namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryAppClonePanel
{
    [CascadingParameter] public RepositoryAppContext? Context { get; set; }

    [Parameter] public string ActiveProtocol { get; set; } = "https";

    [Parameter] public EventCallback<string> OnProtocolChanged { get; set; }

    private string _activeProtocol = "https";
    private bool _copied;

    private readonly IReadOnlyList<ProtocolDescriptor> _protocols = new List<ProtocolDescriptor>
    {
        new("https", "HTTPS", "lock"),
        new("ssh",   "SSH",   "terminal"),
        new("tea",   "Tea",   "package")
    };

    private sealed record ProtocolDescriptor(string Key, string Label, string Icon);

    protected override void OnInitialized()
    {
        if (!string.IsNullOrWhiteSpace(ActiveProtocol))
        {
            _activeProtocol = ActiveProtocol;
        }
    }

    private void SelectProtocol(string key)
    {
        _activeProtocol = key;
        _ = OnProtocolChanged.InvokeAsync(key);
    }

    private string GetCommandText() => Context?.State.CloneInfo switch
    {
        null => string.Empty,
        var info when _activeProtocol == "ssh" => info.Ssh,
        var info when _activeProtocol == "tea" => info.Tea,
        var info => info.Https
    };

    private string GetHintText() => _activeProtocol switch
    {
        "ssh" => "Use SSH keys for passwordless cloning. Configure your key in account settings.",
        "tea" => "tea is the Gitea command line tool. Run 'tea login add' first.",
        _ => "Clone over HTTPS. Provide your username and password (or a personal access token) when prompted."
    };

    private async Task CopyCommandAsync()
    {
        var text = GetCommandText();
        if (string.IsNullOrEmpty(text)) return;
        try
        {
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", text);
            _copied = true;
            StateHasChanged();
            await Task.Delay(1500);
            _copied = false;
            StateHasChanged();
        }
        catch
        {
            // clipboard may be unavailable in some hosting contexts; silently no-op
        }
    }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex w-full flex-col border-b border-border bg-muted/30"
        : $"flex w-full flex-col border-b border-border bg-muted/30 {Class}";
}
