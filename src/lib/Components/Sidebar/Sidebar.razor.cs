using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;
using LowlandTech.Components.Components.Sheet;

namespace LowlandTech.Components.Components.Sidebar;

public enum SidebarSide
{
    Left,
    Right
}

public enum SidebarVariant
{
    Sidebar,
    Floating,
    Inset
}

public enum SidebarCollapsible
{
    Offcanvas,
    Icon,
    None
}

public partial class Sidebar : ComponentBase
{
    private const string SidebarWidthMobile = "18rem";

    [CascadingParameter] private SidebarProvider? Provider { get; set; }

    [Parameter] public SidebarSide Side { get; set; } = SidebarSide.Left;
    [Parameter] public SidebarVariant Variant { get; set; } = SidebarVariant.Sidebar;
    [Parameter] public SidebarCollapsible Collapsible { get; set; } = SidebarCollapsible.Offcanvas;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => CssClassUtils.Merge(
        "bg-sidebar",
        "text-sidebar-foreground",
        "flex",
        "h-full",
        "w-[var(--sidebar-width)]",
        "flex-col",
        Class
    );

    private string MobileClasses => CssClassUtils.Merge(
        "bg-sidebar",
        "text-sidebar-foreground",
        "w-[var(--sidebar-width)]",
        "p-0",
        "[&>button]:hidden",
        Class
    );

    private string MobileStyle => $"--sidebar-width: {SidebarWidthMobile}";

    private string NonCollapsibleClasses => CssClassUtils.Merge(
        "bg-sidebar",
        "text-sidebar-foreground",
        "flex",
        "h-full",
        "w-[var(--sidebar-width)]",
        "flex-col",
        Class
    );

    private string GroupClasses => "group peer text-sidebar-foreground hidden md:block";

    private string GapClasses
    {
        get
        {
            var baseClasses = CssClassUtils.Merge(
                "relative",
                "w-[var(--sidebar-width)]",
                "bg-transparent",
                "transition-[width]",
                "duration-200",
                "ease-linear",
                "group-data-[collapsible=offcanvas]:w-0",
                "group-data-[side=right]:rotate-180"
            );

            var variantClasses = Variant is SidebarVariant.Floating or SidebarVariant.Inset
                ? "group-data-[collapsible=icon]:w-[calc(var(--sidebar-width-icon)+theme(spacing.4))]"
                : "group-data-[collapsible=icon]:w-[var(--sidebar-width-icon)]";

            return CssClassUtils.Merge(baseClasses, variantClasses);
        }
    }

    private string ContainerClasses
    {
        get
        {
            var baseClasses = CssClassUtils.Merge(
                "fixed",
                "inset-y-0",
                "z-10",
                "hidden",
                "h-svh",
                "w-[var(--sidebar-width)]",
                "transition-[left,right,width]",
                "duration-200",
                "ease-linear",
                "data-[side=left]:left-0",
                "data-[side=left]:group-data-[collapsible=offcanvas]:left-[calc(var(--sidebar-width)*-1)]",
                "data-[side=right]:right-0",
                "data-[side=right]:group-data-[collapsible=offcanvas]:right-[calc(var(--sidebar-width)*-1)]",
                "md:flex"
            );

            var variantClasses = Variant is SidebarVariant.Floating or SidebarVariant.Inset
                ? CssClassUtils.Merge(
                    "p-2",
                    "group-data-[collapsible=icon]:w-[calc(var(--sidebar-width-icon)+theme(spacing.4)+2px)]"
                )
                : CssClassUtils.Merge(
                    "group-data-[collapsible=icon]:w-[var(--sidebar-width-icon)]",
                    "group-data-[side=left]:border-r",
                    "group-data-[side=right]:border-l"
                );

            return CssClassUtils.Merge(baseClasses, variantClasses, Class);
        }
    }

    private string InnerClasses => CssClassUtils.Merge(
        "bg-sidebar",
        "flex",
        "size-full",
        "flex-col",
        "group-data-[variant=floating]:rounded-lg",
        "group-data-[variant=floating]:shadow-sm",
        "group-data-[variant=floating]:ring-1",
        "group-data-[variant=floating]:ring-sidebar-border"
    );

    private SheetSide ConvertToSheetSide(SidebarSide side)
    {
        return side == SidebarSide.Left ? SheetSide.Left : SheetSide.Right;
    }
}
