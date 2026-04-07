namespace LowlandTech.Components.Sonner;

public class SonnerService : ISonnerService
{
    private readonly List<SonnerToast> _toasts = [];

    public IReadOnlyList<SonnerToast> Toasts => _toasts.AsReadOnly();

    public event Action? OnChanged;

    public Guid Show(string message, ToastType type = ToastType.Default, int duration = 4000, string? description = null)
    {
        var toast = new SonnerToast(Guid.NewGuid(), message, type, duration, description);
        _toasts.Add(toast);
        OnChanged?.Invoke();
        
        // Auto-dismiss after duration
        if (duration > 0)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(duration);
                await DismissWithAnimationAsync(toast.Id);
            });
        }
        
        return toast.Id;
    }

    public Guid Promise(string message, Func<Task> operation, int duration = 4000, string? description = null)
    {
        // Show loading toast
        var toast = new SonnerToast(Guid.NewGuid(), message, ToastType.Promise, 0, description);
        _toasts.Add(toast);
        OnChanged?.Invoke();

        _ = Task.Run(async () =>
        {
            try
            {
                await operation();
                // Update to success
                UpdateToastType(toast.Id, ToastType.Success, message, duration);
            }
            catch
            {
                // Update to error
                UpdateToastType(toast.Id, ToastType.Error, $"Error: {message}", duration);
            }
        });

        return toast.Id;
    }

    private void UpdateToastType(Guid id, ToastType newType, string? newMessage = null, int duration = 4000)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == id);
        if (toast != null)
        {
            _toasts.Remove(toast);
            var updatedToast = toast with { Type = newType, Message = newMessage ?? toast.Message, Duration = duration };
            _toasts.Add(updatedToast);
            OnChanged?.Invoke();
            
            // Auto-dismiss after duration
            if (duration > 0)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(duration);
                    await DismissWithAnimationAsync(id);
                });
            }
        }
    }

    public void Dismiss(Guid id)
    {
        _ = DismissWithAnimationAsync(id);
    }

    private async Task DismissWithAnimationAsync(Guid id)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == id);
        if (toast != null && !toast.IsExiting)
        {
            // Mark as exiting for animation
            var index = _toasts.IndexOf(toast);
            _toasts[index] = toast with { IsExiting = true };
            OnChanged?.Invoke();
            
            // Wait for exit animation
            await Task.Delay(300);
            
            _toasts.RemoveAll(t => t.Id == id);
            OnChanged?.Invoke();
        }
    }

    public void DismissAll()
    {
        _toasts.Clear();
        OnChanged?.Invoke();
    }
}
