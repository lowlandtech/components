using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Components.Accordion;

namespace LowlandTech.Components.Components.Accordion;

public record AccordionItemOptions(string? Value, bool Disabled = false);

public partial class Accordion : ComponentBase
{
    private List<AccordionItemOptions> _items = new();
    private HashSet<string> _openValues = new();
    
    [Parameter] public bool Collapsible { get; set; }
    [Parameter] public bool OneOpen { get; set; } = true;
    [Parameter] public int? DefaultOpenValue { get; set; }
    [Parameter] public EventCallback<string> OnToggle { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        if (DefaultOpenValue.HasValue)
        {
            var value = DefaultOpenValue.Value.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                _openValues.Add(value);
            }
        }
    }

    public async Task HandleToggle(string value, bool isOpen)
    {
        if (isOpen)
        {
            if (OneOpen && !_openValues.Contains(value))
            {
                _openValues.Clear();
                _openValues.Add(value);
            }
            else
            {
                _openValues.Add(value);
            }
        }
        else
        {
            _openValues.Remove(value);
        }
        await OnToggle.InvokeAsync(value);
    }

    public bool IsOpen(string value)
    {
        if (DefaultOpenValue.HasValue)
        {
            return DefaultOpenValue.Value.ToString() == value;
        }
        return _openValues.Contains(value);
    }

    public bool IsItemVisible(string value)
    {
        if (OneOpen)
        {
            return DefaultOpenValue.HasValue && DefaultOpenValue.Value.ToString() == value;
        }
        return IsOpen(value);
    }
}
