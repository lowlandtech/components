namespace LowlandTech.Components.SelectInput;

public partial class SelectInput<TValue>
{
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }
    [Parameter] public List<SelectOption<TValue>> Options { get; set; } = [];
    [Parameter] public string Placeholder { get; set; } = "Select...";

    /// <summary>SVG path data for the chevron-down icon. Defaults to Lucide chevron-down.</summary>
    [Parameter] public string ChevronDownIcon { get; set; } = """<path d="m6 9 6 6 6-6"/>""";

    /// <summary>SVG path data for the check icon. Defaults to Lucide check.</summary>
    [Parameter] public string CheckIcon { get; set; } = """<path d="M20 6 9 17l-5-5"/>""";

    private bool _open;

    private string? DisplayValue => Options.FirstOrDefault(o =>
        EqualityComparer<TValue>.Default.Equals(o.Value, Value))?.Label;

    private void Toggle() => _open = !_open;
    private void Close() => _open = false;

    private async Task SelectOption(TValue value)
    {
        Value = value;
        _open = false;
        await ValueChanged.InvokeAsync(value);
    }


}
