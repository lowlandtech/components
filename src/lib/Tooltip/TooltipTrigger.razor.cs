namespace LowlandTech.Components;

public partial class TooltipTrigger : TinyComponentBase
{
    [CascadingParameter] private Tooltip? Parent { get; set; }

    private async Task OnMouseEnter()
    {
        if (Parent != null)
        {
            await Parent.ShowAsync();
        }
    }

    private async Task OnMouseLeave()
    {
        if (Parent != null)
        {
            await Parent.HideAsync();
        }
    }

    private async Task OnFocus()
    {
        if (Parent != null)
        {
            await Parent.ShowAsync();
        }
    }

    private async Task OnBlur()
    {
        if (Parent != null)
        {
            await Parent.HideAsync();
        }
    }

    private string GetClass()
    {
        var baseClass = "cn-tooltip-trigger inline-flex items-center justify-center select-none";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
