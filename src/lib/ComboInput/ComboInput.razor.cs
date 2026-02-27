using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.ComboInput;

public partial class ComboInput
{
    [Parameter] public string Text { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> TextChanged { get; set; }
    [Parameter] public List<string> Options { get; set; } = [];
    [Parameter] public string Placeholder { get; set; } = string.Empty;
    [Parameter] public EventCallback OnEnter { get; set; }

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
        Filter();
        _open = _filtered.Any();
    }

    private void Close() => _open = false;

    private void Filter()
    {
        _filtered = string.IsNullOrWhiteSpace(Text)
            ? Options.Take(20).ToList()
            : Options.Where(o => o.Contains(Text, StringComparison.OrdinalIgnoreCase))
                     .Take(20).ToList();
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
