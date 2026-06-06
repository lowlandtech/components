namespace LowlandTech.Components;

public partial class WorkAppSidebar
{
    [CascadingParameter] public WorkAppContext? Context { get; set; }

    private string GetClass()
    {
        var baseClass = "min-h-0 border-b border-border bg-muted/20 lg:border-b-0 lg:border-r";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
