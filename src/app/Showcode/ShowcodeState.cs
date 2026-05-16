namespace LowlandTech.Components.App.Showcode;

public class ShowcodeState
{
    private StoryMetadata? _current;

    public StoryMetadata? Current => _current;

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

    public void Clear()
    {
        _current = null;
        Changed?.Invoke();
    }
}
