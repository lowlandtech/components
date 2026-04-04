namespace LowlandTech.Components.Sonner;

public interface ISonnerService
{
    IReadOnlyList<SonnerToast> Toasts { get; }
    event Action? OnChanged;
    Guid Show(string message, ToastType type = ToastType.Default, int duration = 4000);
    void Dismiss(Guid id);
    void DismissAll();
}
