using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US04.ExpansionNavigation;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "TreeView Expansion and Keyboard Navigation")]
public class SC01_TreeViewExpansion : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [new("a1x", "Alpha-1-deep", [])]), new("a2", "Alpha-2", [])]),
        new("b", "Beta", [])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        bool singleExpand = false,
        bool autoExpand = false,
        IReadOnlyList<string>? expandedKeys = null,
        TreeViewSelectionMode selectionMode = TreeViewSelectionMode.None) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SingleExpand, singleExpand)
            .Add(c => c.AutoExpand, autoExpand)
            .Add(c => c.ExpandedKeys, expandedKeys)
            .Add(c => c.SelectionMode, selectionMode));

    [Fact(DisplayName = "UAC01: toggle button expands branch")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ToggleExpands()
    {
        var cut = RenderTree();
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC05: toggle again collapses branch")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ToggleCollapses()
    {
        var cut = RenderTree(expandedKeys: ["a"]);
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: single expand collapses siblings")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_SingleExpandCollapsesSiblings()
    {
        // Add another root with children to test sibling collapse
        var roots = new[]
        {
            new Node("a", "Alpha", [new Node("a1", "A1", [])]),
            new Node("b", "Beta", [new Node("b1", "B1", [])])
        };
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SingleExpand, true)
            .Add(c => c.ExpandedKeys, ["a"]));

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);

        cut.Find("[data-tree-node='b'] .lt-tree-node-toggle").Click();
        cut.Render();

        cut.FindAll("[data-tree-node='b1']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC23: ExpandAll expands every branch")]
    [Trait(Spec.UAC, "23")]
    public async Task UAC23_ExpandAll()
    {
        var cut = RenderTree();
        await cut.Instance.ExpandAll();
        cut.Render();

        cut.FindAll("[data-tree-node='a1x']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC25: CollapseAll collapses every branch")]
    [Trait(Spec.UAC, "25")]
    public async Task UAC25_CollapseAll()
    {
        var cut = RenderTree(expandedKeys: ["a", "a1"]);
        await cut.Instance.CollapseAll();
        cut.Render();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }
}
