using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Spinner;

public partial class Spinner
{
    [Parameter] public string? Color { get; set; }

    private string CssClass
    {
        get
        {
            var baseClass = "cn-spinner inline-flex size-4 animate-spin items-center justify-center";
            if (!string.IsNullOrWhiteSpace(Color))
            {
                baseClass += $" {Color}";
            }
            if (!string.IsNullOrWhiteSpace(Class))
            {
                baseClass += $" {Class}";
            }
            return baseClass;
        }
    }
}
