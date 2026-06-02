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

    [Fact(DisplayName = "UAC15: ArrowDown moves focus to next sibling")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ArrowDownMovesToNextSibling()
    {
        var cut = RenderTree(expandedKeys: []);
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC16: ArrowDown on last sibling wraps to first sibling")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_ArrowDownWrapsToFirstSibling()
    {
        var cut = RenderTree(expandedKeys: []);
        // Move focus to 'b' (last root)
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();
        // ArrowDown again wraps to 'a'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC17: ArrowUp moves focus to previous sibling")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_ArrowUpMovesToPreviousSibling()
    {
        var cut = RenderTree(expandedKeys: []);
        // Move focus to 'b'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();
        // ArrowUp goes back to 'a'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowUp");
        cut.Render();

        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC18: ArrowUp on first sibling wraps to last sibling")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_ArrowUpWrapsToLastSibling()
    {
        var cut = RenderTree(expandedKeys: []);
        // ArrowUp on 'a' wraps to 'b'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowUp");
        cut.Render();

        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC19: ArrowLeft collapses expanded node")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_ArrowLeftCollapses()
    {
        var cut = RenderTree(expandedKeys: ["a"]);
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowLeft");
        cut.Render();
        cut.Render();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC20: End moves focus to last visible node")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_EndMovesToLast()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();

        var last = cut.Find("[data-tree-node='b1']");
        last.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC21: Home moves focus to first visible node")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_HomeMovesToFirst()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();
        cut.Find("[data-slot='tree-view']").KeyDown("Home");
        cut.Render();

        var first = cut.Find("[data-tree-node='a']");
        first.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC22: Enter selects focused node")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_EnterSelects()
    {
        var cut = RenderTree(TreeViewSelectionMode.Single);
        cut.Find("[data-slot='tree-view']").KeyDown("Enter");
        cut.Render();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
    }
}
