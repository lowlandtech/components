namespace LowlandTech.Components.App.Showcode;

public class ShowcodeState
{
    private StoryMetadata? _current;

    public StoryMetadata? Current => _current;
    public bool IsTableMode { get; private set; }

    public event Action? Changed;

    public void Select(StoryMetadata story)
    {
        _current = story;
        Changed?.Invoke();
    }

    public void SelectFirst()
    {
        var first = StoryRegistry.GetFirst();
        if (first is not null) Select(first);
    }

    public void ShowTable()
    {
        _current = null;
        IsTableMode = true;
        Changed?.Invoke();
    }

    public void Clear()
    {
        _current = null;
        IsTableMode = false;
        Changed?.Invoke();
    }
}
