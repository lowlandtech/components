namespace LowlandTech.Components;

public partial class SessionCommandBar : TinyComponentBase, IDisposable
{
    private const string ProvidersCommand = "providers";
    private const string ModelsCommand = "models";
    private const string ValueSeparator = "::";

    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IAgentSelectionService AgentSelection { get; set; } = default!;
    [Inject] private IServiceProvider Services { get; set; } = default!;

    private IEdgeTypeService? EdgeTypeService => Services.GetService<IEdgeTypeService>();

    [Parameter] public SessionUiState State { get; set; } = SessionUiDefaults.Empty;
    [Parameter] public EventCallback<SessionUiState> StateChanged { get; set; }
    [Parameter] public SessionNodeInput? SelectedNode { get; set; }
    [Parameter] public IReadOnlyList<SessionAgentType> AgentTypes { get; set; } = SessionUiDefaults.AgentTypes;
    [Parameter] public string ActiveAgentTypeKey { get; set; } = "ask";
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Ask about the active session...";
    [Parameter] public string SubmitTitle { get; set; } = "Send";
    [Parameter] public int Rows { get; set; } = 3;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool IsBusy { get; set; }
    [Parameter] public EventCallback<SessionAgentTypeSelected> OnAgentTypeSelected { get; set; }
    [Parameter] public EventCallback<SessionCommandSubmitted> OnSubmit { get; set; }
    [Parameter] public string CompletionPersistKey { get; set; } = "session-command-bar:completion";

    private string? _activeSlashCommand;
    private string _draftApiKey = string.Empty;
    private string _selectionValue = string.Empty;
    private bool _subscribed;

    private string CurrentInput => Value is not null ? Value : State.Input;
    private SessionProviderOption ActiveProvider =>
        AgentSelection.Providers.FirstOrDefault(option =>
            string.Equals(option.Key, AgentSelection.CurrentSelection.ProviderKey, StringComparison.OrdinalIgnoreCase))
        ?? AgentSelection.Providers.First();
    private SessionProviderModelOption ActiveModel =>
        ActiveProvider.Models.FirstOrDefault(option =>
            string.Equals(option.Key, AgentSelection.CurrentSelection.ModelKey, StringComparison.OrdinalIgnoreCase))
        ?? ActiveProvider.Models.First();

    protected override void OnInitialized()
    {
        Subscribe();
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        SelectedNode ??= State.SelectedNode;
        Value ??= State.Input;
        if (string.IsNullOrWhiteSpace(ActiveAgentTypeKey))
            ActiveAgentTypeKey = State.ActiveAgentTypeKey;
        _activeSlashCommand ??= ParseSlashCommand(CurrentInput);
        UpdateLocalSelection(AgentSelection.CurrentSelection);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InstallEnterSubmitHandlerAsync();
            await AgentSelection.HydrateAsync(CompletionPersistKey);
        }
    }

    private void Subscribe()
    {
        if (_subscribed)
            return;

        AgentSelection.SelectionChanged += OnSelectionChanged;
        _subscribed = true;
    }

    private void OnSelectionChanged(SessionCompletionSelection selection)
    {
        UpdateLocalSelection(selection);
        InvokeAsync(StateHasChanged);
    }

    private void UpdateLocalSelection(SessionCompletionSelection selection)
    {
        _selectionValue = GetCompositeKey(selection);
        _draftApiKey = selection.ApiKey ?? string.Empty;
    }

    public void Dispose()
    {
        if (_subscribed)
            AgentSelection.SelectionChanged -= OnSelectionChanged;
    }

    private string GetClass()
    {
        var baseClass = "flex w-full flex-col rounded-lg border border-primary bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private string GetContextNodeTypeKey()
    {
        if (SelectedNode is null)
            return string.Empty;

        if (SelectedNode.Properties?.GetValueOrDefault("createTargetNodeTypeKey") is string targetNodeTypeKey
            && !string.IsNullOrWhiteSpace(targetNodeTypeKey))
            return targetNodeTypeKey;

        var edge = EdgeTypeService?.FindEdgeByAgentTypeKey(SelectedNode, ActiveAgentTypeKey);
        if (edge is not null && !string.IsNullOrWhiteSpace(edge.TargetNodeTypeKey))
            return edge.TargetNodeTypeKey;

        return SelectedNode.NodeTypeKey;
    }

    private string? GetTransitionKey()
    {
        if (SelectedNode is null)
            return null;

        var edge = EdgeTypeService?.FindEdgeByAgentTypeKey(SelectedNode, ActiveAgentTypeKey);
        if (edge is null)
            return null;

        var sourceNoun = GetSourceNoun(SelectedNode.NodeTypeKey);
        var verb = string.IsNullOrWhiteSpace(edge.Intent)
            ? ActiveAgentTypeKey.ToLowerInvariant()
            : edge.Intent.ToLowerInvariant();

        return $"{sourceNoun}.{verb}.{edge.TargetNodeTypeKey}".ToLowerInvariant();
    }

    private static string GetSourceNoun(string nodeTypeKey) =>
        nodeTypeKey.EndsWith(".header", StringComparison.OrdinalIgnoreCase)
            ? nodeTypeKey[..^".header".Length]
            : nodeTypeKey;

    private bool IsAgentActive(SessionAgentType agent)
    {
        if (string.Equals(agent.Key, ActiveAgentTypeKey, StringComparison.OrdinalIgnoreCase))
            return true;

        // The static Create pill stays active when a specific create edge
        // is selected from the context menu (e.g. "create:has.workspace").
        if (string.Equals(agent.Key, "create", StringComparison.OrdinalIgnoreCase)
            && ActiveAgentTypeKey.StartsWith("create:", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static string GetAgentClass(SessionAgentType agent, bool isActive)
    {
        var stateClass = isActive
            ? "border-primary bg-primary text-primary-foreground"
            : agent.Disabled
                ? "border-primary/30 text-primary/40 cursor-not-allowed"
                : "border-primary/60 text-primary hover:border-primary hover:bg-primary/10";

        return $"inline-flex h-8 items-center rounded-md border px-3 text-xs font-medium transition-colors {stateClass}";
    }

    private async Task HandleInput(ChangeEventArgs e)
    {
        var nextValue = e.Value?.ToString() ?? string.Empty;
        _activeSlashCommand = ParseSlashCommand(nextValue);
        Value = nextValue;
        await ValueChanged.InvokeAsync(nextValue);

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                Input = nextValue,
                SelectedNode = SelectedNode ?? State.SelectedNode,
                ActiveAgentTypeKey = string.IsNullOrWhiteSpace(ActiveAgentTypeKey)
                    ? State.ActiveAgentTypeKey
                    : ActiveAgentTypeKey
            });
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key != "Enter" || e.ShiftKey)
            return;

        if (e.CtrlKey || e.MetaKey)
        {
            await SubmitAsync(isControlEnter: true);
            return;
        }

        await SubmitAsync();
    }

    private async Task SelectAgentAsync(SessionAgentType agent)
    {
        if (agent.Disabled || SelectedNode is null)
            return;

        ActiveAgentTypeKey = agent.Key;
        await agent.OnSelected.InvokeAsync(SelectedNode);
        await OnAgentTypeSelected.InvokeAsync(new SessionAgentTypeSelected(SelectedNode, agent.Key));

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                ActiveAgentTypeKey = agent.Key,
                SelectedNode = SelectedNode,
                Input = CurrentInput
            });
        }
    }

    private async Task SubmitAsync(bool isControlEnter = false)
    {
        var input = CurrentInput;
        if (Disabled || IsBusy || SelectedNode is null || string.IsNullOrWhiteSpace(input))
            return;
        if (ParseSlashCommand(input) is not null)
            return;

        var agent = AgentTypes.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, ActiveAgentTypeKey, StringComparison.OrdinalIgnoreCase));
        if (agent?.Disabled == true)
            return;

        var submitted = new SessionCommandSubmitted(
            SelectedNode,
            ActiveAgentTypeKey,
            input,
            Completion: AgentSelection.CurrentSelection,
            IsControlEnter: isControlEnter);
        Value = string.Empty;

        await ValueChanged.InvokeAsync(Value);

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                Input = string.Empty,
                SelectedNode = SelectedNode,
                ActiveAgentTypeKey = ActiveAgentTypeKey
            });
        }

        await ResetInputElementAsync();

        if (agent is not null)
            await agent.OnSubmit.InvokeAsync(submitted);
        await OnSubmit.InvokeAsync(submitted);
    }

    private Task OpenModelsAsync()
    {
        _activeSlashCommand = ModelsCommand;
        return InvokeAsync(StateHasChanged);
    }

    private async Task SelectProviderAsync(SessionProviderOption provider)
    {
        _activeSlashCommand = ModelsCommand;
        await AgentSelection.SelectProviderAsync(provider.Key);
    }

    private async Task SelectModelAsync(SessionProviderModelOption model)
    {
        _activeSlashCommand = null;
        Value = string.Empty;
        await ValueChanged.InvokeAsync(Value);
        await AgentSelection.SelectModelAsync(ActiveProvider.Key, model.Key);
    }

    private Task HandleApiKeyInput(ChangeEventArgs e)
    {
        _draftApiKey = e.Value?.ToString() ?? string.Empty;
        return Task.CompletedTask;
    }

    private async Task SaveApiKeyAsync()
    {
        _activeSlashCommand = null;
        Value = string.Empty;
        await ValueChanged.InvokeAsync(Value);
        await AgentSelection.SetApiKeyAsync(_draftApiKey.Trim());
    }

    private async Task HandleSelectionChangeAsync(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        var (providerKey, modelKey) = ParseCompositeKey(value);
        await AgentSelection.SelectModelAsync(providerKey, modelKey);
        _selectionValue = value;
    }

    private static string GetCompositeKey(SessionCompletionSelection selection)
        => $"{selection.ProviderKey}{ValueSeparator}{selection.ModelKey}";

    private static (string ProviderKey, string ModelKey) ParseCompositeKey(string value)
    {
        var parts = value.Split(ValueSeparator, 2);
        return (parts[0], parts.Length > 1 ? parts[1] : string.Empty);
    }

    private static string? ParseSlashCommand(string value)
    {
        var command = value.Trim().ToLowerInvariant();
        return command switch
        {
            "/providers" => ProvidersCommand,
            "/models" => ModelsCommand,
            _ => null
        };
    }

    private static string GetPickerItemClass(bool active) =>
        $"flex h-9 w-full items-center justify-between rounded-md px-2 text-left text-sm hover:bg-accent {(active ? "bg-accent" : string.Empty)}";

    private async Task ResetInputElementAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (!input) return;

                    input.value = '';
                    input.scrollTop = 0;
                    input.focus({ preventScroll: true });
                    input.setSelectionRange(0, 0);
                })();
                """);
        }
        catch
        {
        }
    }

    private async Task InstallEnterSubmitHandlerAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    document.querySelectorAll('[data-slot="session-command-input"]').forEach((input) => {
                        if (input.dataset.enterSubmitBound === 'true') return;
                        input.dataset.enterSubmitBound = 'true';

                        input.addEventListener('keydown', (event) => {
                            if (event.key !== 'Enter' || event.shiftKey || event.isComposing || event.ctrlKey || event.metaKey) return;

                            event.preventDefault();
                            event.stopPropagation();

                            const commandBar = input.closest('[data-slot="session-command-bar"]');
                            const submit = commandBar?.querySelector('[data-slot="session-command-submit"]');
                            if (submit && !submit.disabled) {
                                submit.click();
                            }
                        }, true);
                    });
                })();
                """);
        }
        catch
        {
        }
    }
}
