using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US04.ExpansionNavigation;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "02")]
[UserStory("04", "TreeView Keyboard Navigation")]
public class SC02_TreeViewKeyboard : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta", [new("b1", "Beta-1", [])])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        TreeViewSelectionMode selectionMode = TreeViewSelectionMode.Single,
        IReadOnlyList<string>? expandedKeys = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, selectionMode)
            .Add(c => c.ExpandedKeys, expandedKeys ?? ["a", "b"]));

    [Fact(DisplayName = "UAC14: ArrowRight expands collapsed branch")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ArrowRightExpands()
    {
        var cut = RenderTree(expandedKeys: []);
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowRight");
        cut.Render();
        cut.Render();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC16: ArrowDown moves focus to next visible node")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_ArrowDownMovesFocus()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        var a1 = cut.Find("[data-tree-node='a1']");
        a1.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC17: ArrowLeft on child moves focus to parent")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_ArrowLeftToParent()
    {
        var cut = RenderTree();
        // Focus a1 first
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();
        // Arrow left should move to parent 'a'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowLeft");
        cut.Render();

        var a = cut.Find("[data-tree-node='a']");
        a.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC19: End moves focus to last visible node")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_EndMovesToLast()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();

        var last = cut.Find("[data-tree-node='b1']");
        last.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC20: Home moves focus to first visible node")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_HomeMovesToFirst()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();
        cut.Find("[data-slot='tree-view']").KeyDown("Home");
        cut.Render();

        var first = cut.Find("[data-tree-node='a']");
        first.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC21: Enter selects focused node")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_EnterSelects()
    {
        var cut = RenderTree(TreeViewSelectionMode.Single);
        cut.Find("[data-slot='tree-view']").KeyDown("Enter");
        cut.Render();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
    }
}
