using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Alert;

/// <summary>
/// Extends the auto-generated Alert partial class.
/// </summary>
public partial class Alert
{
    /// <summary>
    /// The variant of the alert.
    /// </summary>
    [Parameter] public string Variant { get; set; } = "default";

    private string GetClass()
    {
        var variantClass = Variant switch
        {
            "destructive" => " cn-alert-destructive border-destructive/50 text-destructive dark:border-destructive [&>svg]:text-destructive",
            _ => " bg-background text-foreground [&>svg]:text-foreground"
        };

        var baseClass = $"cn-alert group/alert relative w-full rounded-lg border p-4 [&>svg]:absolute [&>svg]:left-4 [&>svg]:top-4 [&>svg~*]:pl-7{variantClass}";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
