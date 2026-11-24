namespace LowlandTech.Components.Tests.SPEC1065.EdgeForm.US02.ReadOnlyView;

[Trait(Spec.SPEC, "1065")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "A view-only relation does not look editable")]
public class SC01_ReadOnlyView : ViewScenario
{
    private static EdgeFormField[] RelationFields() =>
    [
        new("source", "Source", FieldType.Text),
        new("kind", "Kind", FieldType.Select, Options: [new SelectOption("depends", "Depends on")])
    ];

    [Fact(DisplayName = "UAC01: read-only fields show text and no input")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ReadOnlyShowsTextAndNoInput()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, RelationFields())
            .Add(x => x.ReadOnly, true));

        cut.Find("[data-field-readonly='source']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='source']").Count.ShouldBe(0);
        cut.FindAll("[data-field-input='kind']").Count.ShouldBe(0);
        cut.Find("[data-slot='edge-form']").GetAttribute("data-readonly").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC02: a read-only form raises no change")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ReadOnlyRaisesNoChange()
    {
        var raised = 0;

        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, RelationFields())
            .Add(x => x.ReadOnly, true)
            .Add(x => x.OnChange, (EdgeFormResult _) => raised++));

        // Nothing writable is rendered, so there is no control that could raise a change.
        cut.FindAll("[data-field-input]").Count.ShouldBe(0);
        cut.Markup.ShouldNotContain("oninput");
        cut.Markup.ShouldNotContain("onchange");

        raised.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: supplied values are what the read-only view shows")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SuppliedValuesAreShown()
    {
        var values = new Dictionary<string, string?> { ["source"] = "Episode 1", ["kind"] = "depends" };

        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, RelationFields())
            .Add(x => x.ReadOnly, true)
            .Add(x => x.Values, values));

        cut.Find("[data-field-readonly='source']").TextContent.Trim().ShouldBe("Episode 1");
        cut.Find("[data-field-readonly='kind']").TextContent.Trim().ShouldBe("depends");
    }

    [Fact(DisplayName = "UAC04: the default form still edits and reports changes")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EditableDefaultUnchanged()
    {
        EdgeFormResult? result = null;

        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, RelationFields())
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='source']").ShouldNotBeNull();
        cut.Find("[data-slot='edge-form']").GetAttribute("data-readonly").ShouldBeNull();
        cut.Find("[data-field-input='source']").Input("Episode 2");

        result.ShouldNotBeNull();
        result!.IsValid.ShouldBeTrue();
    }
}
