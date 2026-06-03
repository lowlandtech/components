namespace LowlandTech.Components.Work;

public partial class WorkAppDetailPanel
{
    [CascadingParameter] public WorkAppContext? Context { get; set; }

    private string GetClass()
    {
        var baseClass = "min-h-0 overflow-auto border-t border-border bg-card xl:border-l xl:border-t-0";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
