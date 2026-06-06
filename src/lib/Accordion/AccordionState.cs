namespace LowlandTech.Components;

public class AccordionState
{
    public string Type { get; set; } = "single";
    public bool Collapsible { get; set; }

    private readonly HashSet<string> _openItems = [];

    public event Action? OnChange;

    public bool IsOpen(string value) => _openItems.Contains(value);

    public void Toggle(string value)
    {
        if (_openItems.Contains(value))
        {
            if (Type == "single" && !Collapsible)
                return;

            _openItems.Remove(value);
        }
        else
        {
            if (Type == "single")
                _openItems.Clear();

            _openItems.Add(value);
        }

        OnChange?.Invoke();
    }

    public void SetOpen(string value)
    {
        if (Type == "single")
            _openItems.Clear();

        _openItems.Add(value);
        OnChange?.Invoke();
    }

    public void CloseAll()
    {
        if (_openItems.Count > 0)
        {
            _openItems.Clear();
            OnChange?.Invoke();
        }
    }

    public IReadOnlySet<string> OpenItems => _openItems;
}
