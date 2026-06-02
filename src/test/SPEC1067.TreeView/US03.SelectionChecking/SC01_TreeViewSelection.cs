using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US03.SelectionChecking;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "TreeView Selection and Checkboxes")]
public class SC01_TreeViewSelection : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", []), new("a2", "Alpha-2", [])]),
        new("b", "Beta", [])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        TreeViewSelectionMode mode = TreeViewSelectionMode.Single,
        IReadOnlyList<string>? selectedKeys = null,
        TreeViewCheckMode checkMode = TreeViewCheckMode.None,
        IReadOnlyList<string>? checkedKeys = null,
        IReadOnlyList<string>? expandedKeys = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, mode)
            .Add(c => c.SelectedKeys, selectedKeys)
            .Add(c => c.CheckMode, checkMode)
            .Add(c => c.CheckedKeys, checkedKeys)
            .Add(c => c.ExpandedKeys, expandedKeys));

    [Fact(DisplayName = "UAC01: single selection selects one node")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SingleSelect()
    {
        var cut = RenderTree(TreeViewSelectionMode.Single);
        cut.Find("[data-tree-node='a']").Click();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC02: single selection replaces previous selection")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SingleSelectReplaces()
    {
        var cut = RenderTree(TreeViewSelectionMode.Single);
        cut.Find("[data-tree-node='a']").Click();
        cut.Render();
        cut.Find("[data-tree-node='b']").Click();
        cut.Render();

        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC05: toggle selection clears on second click")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ToggleSelectClears()
    {
        var cut = RenderTree(TreeViewSelectionMode.Toggle);
        cut.Find("[data-tree-node='a']").Click();
        cut.Find("[data-tree-node='a']").Click();

        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC08: multi selection preserves existing selections")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_MultiSelectPreserves()
    {
        var cut = RenderTree(TreeViewSelectionMode.Multi);
        cut.Find("[data-tree-node='a']").Click();
        cut.Find("[data-tree-node='b']").Click();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC11: checkbox checks only the clicked node")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_CheckboxChecksOnlyClickedNode()
    {
        var cut = RenderTree(checkMode: TreeViewCheckMode.Checkbox, checkedKeys: [], expandedKeys: ["a"]);
        var checkbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        checkbox.Change(true);
        cut.Render();

        // In checkbox mode, only the clicked node is checked (no cascade)
        var aCheckbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        aCheckbox.GetAttribute("checked").ShouldNotBeNull();
        var a1Checkbox = cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox");
        a1Checkbox.GetAttribute("checked").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC14: tri-state parent becomes indeterminate when child checked")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_TriStateChildCheck()
    {
        var cut = RenderTree(checkMode: TreeViewCheckMode.TriState, checkedKeys: [], expandedKeys: ["a"]);
        var checkbox = cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox");
        checkbox.Change(true);
        cut.Render();

        // Child should be checked
        var a1Checkbox = cut.Find("[data-tree-node='a1'] .lt-tree-node-checkbox");
        a1Checkbox.GetAttribute("checked").ShouldNotBeNull();

        // Parent should become indeterminate
        var parentCheckbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        parentCheckbox.GetAttribute("data-indeterminate").ShouldBe("true");
    }
}
