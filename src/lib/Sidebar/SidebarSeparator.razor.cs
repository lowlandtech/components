namespace LowlandTech.Components.Sidebar;

public partial class SidebarSeparator
{
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
