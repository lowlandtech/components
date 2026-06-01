namespace LowlandTech.Components.Tabs;

[Properties(Group = "Navigation", Icon = "tabs", Description = "Tabbed content with collapsible and dynamic variants")]
public partial class Tabs
{
    [Parameter, PropertyGrid(Group = "Layout", Editor = PropertyEditor.Select,
        Options = new[] { "horizontal", "vertical" })]
    public string Orientation { get; set; } = "horizontal";

    [Parameter, PropertyGrid(Group = "Layout", Editor = PropertyEditor.Select,
        Options = new[] { "top", "bottom", "left", "right" })]
    public string Placement { get; set; } = "top";

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Text)]
    public string DefaultValue { get; set; } = "";

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Text)]
    public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Select,
        Options = new[] { "automatic", "manual" })]
    public string KeyboardActivation { get; set; } = "automatic";

    [Parameter] public IReadOnlyList<TabsItem>? Items { get; set; }

    [Parameter] public EventCallback<IReadOnlyList<TabsItem>> ItemsChanged { get; set; }

    [Parameter] public RenderFragment? HeaderLeading { get; set; }

    [Parameter] public RenderFragment<TabsItem>? ItemContent { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "line", "ide" })]
    public string DynamicVariant { get; set; } = "ide";

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "left", "center", "right" })]
    public string DynamicListAlign { get; set; } = "left";

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool DynamicListStretch { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool ShowScrollButtons { get; set; } = true;

    [Parameter] public EventCallback<TabsItem> TabClosed { get; set; }

    [Parameter] public EventCallback<TabsItem> TabPinnedChanged { get; set; }

    [Parameter] public EventCallback<TabsRenameEventArgs> TabRenamed { get; set; }

    [Parameter] public EventCallback<TabsReorderEventArgs> TabReordered { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Text)]
    public string? ContentClass { get; set; }

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Select,
        Options = new[] { "startclosed", "startopen" }, EmptyOption = "")]
    public string? IsCollapsable { get; set; }

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Switch)]
    public bool? InitialIsContentCollapsed { get; set; }

    [Parameter] public EventCallback<bool> IsContentCollapsedChanged { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool HasGap { get; set; } = true;

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool GapOutline { get; set; }
}
