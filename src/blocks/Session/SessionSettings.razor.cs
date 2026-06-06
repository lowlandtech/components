namespace LowlandTech.Components;

public partial class SessionSettings : TinyComponentBase
{
    private enum SettingsSection
    {
        ApiKeys,
        Activity,
    }

    private SettingsSection _section = SettingsSection.ApiKeys;
    private IReadOnlyList<SessionClientKeyEntry> _keys = [];
    private SessionClientKeyIssued? _issued;
    private bool _isLoading = true;
    private bool _isBusy;
    private string? _error;

    [Inject] private IServiceProvider Services { get; set; } = default!;

    [Parameter] public ISessionClientSettingsService? ClientSettings { get; set; }

    [Parameter] public RenderFragment? ActivityContent { get; set; }

    private ISessionClientSettingsService EffectiveClientSettings =>
        ClientSettings
        ?? Services.GetService<ISessionClientSettingsService>()
        ?? LocalSessionClientSettingsService.Instance;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private string GetClass()
    {
        const string baseClass = "flex h-full min-h-0 w-full overflow-hidden bg-background text-foreground";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private string GetNavButtonClass(SettingsSection section)
    {
        var active = _section == section;
        return active
            ? "flex w-full items-center rounded-md bg-muted px-2 py-2 text-left text-sm font-semibold text-foreground"
            : "flex w-full items-center rounded-md px-2 py-2 text-left text-sm text-muted-foreground transition-colors hover:bg-muted/60 hover:text-foreground";
    }

    private async Task LoadAsync()
    {
        _isLoading = true;
        _error = null;

        try
        {
            _keys = await EffectiveClientSettings.ListAsync();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
            _keys = [];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task CreateKeyAsync()
    {
        _isBusy = true;
        _error = null;

        try
        {
            _issued = await EffectiveClientSettings.CreateAsync("Default API Key");
            _keys = await EffectiveClientSettings.ListAsync();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task RevokeKeyAsync(string id)
    {
        _isBusy = true;
        _error = null;

        try
        {
            await EffectiveClientSettings.RevokeAsync(id);
            _keys = await EffectiveClientSettings.ListAsync();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _isBusy = false;
        }
    }

    private void DismissIssuedSecret() => _issued = null;
}

internal sealed class LocalSessionClientSettingsService : ISessionClientSettingsService
{
    public static LocalSessionClientSettingsService Instance { get; } = new();

    private readonly List<SessionClientKeyEntry> _keys =
    [
        new("local-default", "Default API Key", "ltg_6d5cff...demo", "local user", DateTimeOffset.UtcNow.AddDays(-1)),
    ];

    public Task<IReadOnlyList<SessionClientKeyEntry>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SessionClientKeyEntry>>([.. _keys.Where(key => !key.IsRevoked)]);

    public Task<SessionClientKeyIssued> CreateAsync(string name, CancellationToken ct = default)
    {
        var id = Guid.NewGuid().ToString("N");
        var secret = $"ltg_local_{id[..8]}_{Guid.NewGuid():N}";
        var entry = new SessionClientKeyEntry(
            id,
            string.IsNullOrWhiteSpace(name) ? "Default API Key" : name,
            $"{secret[..12]}...{secret[^4..]}",
            "local user",
            DateTimeOffset.UtcNow);
        _keys.Add(entry);
        return Task.FromResult(new SessionClientKeyIssued(entry, secret));
    }

    public Task RevokeAsync(string id, CancellationToken ct = default)
    {
        var index = _keys.FindIndex(key => string.Equals(key.Id, id, StringComparison.Ordinal));
        if (index >= 0)
            _keys[index] = _keys[index] with { IsRevoked = true };

        return Task.CompletedTask;
    }
}
