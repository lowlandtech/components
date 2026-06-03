namespace LowlandTech.Components.Work;

public partial class WorkAppHeader
{
    [CascadingParameter] public WorkAppContext? Context { get; set; }

    private string GetClass()
    {
        var baseClass = "flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
