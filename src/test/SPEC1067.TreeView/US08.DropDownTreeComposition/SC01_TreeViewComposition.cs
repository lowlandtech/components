using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US08.DropDownTreeComposition;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "01")]
[UserStory("08", "TreeView Dropdown Composition")]
public class SC01_TreeViewComposition : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta", [])
    ];

    [Fact(DisplayName = "UAC01: Multiple TreeViews on same page render independently")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MultipleInstancesIndependent()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id));

        var cut2 = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id));

        cut.FindAll("[data-tree-node]").Count.ShouldBe(2);
        cut2.FindAll("[data-tree-node]").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC02: SelectedKey binding updates DOM selection")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SelectedKeyBinding()
    {
        string? selectedKey = null;
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.SelectedKey, selectedKey)
            .Add(c => c.SelectedKeyChanged, (string? k) => selectedKey = k)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single));

        cut.FindAll("[data-tree-node]").First().Click();

        selectedKey.ShouldBe("a");
    }
}
