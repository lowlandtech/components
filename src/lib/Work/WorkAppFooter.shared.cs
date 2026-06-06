namespace LowlandTech.Components;

public partial class WorkAppFooter
{
    [CascadingParameter] public WorkAppContext? Context { get; set; }

    private string GetClass()
    {
        var baseClass = "flex shrink-0 px-4 py-3";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
