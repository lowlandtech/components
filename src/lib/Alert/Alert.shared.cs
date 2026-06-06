namespace LowlandTech.Components;

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
            "destructive" => " cn-alert-destructive",
            _ => ""
        };

        var baseClass = $"cn-alert group/alert{variantClass}";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
