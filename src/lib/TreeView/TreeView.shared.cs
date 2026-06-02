namespace LowlandTech.Components;

/// <summary>
/// Design-time representative for the generic <see cref="TreeView{TItem}"/> component.
///
/// <see cref="TreeView{TItem}"/> is an open generic and cannot be directly discovered
/// by the <see cref="ComponentDescriptorRegistry"/> (open generics are not assignable
/// to <see cref="TinyComponentBase"/> at the reflection level). This sealed concrete
/// proxy extends <c>TreeView&lt;string&gt;</c> so the registry picks it up, inheriting
/// all <c>[Parameter, PropertyGrid]</c> decorations from the generic base class.
///
/// The story passes <c>typeof(TreeViewDesigner)</c> as <c>DesignComponentType</c> so
/// the StoryCard "Design mode" toggle resolves a descriptor and shows the Properties
/// panel. At canvas drop time the element name resolves back to <c>"TreeView"</c>
/// via the <see cref="PropertiesAttribute.Element"/> override. (SPEC1067 design-time)
/// </summary>
[Properties(Group = "Navigation", Element = "TreeView", Icon = "list-tree",
    Description = "Hierarchical tree with expandable nodes, selection, and checkbox modes")]
public sealed class TreeViewDesigner : TreeView<string>
{
}
