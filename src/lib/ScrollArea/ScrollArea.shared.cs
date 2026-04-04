namespace LowlandTech.Components.ScrollArea;

public partial class ScrollArea
{
    private string CssClass
    {
        get
        {
            const string baseClass = "cn-scroll-area relative";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
