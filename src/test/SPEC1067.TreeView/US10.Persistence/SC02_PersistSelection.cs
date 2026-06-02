namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US10.Persistence;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "02")]
[UserStory("10", "TreeView PersistKey selection persistence via localStorage")]
public class SC02_PersistSelection : JsViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta",  [])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        string? persistKey = null,
        bool noPersist = false,
        TreeViewSelectionMode selectionMode = TreeViewSelectionMode.Single,
        IReadOnlyList<string>? selectedKeys = null,
        EventCallback<IReadOnlyList<string>>? selectedKeysChanged = null) =>
        Ctx.Render<TreeView<Node>>(p =>
        {
            p.Add(c => c.Items, Roots)
             .Add(c => c.GetChildren, n => n.Children)
             .Add(c => c.GetKey, n => n.Id)
             .Add(c => c.HasChildren, n => n.Children.Count > 0)
             .Add(c => c.SelectionMode, selectionMode)
             .Add(c => c.PersistKey, persistKey)
             .Add(c => c.NoPersist, noPersist);
            if (selectedKeys != null)
                p.Add(c => c.SelectedKeys, selectedKeys);
            if (selectedKeysChanged.HasValue)
                p.Add(c => c.SelectedKeysChanged, selectedKeysChanged.Value);
        });

    [Fact(DisplayName = "UAC01: selecting a node calls localStorage.setItem with 'treeview:{key}:selected'")]
    [Trait(Spec.UAC, "01")]
    [Given("a tree rendered with PersistKey='test-tree' and Single selection mode")]
    [When("node 'a' is clicked")]
    [Then("localStorage.setItem is called with key 'treeview:test-tree:selected' containing '\"a\"'")]
    public void UAC01_SelectionWritesToLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree");

        cut.Find("[data-tree-node='a']").Click();

        var setItemCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .ToList();

        setItemCalls.ShouldNotBeEmpty();
        var call = setItemCalls.First(i => i.Arguments[0]?.ToString()?.Contains(":selected") == true);
        call.Arguments[0].ShouldBe("treeview:test-tree:selected");
        var json = call.Arguments[1]?.ToString();
        json.ShouldNotBeNullOrEmpty();
        json.ShouldContain("\"a\"");
    }

    [Fact(DisplayName = "UAC02: deselecting a node removes it from the persisted selection")]
    [Trait(Spec.UAC, "02")]
    [Given("a tree with PersistKey='test-tree' in Toggle mode")]
    [When("node 'a' is clicked twice")]
    [Then("the last localStorage.setItem call for ':selected' contains an empty array")]
    public void UAC02_DeselectionUpdatesLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree", selectionMode: TreeViewSelectionMode.Toggle);

        cut.Find("[data-tree-node='a']").Click();
        cut.Find("[data-tree-node='a']").Click();

        var selectedCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem"
                     && i.Arguments[0]?.ToString()?.EndsWith(":selected") == true)
            .ToList();

        selectedCalls.ShouldNotBeEmpty();
        var lastSelectedCall = selectedCalls.Last();

        var keys = JsonSerializer.Deserialize<List<string>>(lastSelectedCall.Arguments[1]?.ToString()!);
        keys.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC03: no selected-key localStorage calls when NoPersist is set")]
    [Trait(Spec.UAC, "03")]
    [Given("a tree rendered with NoPersist=true in Single selection mode")]
    [When("node 'a' is clicked")]
    [Then("no localStorage.setItem call is made for a ':selected' key")]
    public void UAC03_NoPersistSkipsSelectedStorage()
    {
        var cut = RenderTree(noPersist: true);

        cut.Find("[data-tree-node='a']").Click();

        var selectedCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem"
                     && i.Arguments[0]?.ToString()?.EndsWith(":selected") == true)
            .ToList();

        selectedCalls.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC04: controlled SelectedKeys parameter is not overwritten by PersistKey on first render")]
    [Trait(Spec.UAC, "04")]
    [Given("a tree with both PersistKey and a controlled SelectedKeys=['b'] parameter")]
    [When("the component renders for the first time")]
    [Then("node 'b' has the 'lt-tree-node-selected' class and node 'a' does not")]
    public void UAC04_ControlledSelectedKeysNotOverridden()
    {
        var cut = RenderTree(persistKey: "test-tree", selectedKeys: ["b"]);

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC05: selected state is restored from localStorage on first render")]
    [Trait(Spec.UAC, "05")]
    [Given("localStorage contains '[\"a\"]' under 'treeview:test-tree:selected'")]
    [When("the tree renders for the first time with PersistKey='test-tree' and no controlled SelectedKeys")]
    [Then("node 'a' has the 'lt-tree-node-selected' class after the async restore")]
    public async Task UAC05_SelectionRehydratesFromLocalStorage()
    {
        Ctx.JSInterop.SetupModule("localStorage.getItem")
            .SetupVoid();

        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:selected")
            .SetResult("[\"a\"]");
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult(null);

        var cut = RenderTree(persistKey: "test-tree");

        // Trigger the async OnAfterRenderAsync by waiting
        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldNotContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC06: controlled SelectedKeys rehydrate when the same list instance changes")]
    [Trait(Spec.UAC, "06")]
    [Given("a tree bound to a reusable SelectedKeys list containing 'a'")]
    [When("the host rehydrates the same list instance with only 'b' and rerenders")]
    [Then("node 'b' is selected and node 'a' is not selected")]
    public void UAC06_ControlledSelectedKeysRehydrateWhenSameListChanges()
    {
        var selectedKeys = new List<string> { "a" };
        var cut = RenderTree(persistKey: "test-tree", selectedKeys: selectedKeys);

        selectedKeys.Clear();
        selectedKeys.Add("b");
        cut.Render(p => p.Add(c => c.SelectedKeys, selectedKeys));

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC07: controlled empty SelectedKeys restore from localStorage on reload")]
    [Trait(Spec.UAC, "07")]
    [Given("localStorage contains '[\"b\"]' and the host passes an empty controlled SelectedKeys list")]
    [When("the tree renders for the first time with PersistKey='test-tree'")]
    [Then("node 'b' is selected and SelectedKeysChanged is fired with 'b'")]
    public async Task UAC07_ControlledEmptySelectedKeysRestoreFromLocalStorage()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult(null);
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:selected")
            .SetResult("[\"b\"]");

        var captured = new List<IReadOnlyList<string>>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys => captured.Add(keys));
        var cut = RenderTree(persistKey: "test-tree", selectedKeys: [], selectedKeysChanged: cb);

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
        captured.Count.ShouldBe(1);
        captured[0].ShouldContain("b");
    }

    [Fact(DisplayName = "UAC08: persisted SelectedKeys override controlled defaults on reload")]
    [Trait(Spec.UAC, "08")]
    [Given("localStorage contains '[\"b\"]' and the host passes controlled SelectedKeys=['a']")]
    [When("the tree renders for the first time with PersistKey='test-tree'")]
    [Then("node 'b' is selected and node 'a' is not selected")]
    public async Task UAC08_PersistedSelectedKeysOverrideControlledDefaults()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult(null);
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:selected")
            .SetResult("[\"b\"]");

        var captured = new List<IReadOnlyList<string>>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys => captured.Add(keys));
        var cut = RenderTree(persistKey: "test-tree", selectedKeys: ["a"], selectedKeysChanged: cb);

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
        captured.Count.ShouldBe(1);
        captured[0].ShouldContain("b");
    }

    [Fact(DisplayName = "UAC09: persisted SelectedKeys hydrate before selection mode is enabled")]
    [Trait(Spec.UAC, "09")]
    [Given("localStorage contains '[\"b\"]' and the first render has SelectionMode=None")]
    [When("the host rerenders with SelectionMode=Single")]
    [Then("node 'b' is selected from the persisted selected-key state")]
    public async Task UAC09_PersistedSelectedKeysHydrateBeforeSelectionModeIsEnabled()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult(null);
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:selected")
            .SetResult("[\"b\"]");

        var selectedKeys = new List<string>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys =>
        {
            selectedKeys.Clear();
            selectedKeys.AddRange(keys);
        });
        var cut = RenderTree(
            persistKey: "test-tree",
            selectionMode: TreeViewSelectionMode.None,
            selectedKeys: selectedKeys,
            selectedKeysChanged: cb);

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render(p => p
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKeys, selectedKeys));

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC10: controlled SelectedKey hydrates focus to selected node")]
    [Trait(Spec.UAC, "10")]
    [Given("a tree rendered with controlled SelectedKey='b'")]
    [When("the component hydrates controlled selection state")]
    [Then("node 'b' is selected and receives roving tabindex focus")]
    public void UAC10_ControlledSelectedKeyHydratesFocus()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.SelectionMode, TreeViewSelectionMode.Single)
            .Add(c => c.SelectedKey, "b"));

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").GetAttribute("tabindex").ShouldBe("0");
        cut.Find("[data-tree-node='a']").GetAttribute("tabindex").ShouldBe("-1");
    }
}
