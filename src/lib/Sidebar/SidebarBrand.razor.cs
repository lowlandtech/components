namespace LowlandTech.Components.Sidebar;

public partial class SidebarBrand : TinyComponentBase
{
    [Parameter] public string? LogoSrc { get; set; }
    [Parameter] public string LogoAlt { get; set; } = "";
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? LogoClass { get; set; }

    private string GetClass()
    {
        var baseClass = "flex items-center gap-2 px-2";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private string GetLogoClass()
    {
        var baseClass = "h-6 w-6 shrink-0 rounded-md";
        return string.IsNullOrWhiteSpace(LogoClass) ? baseClass : $"{baseClass} {LogoClass}";
    }
}
