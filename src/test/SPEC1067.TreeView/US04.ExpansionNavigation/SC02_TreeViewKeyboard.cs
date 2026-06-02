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

    [Fact(DisplayName = "UAC15: ArrowDown on expanded node moves focus to first child")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ArrowDownIntoFirstChild()
    {
        var cut = RenderTree(expandedKeys: ["a"]);
        // Focus starts on 'a'; Down enters child 'a1'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        cut.Find("[data-tree-node='a1']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC16: ArrowDown on collapsed node moves focus to next sibling")]
    [Trait(Spec.UAC, "16")]
    [Given("a tree where node 'a' is collapsed")]
    [When("ArrowDown is pressed from node 'a'")]
    [Then("focus moves to the next visible root node 'b'")]
    public void UAC16_ArrowDownToNextSibling()
    {
        var cut = RenderTree(expandedKeys: []);
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC17: ArrowDown on leaf moves to next visible branch")]
    [Trait(Spec.UAC, "17")]
    [Given("a tree where node 'a' is expanded and focus is on leaf node 'a1'")]
    [When("ArrowDown is pressed")]
    [Then("focus moves to the next visible branch node 'b'")]
    public void UAC17_ArrowDownFromLeafMovesToNextVisibleBranch()
    {
        var cut = RenderTree(expandedKeys: ["a"]);
        // Move focus to 'a1'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();
        // ArrowDown from leaf continues to next visible branch.
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC18: ArrowUp on first child moves focus to parent")]
    [Trait(Spec.UAC, "18")]
    [Given("a tree where node 'a' is expanded and focus is on child node 'a1'")]
    [When("ArrowUp is pressed")]
    [Then("focus moves to the previous visible node, parent node 'a'")]
    public void UAC18_ArrowUpFromChildToParent()
    {
        var cut = RenderTree(expandedKeys: ["a"]);
        // Focus 'a1'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();
        // Up from first child returns to parent 'a'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowUp");
        cut.Render();

        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC19: ArrowUp moves focus to previous visible node")]
    [Trait(Spec.UAC, "19")]
    [Given("a tree where node 'a' is collapsed and focus is on root node 'b'")]
    [When("ArrowUp is pressed")]
    [Then("focus moves to previous visible root node 'a'")]
    public void UAC19_ArrowUpToPreviousVisibleNode()
    {
        var cut = RenderTree(expandedKeys: []);
        // Focus 'b'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();
        // Up goes back to 'a'
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowUp");
        cut.Render();

        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC20: ArrowUp on first root does nothing")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_ArrowUpOnFirstRootNoOp()
    {
        var cut = RenderTree(expandedKeys: []);
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowUp");
        cut.Render();

        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC21: ArrowLeft collapses expanded node")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_ArrowLeftCollapses()
    {
        var cut = RenderTree(expandedKeys: ["a"]);
        cut.Find("[data-slot='tree-view']").KeyDown("ArrowLeft");
        cut.Render();
        cut.Render();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC22: End moves focus to last visible node")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_EndMovesToLast()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();

        var last = cut.Find("[data-tree-node='b1']");
        last.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC23: Home moves focus to first visible node")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_HomeMovesToFirst()
    {
        var cut = RenderTree();
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();
        cut.Find("[data-slot='tree-view']").KeyDown("Home");
        cut.Render();

        var first = cut.Find("[data-tree-node='a']");
        first.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC24: Enter selects focused node")]
    [Trait(Spec.UAC, "24")]
    public void UAC24_EnterSelects()
    {
        var cut = RenderTree(TreeViewSelectionMode.Single);
        cut.Find("[data-slot='tree-view']").KeyDown("Enter");
        cut.Render();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC25: ArrowDown moves keyboard focus without clearing selected outline class")]
    [Trait(Spec.UAC, "25")]
    [Given("a tree in Single selection mode with node 'a' selected by click")]
    [When("ArrowDown moves keyboard focus to the next visible node")]
    [Then("node 'a' keeps the selected class and the focused node gets only the keyboard focus class")]
    public void UAC25_ArrowDownKeepsSelectedClassOnClickedNode()
    {
        var cut = RenderTree(TreeViewSelectionMode.Single, expandedKeys: ["a"]);
        cut.Find("[data-tree-node='a']").Click();
        cut.Render();

        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        var selected = cut.Find("[data-tree-node='a']");
        selected.ClassList.ShouldContain("lt-tree-node-selected");
        selected.ClassList.ShouldNotContain("lt-tree-node-keyboard-focused");

        var focused = cut.Find("[data-tree-node='a1']");
        focused.GetAttribute("tabindex").ShouldBe("0");
        focused.ClassList.ShouldContain("lt-tree-node-keyboard-focused");
        focused.ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC26: ArrowDown on last visible node does not wrap")]
    [Trait(Spec.UAC, "26")]
    [Given("a tree where focus is on the last visible node")]
    [When("ArrowDown is pressed")]
    [Then("focus remains on the last visible node")]
    public void UAC26_ArrowDownOnLastVisibleNodeDoesNotWrap()
    {
        var cut = RenderTree(expandedKeys: []);
        cut.Find("[data-slot='tree-view']").KeyDown("End");
        cut.Render();

        cut.Find("[data-slot='tree-view']").KeyDown("ArrowDown");
        cut.Render();

        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }
}
