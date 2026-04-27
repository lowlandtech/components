using LowlandTech.Components.EdgeForm;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1065.EdgeForm.US01.View;

[Trait(Spec.SPEC, "1065")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render DynamicEdgeForm fields")]
public class SC01_RendersFields : ViewScenario
{
    [Fact(DisplayName = "UAC01: text field renders input[type=text]")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TextFieldRendersInput()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text)]));
        cut.Find("[data-field-input='name']").GetAttribute("type").ShouldBe("text");
    }

    [Fact(DisplayName = "UAC02: number field renders input[type=number]")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NumberFieldRendersInput()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("qty", "Qty", FieldType.Number)]));
        cut.Find("[data-field-input='qty']").GetAttribute("type").ShouldBe("number");
    }

    [Fact(DisplayName = "UAC03: date field renders input[type=date]")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DateFieldRendersInput()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("due", "Due", FieldType.Date)]));
        cut.Find("[data-field-input='due']").GetAttribute("type").ShouldBe("date");
    }

    [Fact(DisplayName = "UAC04: boolean field renders input[type=checkbox]")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_BooleanFieldRendersCheckbox()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("active", "Active", FieldType.Boolean)]));
        cut.Find("[data-field-input='active']").GetAttribute("type").ShouldBe("checkbox");
    }

    [Fact(DisplayName = "UAC05: select field renders <select>")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SelectFieldRendersSelect()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("status", "Status", FieldType.Select,
                Options: [new SelectOption("a", "A"), new SelectOption("b", "B")])]));
        cut.Find("[data-field-input='status']").TagName.ShouldBe("SELECT");
    }

    [Fact(DisplayName = "UAC06: select options are rendered as <option> elements")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SelectOptionsRendered()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("status", "Status", FieldType.Select,
                Options: [new SelectOption("a", "A"), new SelectOption("b", "B")])]));
        cut.FindAll("[data-field-input='status'] option").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC07: each field has data-field wrapper")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_FieldHasWrapper()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text)]));
        cut.Find("[data-field='name']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: label text is rendered")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_LabelRendered()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Full Name", FieldType.Text)]));
        cut.Find("[data-field-label='name']").TextContent.ShouldContain("Full Name");
    }

    [Fact(DisplayName = "UAC09: placeholder is forwarded to input")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_PlaceholderForwarded()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text, Placeholder: "Enter name")]));
        cut.Find("[data-field-input='name']").GetAttribute("placeholder").ShouldBe("Enter name");
    }

    [Fact(DisplayName = "UAC10: multiple fields all render")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_MultipleFieldsRender()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [
                new EdgeFormField("a", "A", FieldType.Text),
                new EdgeFormField("b", "B", FieldType.Number),
                new EdgeFormField("c", "C", FieldType.Boolean)
            ]));
        cut.Find("[data-field-input='a']").ShouldNotBeNull();
        cut.Find("[data-field-input='b']").ShouldNotBeNull();
        cut.Find("[data-field-input='c']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: edge-form root has data-slot='edge-form'")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_RendersDataSlot()
    {
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("x", "X", FieldType.Text)]));
        cut.Find("[data-slot='edge-form']").ShouldNotBeNull();
    }
}
