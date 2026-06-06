namespace LowlandTech.Components;

public partial class InputGroup
{
    private string CssClass
    {
        get
        {
            const string baseClass =
                "group/input-group cn-input-group relative flex w-full min-w-0 items-center outline-none " +
                "has-[>textarea]:h-auto";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
