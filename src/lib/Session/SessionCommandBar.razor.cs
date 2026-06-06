namespace LowlandTech.Components;

public partial class SessionCommandBar : TinyComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

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

    private string CurrentInput => Value is not null ? Value : State.Input;

    protected override void OnParametersSet()
    {
        SelectedNode ??= State.SelectedNode;
        Value ??= State.Input;
        if (string.IsNullOrWhiteSpace(ActiveAgentTypeKey))
            ActiveAgentTypeKey = State.ActiveAgentTypeKey;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await InstallEnterSubmitHandlerAsync();
    }

    private string GetClass()
    {
        var baseClass = "flex w-full flex-col rounded-lg border border-primary bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
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
        if (e.Key == "Enter" && !e.ShiftKey)
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

    private async Task SubmitAsync()
    {
        var input = CurrentInput;
        if (Disabled || IsBusy || SelectedNode is null || string.IsNullOrWhiteSpace(input))
            return;

        var agent = AgentTypes.FirstOrDefault(candidate => string.Equals(candidate.Key, ActiveAgentTypeKey, StringComparison.OrdinalIgnoreCase));
        if (agent?.Disabled == true)
            return;

        var submitted = new SessionCommandSubmitted(SelectedNode, ActiveAgentTypeKey, input);
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
                            if (event.key !== 'Enter' || event.shiftKey || event.isComposing) return;

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