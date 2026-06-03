namespace LowlandTech.Components.Work;

public partial class WorkAppMain
{
    private string GetClass()
    {
        var baseClass = "flex min-h-0 min-w-0 flex-col overflow-hidden";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
