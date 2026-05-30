namespace LowlandTech.Components.Card;

public interface IPageOverlayService
{
    RenderFragment? Content { get; }
    string? Title { get; }
    string? ContainerClass { get; }
    event Action? OnChanged;
    void Set(RenderFragment? content, string? title = null, string? containerClass = null);
    void Clear();
}

public class PageOverlayService : IPageOverlayService
{
    public RenderFragment? Content { get; private set; }
    public string? Title { get; private set; }
    public string? ContainerClass { get; private set; }
    public event Action? OnChanged;

    public void Set(RenderFragment? content, string? title = null, string? containerClass = null)
    {
        Content = content;
        Title = title;
        ContainerClass = containerClass;
        OnChanged?.Invoke();
    }

    public void Clear() => Set(null);
}
