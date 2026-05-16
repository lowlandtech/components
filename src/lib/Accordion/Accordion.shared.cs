namespace LowlandTech.Components.Accordion;

// This file extends the auto-generated Accordion partial class.
public partial class Accordion : IDisposable
{
    [Parameter] public string Type { get; set; } = "single";
    [Parameter] public bool Collapsible { get; set; }
    [Parameter] public string? DefaultValue { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    internal AccordionState State { get; private set; } = new();
    internal List<AccordionItem> Items { get; } = new();
    private string? _previousValue;

    protected override void OnInitialized()
    {
        State.Type = Type;
        State.Collapsible = Collapsible;
        
        // Initialize with controlled Value or DefaultValue
        if (!string.IsNullOrEmpty(Value))
            State.SetOpen(Value);
        else if (!string.IsNullOrEmpty(DefaultValue))
            State.SetOpen(DefaultValue);

        State.OnChange += OnStateChanged;
    }

    protected override void OnParametersSet()
    {
        State.Type = Type;
        State.Collapsible = Collapsible;

        // Handle controlled state changes (Value parameter)
        if (Value != _previousValue)
        {
            if (Value != null)
            {
                State.SetOpen(Value);
            }
            else if (_previousValue != null)
            {
                // Value was cleared
                State.CloseAll();
            }
            _previousValue = Value;
        }
    }

    private void OnStateChanged()
    {
        // Notify controlled state change
        if (Type == "single")
        {
            var firstOpen = State.OpenItems.FirstOrDefault();
            if (firstOpen != Value)
            {
                _previousValue = firstOpen;
                Value = firstOpen;
                InvokeAsync(() => ValueChanged.InvokeAsync(firstOpen));
            }
        }

        InvokeAsync(StateHasChanged);
    }

    internal void RegisterItem(AccordionItem item)
    {
        if (!Items.Contains(item))
            Items.Add(item);
    }

    internal void UnregisterItem(AccordionItem item)
    {
        Items.Remove(item);
    }

    internal AccordionItem? GetNextItem(AccordionItem current)
    {
        var index = Items.IndexOf(current);
        if (index >= 0 && index < Items.Count - 1)
            return Items[index + 1];
        return null;
    }

    internal AccordionItem? GetPreviousItem(AccordionItem current)
    {
        var index = Items.IndexOf(current);
        if (index > 0)
            return Items[index - 1];
        return null;
    }

    internal AccordionItem? GetFirstItem()
    {
        return Items.FirstOrDefault();
    }

    internal AccordionItem? GetLastItem()
    {
        return Items.LastOrDefault();
    }

    internal void FocusItem(AccordionItem? item)
    {
        item?.FocusTrigger();
    }

    public void Dispose()
    {
        State.OnChange -= OnStateChanged;
    }
}
