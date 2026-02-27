namespace LowlandTech.Components.Sidebar;

public class SidebarState
{
    public bool Open { get; set; } = true;
    public bool OpenMobile { get; set; }
    public bool IsMobile { get; set; }
    public string Side { get; set; } = "left";
    public string Variant { get; set; } = "sidebar";
    public string Collapsible { get; set; } = "offcanvas";

    public string State => Open ? "expanded" : "collapsed";

    public event Action? OnChange;

    public void Toggle()
    {
        if (IsMobile)
            OpenMobile = !OpenMobile;
        else
            Open = !Open;
        NotifyChanged();
    }

    public void SetOpen(bool value)
    {
        Open = value;
        NotifyChanged();
    }

    public void SetOpenMobile(bool value)
    {
        OpenMobile = value;
        NotifyChanged();
    }

    public void SetIsMobile(bool value)
    {
        IsMobile = value;
        NotifyChanged();
    }

    private void NotifyChanged() => OnChange?.Invoke();
}
