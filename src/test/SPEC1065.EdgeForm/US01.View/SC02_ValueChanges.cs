using LowlandTech.Components.EdgeForm;

namespace LowlandTech.Components.Tests.SPEC1065.EdgeForm.US01.View;

[Trait(Spec.SPEC, "1065")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "02")]
[UserStory("01", "DynamicEdgeForm value change events")]
public class SC02_ValueChanges : ViewScenario
{
    [Fact(DisplayName = "UAC01: typing in text field fires OnChange with updated value")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TextInputFiresOnChange()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='name']").Input("hello");

        result.ShouldNotBeNull();
        result!.Values["name"]?.ToString().ShouldBe("hello");
    }

    [Fact(DisplayName = "UAC02: changing number field fires OnChange with value")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NumberInputFiresOnChange()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("qty", "Qty", FieldType.Number)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='qty']").Input("42");

        result.ShouldNotBeNull();
        result!.Values["qty"]?.ToString().ShouldBe("42");
    }

    [Fact(DisplayName = "UAC03: checking boolean field fires OnChange with true")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CheckboxChangeFiresOnChange()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("active", "Active", FieldType.Boolean)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='active']").Change(true);

        result.ShouldNotBeNull();
        result!.Values["active"].ShouldBe(true);
    }

    [Fact(DisplayName = "UAC04: selecting an option fires OnChange with that value")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SelectChangeFiresOnChange()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("status", "Status", FieldType.Select,
                Options: [new SelectOption("draft", "Draft"), new SelectOption("active", "Active")])])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='status']").Change("active");

        result.ShouldNotBeNull();
        result!.Values["status"]?.ToString().ShouldBe("active");
    }

    [Fact(DisplayName = "UAC05: OnChange result contains all current values")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ResultContainsAllValues()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [
                new EdgeFormField("a", "A", FieldType.Text),
                new EdgeFormField("b", "B", FieldType.Text)
            ])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='a']").Input("foo");
        cut.Find("[data-field-input='b']").Input("bar");

        result.ShouldNotBeNull();
        result!.Values["a"]?.ToString().ShouldBe("foo");
        result!.Values["b"]?.ToString().ShouldBe("bar");
    }

    [Fact(DisplayName = "UAC06: date field change fires OnChange with value")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DateInputFiresOnChange()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("due", "Due", FieldType.Date)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='due']").Input("2026-04-27");

        result.ShouldNotBeNull();
        result!.Values["due"]?.ToString().ShouldBe("2026-04-27");
    }
}
