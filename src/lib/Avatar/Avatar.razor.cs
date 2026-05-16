using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Avatar;

public partial class Avatar
{
    [Parameter] public string Size { get; set; } = "default";

    internal bool ImageLoaded { get; set; }
    internal bool ImageError { get; set; }

    internal void MarkImageLoaded()
    {
        ImageLoaded = true;
        ImageError = false;
        StateHasChanged();
    }

    internal void MarkImageError()
    {
        ImageError = true;
        ImageLoaded = false;
        StateHasChanged();
    }
}
