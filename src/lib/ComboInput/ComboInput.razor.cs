using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components;

public partial class ComboInput
{
    [Parameter] public string Text { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> TextChanged { get; set; }
    [Parameter] public List<string> Options { get; set; } = [];
    [Parameter] public string Placeholder { get; set; } = string.Empty;
    [Parameter] public Func<string, string>? DisplaySelector { get; set; }
    [Parameter] public EventCallback OnEnter { get; set; }

    private string Display(string option) => DisplaySelector?.Invoke(option) ?? option;

    private bool _open;
    private List<string> _filtered = [];

    private async Task OnInput(ChangeEventArgs e)
    {
        Text = e.Value?.ToString() ?? string.Empty;
        await TextChanged.InvokeAsync(Text);
        Filter();
        _open = _filtered.Any();
    }

    private void Open()
    {
        _filtered = Options.Take(100).ToList();
        _open = _filtered.Any();
    }

    private void Close() => _open = false;

    private void Filter()
    {
        _filtered = string.IsNullOrWhiteSpace(Text)
            ? Options.Take(100).ToList()
            : Options.Where(o => Display(o).Contains(Text, StringComparison.OrdinalIgnoreCase))
                     .Take(100).ToList();
    }

    private async Task SelectOption(string option)
    {
        Text = option;
        _open = false;
        await TextChanged.InvokeAsync(Text);
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            _open = false;
        }
        else if (e.Key == "Enter")
        {
            _open = false;
            await OnEnter.InvokeAsync();
        }
    }
}
