using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US04.ExpansionNavigation;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "03")]
[UserStory("04", "TreeView AutoExpand and SingleExpand Depth")]
public class SC03_TreeViewAutoExpand : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [new("a1x", "Alpha-1-deep", [])])]),
        new("b", "Beta", [new("b1", "Beta-1", [])])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        bool autoExpand = false,
        bool singleExpand = false,
        TreeViewSelectionMode selectionMode = TreeViewSelectionMode.Single,
        IReadOnlyList<string>? expandedKeys = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.AutoExpand, autoExpand)
            .Add(c => c.SingleExpand, singleExpand)
            .Add(c => c.SelectionMode, selectionMode)
            .Add(c => c.ExpandedKeys, expandedKeys));

    [Fact(DisplayName = "UAC01: AutoExpand expands ancestors when node is selected")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_AutoExpandExpandsAncestors()
    {
        var cut = RenderTree(autoExpand: true);
        // a1x is deeply nested and ancestors are collapsed
        cut.FindAll("[data-tree-node='a1x']").Count.ShouldBe(0);

        // Programmatically select a1x via click
        // First expand 'a' to reach 'a1'
        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();
        cut.Render();
        cut.Find("[data-tree-node='a1'] .lt-tree-node-toggle").Click();
        cut.Render();

        cut.FindAll("[data-tree-node='a1x']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC02: SingleExpand only collapses siblings at same depth")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SingleExpandRespectsDepth()
    {
        // Start with root 'a' expanded and its child 'a1' expanded
        var cut = RenderTree(singleExpand: true, expandedKeys: ["a", "a1"]);
        cut.FindAll("[data-tree-node='a1x']").Count.ShouldBe(1);

        // Expand root 'b' — should collapse sibling 'a' at depth 0
        cut.Find("[data-tree-node='b'] .lt-tree-node-toggle").Click();
        cut.Render();

        // 'a' should be collapsed, but 'b' expanded
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
        cut.FindAll("[data-tree-node='b']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC03: ExpandAll expands every branch")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ExpandAllExpandsEverything()
    {
        var cut = RenderTree();
        await cut.Instance.ExpandAll();
        cut.Render();

        cut.FindAll("[data-tree-node='a']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='a1x']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='b']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC04: CollapseAll collapses every branch")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_CollapseAllCollapsesEverything()
    {
        var cut = RenderTree(expandedKeys: ["a", "a1"]);
        await cut.Instance.CollapseAll();
        cut.Render();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
        cut.FindAll("[data-tree-node='a1x']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: ExpandedChanged fires with correct args")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ExpandedChangedFires()
    {
        var events = new List<TreeViewExpandEventArgs<Node>>();
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.ExpandedChanged, args => events.Add(args)));

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();
        cut.Render();

        events.Count.ShouldBe(1);
        events[0].Key.ShouldBe("a");
        events[0].Expanded.ShouldBeTrue();
    }
}
