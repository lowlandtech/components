using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US01.Structure;

/// <summary>
/// SC00 — Structural contract for the reusable <see cref="TreeView{TItem}"/>
/// component. Locks the depth-aware rendering, recursion, and expansion
/// semantics that callers depend on (CanvasPage, DashboardPage, etc.).
/// </summary>
[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "00")]
[UserStory("01", "TreeView structural contract")]
public class SC00_TreeViewStructuralContract : JsViewScenario
{
    private record Node(string Id, string? ParentId, string Name);

    private static readonly Node[] _flat =
    [
        new("a", null, "Alpha"),
        new("a.1", "a", "Alpha-1"),
        new("a.1.x", "a.1", "Alpha-1-deep"),
        new("a.2", "a", "Alpha-2"),
        new("b", null, "Beta"),
    ];

    private static IEnumerable<Node> Roots => _flat.Where(n => n.ParentId is null);
    private static IEnumerable<Node> ChildrenOf(Node n) => _flat.Where(x => x.ParentId == n.Id);

    private IRenderedComponent<TreeView<Node>> RenderTree(Func<Node, bool>? isExpanded = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => ChildrenOf(n))
            .Add(c => c.IsExpanded, isExpanded ?? (_ => false))
            .Add(c => c.ItemTemplate, ctx => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "data-id", ctx.Item.Id);
                builder.AddAttribute(2, "data-depth", ctx.Depth.ToString());
                // ToString() avoids Blazor's bool-attribute behavior (false strips, true
                // emits the bare attribute with no value). We need the literal string
                // for the data-* round-trip assertion below.
                builder.AddAttribute(3, "data-has-children", ctx.HasChildren.ToString());
                builder.AddAttribute(4, "class", $"tree-node depth-{ctx.Depth}");
                builder.AddContent(5, ctx.Item.Name);
                builder.CloseElement();
            }));

    [Fact(DisplayName = "UAC01: TreeView root element has data-slot='tree-view'")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: collapsed root renders only top-level items")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CollapsedRendersOnlyRoots()
    {
        var cut = RenderTree();
        var items = cut.FindAll("[data-id]");
        items.Select(e => e.GetAttribute("data-id")).ShouldBe(["a", "b"]);
    }

    [Fact(DisplayName = "UAC03: expanding the root reveals direct children at depth 1")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ExpandedShowsChildren()
    {
        var cut = RenderTree(n => n.Id == "a");
        var items = cut.FindAll("[data-id]");
        items.Select(e => e.GetAttribute("data-id")).ShouldBe(["a", "a.1", "a.2", "b"]);
    }

    [Fact(DisplayName = "UAC04: depth attribute reflects nesting level (0, 1, 2)")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DepthAttributeMatchesNesting()
    {
        var cut = RenderTree(n => n.Id == "a" || n.Id == "a.1");
        var depthByItem = cut.FindAll("[data-id]")
            .ToDictionary(e => e.GetAttribute("data-id")!, e => e.GetAttribute("data-depth"));

        depthByItem["a"].ShouldBe("0");
        depthByItem["a.1"].ShouldBe("1");
        depthByItem["a.1.x"].ShouldBe("2");
        depthByItem["a.2"].ShouldBe("1");
        depthByItem["b"].ShouldBe("0");
    }

    [Fact(DisplayName = "UAC05: HasChildren context flag is true for parents, false for leaves")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_HasChildrenFlag()
    {
        var cut = RenderTree(n => n.Id == "a" || n.Id == "a.1");
        var hasChildren = cut.FindAll("[data-id]")
            .ToDictionary(e => e.GetAttribute("data-id")!, e => e.GetAttribute("data-has-children"));

        hasChildren["a"].ShouldBe("True");
        hasChildren["a.1"].ShouldBe("True");
        hasChildren["a.1.x"].ShouldBe("False");
        hasChildren["a.2"].ShouldBe("False");
        hasChildren["b"].ShouldBe("False");
    }

    [Fact(DisplayName = "UAC06: collapsed parent does not render its descendants")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_CollapsedParentHidesDescendants()
    {
        // a expanded, a.1 collapsed → a.1.x must NOT render.
        var cut = RenderTree(n => n.Id == "a");
        cut.FindAll("[data-id='a.1.x']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC07: empty Items renders an empty tree-view container")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_EmptyItemsRenders()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Array.Empty<Node>())
            .Add(c => c.GetChildren, _ => [])
            .Add(c => c.ItemTemplate, ctx => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "data-id", ctx.Item.Id);
                builder.CloseElement();
            }));

        cut.Find("[data-slot='tree-view']").ShouldNotBeNull();
        cut.FindAll("[data-id]").Count.ShouldBe(0);
    }
}
