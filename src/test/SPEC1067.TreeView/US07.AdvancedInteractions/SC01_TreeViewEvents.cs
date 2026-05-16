using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US07.AdvancedInteractions;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "01")]
[UserStory("07", "TreeView Event Callbacks and Programmatic Control")]
public class SC01_TreeViewEvents : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta", [])
    ];

    [Fact(DisplayName = "UAC01: SelectedChanged fires with correct args on select")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SelectedChangedFires()
    {
        var events = new List<TreeViewSelectEventArgs<Node>>();
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedChanged, args => events.Add(args)));

        cut.Find("[data-tree-node='a']").Click();
        cut.Render();

        events.Count.ShouldBe(1);
        events[0].Key.ShouldBe("a");
        events[0].Selected.ShouldBeTrue();
        events[0].Item.Id.ShouldBe("a");
    }

    [Fact(DisplayName = "UAC02: SelectedKey two-way binding works")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SelectedKeyBinding()
    {
        var selectedKey = "a";
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKey, selectedKey)
            .Add(c => c.SelectedKeyChanged, key => selectedKey = key!));

        cut.Find("[data-tree-node='b']").Click();
        cut.Render();

        selectedKey.ShouldBe("b");
    }

    [Fact(DisplayName = "UAC03: SelectedKeys two-way binding updates on select")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectedKeysBinding()
    {
        var keys = new List<string> { "a" };
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKeys, keys)
            .Add(c => c.SelectedKeysChanged, newKeys => keys = newKeys.ToList()));

        cut.Find("[data-tree-node='b']").Click();
        cut.Render();

        keys.Count.ShouldBe(1);
        keys[0].ShouldBe("b");
    }

    [Fact(DisplayName = "UAC04: Programmatic SelectedKey sets initial selection")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ProgrammaticSelectedKey()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKey, "a"));

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC05: SelectedChanged does not fire on re-selecting same node in single mode")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NoOpSelectDoesNotFireEvent()
    {
        var events = new List<TreeViewSelectEventArgs<Node>>();
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKeys, ["a"])
            .Add(c => c.SelectedChanged, args => events.Add(args)));

        // Click already-selected node
        cut.Find("[data-tree-node='a']").Click();
        cut.Render();

        events.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: Null GetChildren does not throw")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NullGetChildrenHandled()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => null!) // Returns null
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0));

        // Should render without throwing
        cut.FindAll("[data-tree-node]").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC07: Null SelectedKeys parameter results in no selection")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NullSelectedKeysNoSelection()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKeys, (IReadOnlyList<string>?)null));

        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC08: Null CheckedKeys parameter results in no checks")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_NullCheckedKeysNoChecks()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.CheckMode, TreeViewCheckMode.Checkbox)
            .Add(c => c.CheckedKeys, (IReadOnlyList<string>?)null)
            .Add(c => c.ExpandedKeys, ["a"]));

        var checkbox = cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox");
        checkbox.GetAttribute("checked").ShouldBeNull();
    }
}
