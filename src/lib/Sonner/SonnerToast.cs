namespace LowlandTech.Components.Sonner;

public enum ToastType { Default, Success, Info, Warning, Error, Promise, Loading }

public enum ToastPosition
{
    TopLeft, TopCenter, TopRight,
    BottomLeft, BottomCenter, BottomRight
}

public record SonnerToast(
    Guid Id, 
    string Message, 
    ToastType Type = ToastType.Default, 
    int Duration = 4000,
    string? Description = null,
    ToastPosition Position = ToastPosition.BottomRight,
    bool IsDismissible = true)
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsExiting { get; set; } = false;
}

public record ToastAction(string Label, Func<Task> OnClick);
