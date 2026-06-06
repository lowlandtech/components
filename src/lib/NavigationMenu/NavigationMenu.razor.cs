namespace LowlandTech.Components;

public partial class NavigationMenu : TinyComponentBase
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public string? DefaultValue { get; set; }

    internal string? ActiveValue { get; private set; }
    internal event Action? OnActiveValueChanged;

    protected override void OnInitialized()
    {
        ActiveValue = !string.IsNullOrEmpty(Value) ? Value : DefaultValue;
    }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrEmpty(Value))
            ActiveValue = Value;
    }

    internal async Task ActivateItemAsync(string? value)
    {
        ActiveValue = value;
        await ValueChanged.InvokeAsync(value);
        OnActiveValueChanged?.Invoke();
        StateHasChanged();
    }

    internal bool IsItemActive(string? value) => ActiveValue == value;
}
