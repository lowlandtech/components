namespace LowlandTech.Components;

// This file extends the auto-generated Sidebar partial class.
// Add custom parameters, methods, and lifecycle overrides here.
public partial class Sidebar
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }
    [Parameter] public string Side { get; set; } = "left";
    [Parameter] public string Variant { get; set; } = "sidebar";
    [Parameter] public string Collapsible { get; set; } = "offcanvas";
}
