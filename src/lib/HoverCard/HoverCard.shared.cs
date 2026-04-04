namespace LowlandTech.Components.HoverCard;

public partial class HoverCard
{
    private string CssClass
    {
        get
        {
            const string baseClass = "cn-hover-card group/hover-card relative inline-block";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
