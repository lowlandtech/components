using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US05.FilteringSearch;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "01")]
[UserStory("05", "TreeView Filtering and Search")]
public class SC01_TreeViewFiltering : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", []), new("a2", "Alpha-two", [])]),
        new("b", "Beta", [new("b1", "Beta-1", [])])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(string? filterQuery = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.GetText, n => n.Name)
            .Add(c => c.FilterQuery, filterQuery)
            .Add(c => c.ExpandedKeys, ["a", "b"]));

    [Fact(DisplayName = "UAC01: MatchesFilter returns true when text contains query")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MatchesFilter()
    {
        var cut = RenderTree();
        cut.Instance.MatchesFilter(Roots[0]).ShouldBeTrue(); // no filter

        var filtered = RenderTree("Alpha");
        filtered.Instance.MatchesFilter(Roots[0]).ShouldBeTrue();
        filtered.Instance.MatchesFilter(Roots[1]).ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: MatchesFilter is case-insensitive")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_MatchesFilterCaseInsensitive()
    {
        var cut = RenderTree("alpha");
        cut.Instance.MatchesFilter(Roots[0]).ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: IsVisibleInFilter shows nodes matching query")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_IsVisibleInFilter()
    {
        var cut = RenderTree("Alpha-1");
        // Root 'a' should be visible because it has a matching descendant
        cut.Instance.IsVisibleInFilter(Roots[0]).ShouldBeTrue();
        // Root 'b' has no matching descendants
        cut.Instance.IsVisibleInFilter(Roots[1]).ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: Filtered tree renders only matching nodes")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_FilteredTreeRendering()
    {
        var cut = RenderTree("Alpha-1");
        cut.Render();

        // 'a' should render (ancestor of match)
        cut.FindAll("[data-tree-node='a']").Count.ShouldBe(1);
        // 'a1' should render (matches filter)
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
        // 'a2' does not match and is not ancestor of match
        cut.FindAll("[data-tree-node='a2']").Count.ShouldBe(0);
        // 'b' does not match and has no matching descendants
        cut.FindAll("[data-tree-node='b']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: GetText is used for display text")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_GetTextForDisplay()
    {
        var cut = RenderTree();
        var label = cut.Find("[data-tree-node='a'] .lt-tree-node-label");
        label.TextContent.ShouldBe("Alpha");
    }

    [Fact(DisplayName = "UAC06: Empty filter shows all nodes")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_EmptyFilterShowsAll()
    {
        var cut = RenderTree("");
        cut.Render();

        cut.FindAll("[data-tree-node='a']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='b']").Count.ShouldBe(1);
    }
}
