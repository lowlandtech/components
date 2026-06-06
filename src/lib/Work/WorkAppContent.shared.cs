namespace LowlandTech.Components;

public partial class WorkAppContent
{
    [CascadingParameter] public WorkAppContext? Context { get; set; }

    private string GetClass()
    {
        var baseClass = "min-h-0 flex-1 overflow-auto p-4";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
