using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US03.SelectionChecking;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "02")]
[UserStory("03", "TreeView Tri-State Checkbox Cascade")]
public class SC02_TreeViewTriState : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", []), new("a2", "Alpha-2", [])]),
        new("b", "Beta", [new("b1", "Beta-1", [])])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(IReadOnlyList<string>? checkedKeys = null, IReadOnlyList<string>? expandedKeys = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.CheckMode, TreeViewCheckMode.TriState)
            .Add(c => c.CheckedKeys, checkedKeys ?? [])
            .Add(c => c.ExpandedKeys, expandedKeys ?? ["a", "b"]));

    [Fact(DisplayName = "UAC01: checking parent checks all children")]
    [Trait(Spec.UAC, "01")]
    [Given("a TriState tree with no checked nodes")]
    [When("the checkbox for parent 'a' is clicked")]
    [Then("parent 'a', 'a1', and 'a2' all have the checked attribute")]
    public void UAC01_CheckParentChecksChildren()
    {
        var cut = RenderTree();
        var parentCheckbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        parentCheckbox.Change(true);
        cut.Render();

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").GetAttribute("checked").ShouldNotBeNull();
        cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox").GetAttribute("checked").ShouldNotBeNull();
        cut.Find("[data-tree-node='a2'] .lt-tree-node-checkbox").GetAttribute("checked").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: checking all children checks parent")]
    [Trait(Spec.UAC, "02")]
    [Given("a TriState tree with no checked nodes")]
    [When("each child of 'a' is checked individually")]
    [Then("parent 'a' is checked with data-indeterminate='false'")]
    public void UAC02_CheckAllChildrenChecksParent()
    {
        var cut = RenderTree();
        cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox").Change(true);
        cut.Render();
        cut.Find("[data-tree-node='a2'] .lt-tree-node-checkbox").Change(true);
        cut.Render();

        // Parent should be checked, not indeterminate
        var parentCheckbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        parentCheckbox.GetAttribute("checked").ShouldNotBeNull();
        parentCheckbox.GetAttribute("data-indeterminate").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC03: unchecking one child makes parent indeterminate")]
    [Trait(Spec.UAC, "03")]
    [Given("a TriState tree with parent 'a' and all its children checked")]
    [When("child 'a1' is unchecked")]
    [Then("parent 'a' loses the checked attribute and gets data-indeterminate='true'")]
    public void UAC03_UncheckOneChildMakesParentIndeterminate()
    {
        // Start with parent checked (all children checked)
        var cut = RenderTree(checkedKeys: ["a", "a1", "a2"]);
        cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox").Change(false);
        cut.Render();

        var parentCheckbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        parentCheckbox.GetAttribute("checked").ShouldBeNull();
        parentCheckbox.GetAttribute("data-indeterminate").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC04: unchecking parent unchecks all children")]
    [Trait(Spec.UAC, "04")]
    [Given("a TriState tree with parent 'a' and all its children checked")]
    [When("the checkbox for parent 'a' is unchecked")]
    [Then("parent 'a', 'a1', and 'a2' all lose the checked attribute")]
    public void UAC04_UncheckParentUnchecksChildren()
    {
        var cut = RenderTree(checkedKeys: ["a", "a1", "a2"]);
        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").Change(false);
        cut.Render();

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").GetAttribute("checked").ShouldBeNull();
        cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox").GetAttribute("checked").ShouldBeNull();
        cut.Find("[data-tree-node='a2'] .lt-tree-node-checkbox").GetAttribute("checked").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC05: CheckedKeysChanged fires with correct keys after cascade")]
    [Trait(Spec.UAC, "05")]
    [Given("a TriState tree with no checked nodes and a CheckedKeysChanged listener")]
    [When("the checkbox for parent 'a' is clicked")]
    [Then("CheckedKeysChanged fires once with 'a', 'a1', and 'a2' included")]
    public void UAC05_CheckedKeysChangedFiresAfterCascade()
    {
        var capturedKeys = new List<IReadOnlyList<string>>();
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.CheckMode, TreeViewCheckMode.TriState)
            .Add(c => c.CheckedKeys, [])
            .Add(c => c.ExpandedKeys, ["a", "b"])
            .Add(c => c.CheckedKeysChanged, keys => capturedKeys.Add(keys.ToList())));

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").Change(true);
        cut.Render();

        capturedKeys.Count.ShouldBe(1);
        capturedKeys[0].ShouldContain("a");
        capturedKeys[0].ShouldContain("a1");
        capturedKeys[0].ShouldContain("a2");
    }
}
