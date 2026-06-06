using LowlandTech.Components.Core;
using LowlandTech.Components.Tests.SPEC1072.DataForm.Models;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US02.FormFieldComponent;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "FormField Component Rendering")]
public class SC01_FormFieldRendering : ViewScenario
{
    [Fact(DisplayName = "UAC01: Text FormField renders input type text")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TextFieldRendersTextInput()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "testField")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Test"));

        cut.Find("[data-field-input='testField']").GetAttribute("type").ShouldBe("text");
    }

    [Fact(DisplayName = "UAC02: Number FormField renders input type number")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NumberFieldRendersNumberInput()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "age")
            .Add(x => x.FieldType, FormFieldType.Number)
            .Add(x => x.Label, "Age"));

        cut.Find("[data-field-input='age']").GetAttribute("type").ShouldBe("number");
    }

    [Fact(DisplayName = "UAC03: Bool FormField renders Checkbox component")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_BoolFieldRendersCheckbox()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "isActive")
            .Add(x => x.FieldType, FormFieldType.Bool)
            .Add(x => x.Label, "Active"));

        cut.Find("[data-field-input='isActive']").GetAttribute("data-slot").ShouldBe("checkbox");
    }

    [Fact(DisplayName = "UAC04: Enum FormField renders Select component")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EnumFieldRendersSelect()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "status")
            .Add(x => x.FieldType, FormFieldType.Enum)
            .Add(x => x.Label, "Status")
            .Add(x => x.EnumType, typeof(CustomerStatus)));

        cut.Find("[data-field-input='status']").GetAttribute("data-slot").ShouldBe("select");
    }

    [Fact(DisplayName = "UAC05: Date FormField renders input type date")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DateFieldRendersDateInput()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "birthDate")
            .Add(x => x.FieldType, FormFieldType.Date)
            .Add(x => x.Label, "Birth Date"));

        cut.Find("[data-field-input='birthDate']").GetAttribute("type").ShouldBe("date");
    }

    [Fact(DisplayName = "UAC06: Guid FormField renders input with placeholder")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_GuidFieldRendersWithPlaceholder()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "id")
            .Add(x => x.FieldType, FormFieldType.Guid)
            .Add(x => x.Label, "ID"));

        cut.Find("[data-field-input='id']").GetAttribute("placeholder").ShouldContain("xxxx");
    }

    [Fact(DisplayName = "UAC07: Json FormField renders textarea")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_JsonFieldRendersTextarea()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "config")
            .Add(x => x.FieldType, FormFieldType.Json)
            .Add(x => x.Label, "Config"));

        cut.Find("[data-field-input='config']").TagName.ShouldBe("TEXTAREA");
    }

    [Fact(DisplayName = "UAC08: Required field shows asterisk in label")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RequiredFieldShowsAsterisk()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.Required, true));

        cut.Find("label").InnerHtml.ShouldContain("*");
    }
}
