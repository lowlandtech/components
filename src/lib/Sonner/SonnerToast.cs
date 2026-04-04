namespace LowlandTech.Components.Sonner;

public enum ToastType { Default, Success, Info, Warning, Error }

public record SonnerToast(Guid Id, string Message, ToastType Type = ToastType.Default, int Duration = 4000);
