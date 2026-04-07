namespace LowlandTech.Components.Sonner;

public interface ISonnerService
{
    IReadOnlyList<SonnerToast> Toasts { get; }
    event Action? OnChanged;
    Guid Show(string message, ToastType type = ToastType.Default, int duration = 4000, string? description = null);
    Guid Promise(string message, Func<Task> operation, int duration = 4000, string? description = null);
    void Dismiss(Guid id);
    void DismissAll();
}
