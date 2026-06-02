namespace LowlandTech.Components;

/// <summary>
/// A designable label row for use inside <see cref="TreeView{TItem}.ItemTemplate"/>.
///
/// In design mode, clicking a <c>TreeViewItem</c> selects the item independently
/// from the parent <see cref="TreeView{TItem}"/>, allowing the Properties panel to
/// show and edit per-node appearance (icon, suffix, class). The click is stopped
/// from propagating to the tree root so only one component is selected at a time.
/// </summary>
[Properties(Group = "Navigation", Icon = "list",
    Description = "A designable label row for use inside TreeView item templates")]
public partial class TreeViewItem : TinyComponentBase
{
    /// <summary>Icon name rendered to the left of the label.</summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Text, Placeholder = "e.g. file")]
    public string? Icon { get; set; }

    /// <summary>Short text rendered to the right of the label (e.g. a count badge).</summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Text, Placeholder = "e.g. 42")]
    public string? Suffix { get; set; }
}
