namespace LowlandTech.Components.Card;

public interface IPageOverlayService
{
    RenderFragment? Content { get; }
    string? Title { get; }
    event Action? OnChanged;
    void Set(RenderFragment? content, string? title = null);
    void Clear();
}

public class PageOverlayService : IPageOverlayService
{
    public RenderFragment? Content { get; private set; }
    public string? Title { get; private set; }
    public event Action? OnChanged;

    public void Set(RenderFragment? content, string? title = null)
    {
        Content = content;
        Title = title;
        OnChanged?.Invoke();
    }

    public void Clear() => Set(null);
}
