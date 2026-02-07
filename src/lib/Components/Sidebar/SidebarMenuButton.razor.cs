using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Sidebar;

public enum SidebarMenuButtonSize
{
    Default,
    Sm,
    Lg
}

public enum SidebarMenuButtonVariant
{
    Default,
    Outline
}

public partial class SidebarMenuButton : ComponentBase
{
    [CascadingParameter] private SidebarProvider? Provider { get; set; }

    [Parameter] public bool IsActive { get; set; }
    [Parameter] public SidebarMenuButtonVariant Variant { get; set; } = SidebarMenuButtonVariant.Default;
    [Parameter] public SidebarMenuButtonSize Size { get; set; } = SidebarMenuButtonSize.Default;
    [Parameter] public string? TooltipText { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes
    {
        get
        {
            var baseClasses = CssClassUtils.Merge(
                "ring-sidebar-ring",
                "hover:bg-sidebar-accent",
                "hover:text-sidebar-accent-foreground",
                "active:bg-sidebar-accent",
                "active:text-sidebar-accent-foreground",
                "data-active:bg-sidebar-accent",
                "data-active:text-sidebar-accent-foreground",
                "data-open:hover:bg-sidebar-accent",
                "data-open:hover:text-sidebar-accent-foreground",
                "gap-2",
                "rounded-md",
                "p-2",
                "text-left",
                "text-sm",
                "transition-[width,height,padding]",
                "group-has-data-[sidebar=menu-action]/menu-item:pr-8",
                "group-data-[collapsible=icon]:size-8",
                "group-data-[collapsible=icon]:p-2",
                "focus-visible:ring-2",
                "data-active:font-medium",
                "peer/menu-button",
                "flex",
                "w-full",
                "items-center",
                "overflow-hidden",
                "outline-hidden",
                "group/menu-button",
                "disabled:pointer-events-none",
                "disabled:opacity-50",
                "aria-disabled:pointer-events-none",
                "aria-disabled:opacity-50",
                "[&>span:last-child]:truncate",
                "[&_svg]:size-4",
                "[&_svg]:shrink-0"
            );

            var variantClasses = Variant == SidebarMenuButtonVariant.Outline
                ? CssClassUtils.Merge(
                    "bg-background",
                    "hover:bg-sidebar-accent",
                    "hover:text-sidebar-accent-foreground",
                    "shadow-[0_0_0_1px_hsl(var(--sidebar-border))]",
                    "hover:shadow-[0_0_0_1px_hsl(var(--sidebar-accent))]"
                )
                : CssClassUtils.Merge(
                    "hover:bg-sidebar-accent",
                    "hover:text-sidebar-accent-foreground"
                );

            var sizeClasses = Size switch
            {
                SidebarMenuButtonSize.Sm => "h-7 text-xs",
                SidebarMenuButtonSize.Lg => "h-12 text-sm group-data-[collapsible=icon]:p-0",
                _ => "h-8 text-sm"
            };

            return CssClassUtils.Merge(baseClasses, variantClasses, sizeClasses, Class);
        }
    }
}
