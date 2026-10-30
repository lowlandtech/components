namespace LowlandTech.Components.Blocks.Session;

public interface IAgentSelectionService
{
    IReadOnlyList<SessionProviderOption> Providers { get; }
    SessionCompletionSelection CurrentSelection { get; }
    event Action<SessionCompletionSelection>? SelectionChanged;
    Task HydrateAsync(string? persistKey = null);
    Task SelectProviderAsync(string providerKey);
    Task SelectModelAsync(string providerKey, string modelKey);
    Task SetApiKeyAsync(string? apiKey);
}

public sealed class AgentSelectionService : IAgentSelectionService
{
    private const string DefaultPersistKey = "session-command-bar:completion";

    private static readonly IReadOnlyList<SessionProviderOption> DefaultProviders =
    [
        new(
            "kimi",
            "Kimi",
            [
                new("kimi-k2.5", "Kimi K2.5"),
                new("kimi-k2-thinking", "Kimi K2 Thinking")
            ],
            RequiresApiKey: true),
        new(
            "ollama",
            "Ollama",
            [
                new("google/gemma-4-26b-a4b", "Gemma 4 26B"),
                new("qwen3-coder", "Qwen3 Coder")
            ])
    ];

    private readonly IJSRuntime _js;
    private readonly IReadOnlyList<SessionProviderOption> _providers;
    private SessionCompletionSelection _current = new("ollama", "google/gemma-4-26b-a4b");
    private string _persistKey = DefaultPersistKey;
    private bool _hydrated;

    public AgentSelectionService(IJSRuntime js)
    {
        _js = js;
        _providers = DefaultProviders;
    }

    public IReadOnlyList<SessionProviderOption> Providers => _providers;

    public SessionCompletionSelection CurrentSelection => _current;

    public event Action<SessionCompletionSelection>? SelectionChanged;

    public async Task HydrateAsync(string? persistKey = null)
    {
        if (_hydrated)
            return;

        _hydrated = true;
        _persistKey = persistKey ?? DefaultPersistKey;

        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", _persistKey);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var stored = JsonSerializer.Deserialize(json, BlocksJsonContext.Default.SessionCompletionSelection);
            if (stored is null)
                return;

            _current = NormalizeSelection(stored);
            RaiseSelectionChanged();
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSException)
        {
        }
        catch (JsonException)
        {
        }
    }

    public Task SelectProviderAsync(string providerKey)
    {
        var provider = FindProvider(providerKey);
        var model = provider.Models.First();
        var apiKey = provider.RequiresApiKey ? _current.ApiKey : null;
        return SetSelectionAsync(new SessionCompletionSelection(provider.Key, model.Key, apiKey));
    }

    public Task SelectModelAsync(string providerKey, string modelKey)
    {
        var provider = FindProvider(providerKey);
        var model = provider.Models.FirstOrDefault(m =>
            string.Equals(m.Key, modelKey, StringComparison.OrdinalIgnoreCase))
            ?? provider.Models.First();
        var apiKey = provider.RequiresApiKey ? _current.ApiKey : null;
        return SetSelectionAsync(new SessionCompletionSelection(provider.Key, model.Key, apiKey));
    }

    public Task SetApiKeyAsync(string? apiKey)
    {
        var provider = FindProvider(_current.ProviderKey);
        if (!provider.RequiresApiKey)
            return Task.CompletedTask;

        return SetSelectionAsync(_current with { ApiKey = apiKey?.Trim() });
    }

    private async Task SetSelectionAsync(SessionCompletionSelection selection)
    {
        _current = NormalizeSelection(selection);
        RaiseSelectionChanged();
        await PersistAsync();
    }

    private SessionCompletionSelection NormalizeSelection(SessionCompletionSelection selection)
    {
        var provider = FindProvider(selection.ProviderKey);
        var model = provider.Models.FirstOrDefault(m =>
            string.Equals(m.Key, selection.ModelKey, StringComparison.OrdinalIgnoreCase))
            ?? provider.Models.First();
        return new SessionCompletionSelection(provider.Key, model.Key, selection.ApiKey);
    }

    private SessionProviderOption FindProvider(string providerKey)
    {
        return _providers.FirstOrDefault(p =>
            string.Equals(p.Key, providerKey, StringComparison.OrdinalIgnoreCase))
            ?? _providers.First();
    }

    private async Task PersistAsync()
    {
        try
        {
            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                _persistKey,
                JsonSerializer.Serialize(_current, BlocksJsonContext.Default.SessionCompletionSelection));
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSException)
        {
        }
    }

    private void RaiseSelectionChanged()
    {
        SelectionChanged?.Invoke(_current);
    }
}
