namespace LowlandTech.Components.Sonner;

public class SonnerService : ISonnerService
{
    private readonly List<SonnerToast> _toasts = [];

    public IReadOnlyList<SonnerToast> Toasts => _toasts.AsReadOnly();

    public event Action? OnChanged;

    public Guid Show(string message, ToastType type = ToastType.Default, int duration = 4000)
    {
        var toast = new SonnerToast(Guid.NewGuid(), message, type, duration);
        _toasts.Add(toast);
        OnChanged?.Invoke();
        return toast.Id;
    }

    public void Dismiss(Guid id)
    {
        _toasts.RemoveAll(t => t.Id == id);
        OnChanged?.Invoke();
    }

    public void DismissAll()
    {
        _toasts.Clear();
        OnChanged?.Invoke();
    }
}
