namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US03.SelectionChecking;

/// <summary>
/// Guards the DOM attributes that drive checkbox CSS visual states.
/// Prevents regression of the hsl(var()) CSS fix and the @onchange event-handler
/// pattern for controlled checkboxes.
///
/// The CSS selectors that depend on these exact attribute values:
///   .lt-tree-node-checkbox:checked          — driven by the HTML checked attribute
///   .lt-tree-node-checkbox[data-indeterminate="true"]  — driven by data-indeterminate="true"
/// </summary>
[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "03")]
[UserStory("03", "TreeView checkbox DOM attribute contract")]
public class SC03_CheckboxVisualAttributes : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", []), new("a2", "Alpha-2", [])]),
        new("b", "Beta",  [new("b1", "Beta-1",  [])])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        TreeViewCheckMode checkMode,
        IReadOnlyList<string>? checkedKeys = null,
        IReadOnlyList<string>? expandedKeys = null,
        EventCallback<IReadOnlyList<string>>? checkedKeysChanged = null)
    {
        return Ctx.Render<TreeView<Node>>(p =>
        {
            p.Add(c => c.Items, Roots)
             .Add(c => c.GetChildren, n => n.Children)
             .Add(c => c.GetKey, n => n.Id)
             .Add(c => c.HasChildren, n => n.Children.Count > 0)
             .Add(c => c.CheckMode, checkMode)
             .Add(c => c.CheckedKeys, checkedKeys ?? [])
             .Add(c => c.ExpandedKeys, expandedKeys ?? ["a", "b"]);
            if (checkedKeysChanged.HasValue)
                p.Add(c => c.CheckedKeysChanged, checkedKeysChanged.Value);
        });
    }

    // -------------------------------------------------------------------------
    // checked attribute — drives :checked CSS selector
    // -------------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: checkbox renders with checked attribute present when key is in CheckedKeys")]
    [Trait(Spec.UAC, "01")]
    [Given("a tree in Checkbox mode with node 'a' listed in CheckedKeys")]
    [When("the component renders")]
    [Then("the checkbox for node 'a' has the 'checked' attribute")]
    public void UAC01_CheckedAttributePresentWhenInCheckedKeys()
    {
        var cut = RenderTree(TreeViewCheckMode.Checkbox, checkedKeys: ["a"]);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
           .GetAttribute("checked")
           .ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: checkbox renders without checked attribute when key is absent from CheckedKeys")]
    [Trait(Spec.UAC, "02")]
    [Given("a tree in Checkbox mode with an empty CheckedKeys list")]
    [When("the component renders")]
    [Then("the checkbox for node 'a' does not have the 'checked' attribute")]
    public void UAC02_CheckedAttributeAbsentWhenNotInCheckedKeys()
    {
        var cut = RenderTree(TreeViewCheckMode.Checkbox, checkedKeys: []);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
           .GetAttribute("checked")
           .ShouldBeNull();
    }

    // -------------------------------------------------------------------------
    // data-indeterminate attribute — drives [data-indeterminate="true"] CSS selector
    // -------------------------------------------------------------------------

    [Fact(DisplayName = "UAC03: TriState parent has data-indeterminate='true' when some (not all) children are checked")]
    [Trait(Spec.UAC, "03")]
    [Given("a TriState tree where only child 'a1' is checked but not 'a2'")]
    [When("the component renders")]
    [Then("the parent 'a' checkbox has data-indeterminate='true'")]
    public void UAC03_TriStateParentIndeterminateWhenPartialChildren()
    {
        // Only a1 checked, a2 not → parent indeterminate
        var cut = RenderTree(TreeViewCheckMode.TriState, checkedKeys: ["a1"]);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
           .GetAttribute("data-indeterminate")
           .ShouldBe("true");
    }

    [Fact(DisplayName = "UAC04: TriState parent has data-indeterminate='false' when all children are checked")]
    [Trait(Spec.UAC, "04")]
    [Given("a TriState tree where all children of 'a' are checked")]
    [When("the component renders")]
    [Then("the parent 'a' checkbox has data-indeterminate='false', not 'true'")]
    public void UAC04_TriStateParentNotIndeterminateWhenAllChildrenChecked()
    {
        var cut = RenderTree(TreeViewCheckMode.TriState, checkedKeys: ["a", "a1", "a2"]);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
           .GetAttribute("data-indeterminate")
           .ShouldBe("false");
    }

    [Fact(DisplayName = "UAC05: TriState parent has data-indeterminate='false' when no children are checked")]
    [Trait(Spec.UAC, "05")]
    [Given("a TriState tree with an empty CheckedKeys list")]
    [When("the component renders")]
    [Then("all checkboxes have data-indeterminate='false'")]
    public void UAC05_TriStateParentNotIndeterminateWhenNoChildrenChecked()
    {
        var cut = RenderTree(TreeViewCheckMode.TriState, checkedKeys: []);

        foreach (var checkbox in cut.FindAll(".lt-tree-node-checkbox"))
            checkbox.GetAttribute("data-indeterminate").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC06: Checkbox mode (non-TriState) never sets data-indeterminate='true' on a parent")]
    [Trait(Spec.UAC, "06")]
    [Given("a tree in Checkbox mode with only a child 'a1' checked")]
    [When("the component renders")]
    [Then("the parent 'a' checkbox has data-indeterminate='false', not 'true'")]
    public void UAC06_CheckboxModeNeverIndeterminateOnParent()
    {
        // In Checkbox mode (not TriState), no cascade; parent should never be indeterminate
        var cut = RenderTree(TreeViewCheckMode.Checkbox, checkedKeys: ["a1"]);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
           .GetAttribute("data-indeterminate")
           .ShouldBe("false");
    }

    // -------------------------------------------------------------------------
    // Event handler correctness — guards @onchange pattern vs @onclick:preventDefault
    // -------------------------------------------------------------------------

    [Fact(DisplayName = "UAC07: clicking a checkbox fires CheckedKeysChanged with the node's key")]
    [Trait(Spec.UAC, "07")]
    [Given("a tree in Checkbox mode with an empty CheckedKeys list")]
    [When("the checkbox for node 'a' is clicked")]
    [Then("CheckedKeysChanged is fired and contains 'a'")]
    public void UAC07_ClickCheckboxFiresCheckedKeysChanged()
    {
        // Guards against @onclick:preventDefault being re-introduced, which would
        // cause Blazor's reconciler to skip the DOM update for the checked state.
        var fired = new List<IReadOnlyList<string>>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys => fired.Add(keys));
        var cut = RenderTree(TreeViewCheckMode.Checkbox, checkedKeys: [], checkedKeysChanged: cb);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").Change(true);

        fired.Count.ShouldBe(1);
        fired[0].ShouldContain("a");
    }

    [Fact(DisplayName = "UAC08: after click, checkbox renders with checked attribute present (state reflects post-click)")]
    [Trait(Spec.UAC, "08")]
    [Given("a tree in Checkbox mode with an empty CheckedKeys list")]
    [When("the checkbox for node 'a' is clicked")]
    [Then("the checkbox for node 'a' renders with the 'checked' attribute on the next render")]
    public void UAC08_CheckboxAttributeUpdatesAfterClick()
    {
        var cut = RenderTree(TreeViewCheckMode.Checkbox, checkedKeys: []);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").Change(true);
        cut.Render();

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
           .GetAttribute("checked")
           .ShouldNotBeNull();
    }
}
