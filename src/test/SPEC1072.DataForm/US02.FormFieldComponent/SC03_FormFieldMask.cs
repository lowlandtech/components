using LowlandTech.Components.Core;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US02.FormFieldComponent;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "03")]
[UserStory("02", "FormField Mask Support")]
public class SC03_FormFieldMask : ViewScenario
{
    [Fact(DisplayName = "UAC01: Mask formats input with digit placeholders")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MaskFormatsDigits()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "phone")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Phone")
            .Add(x => x.Mask, "(000) 000-0000"));

        cut.Find("[data-field-input='phone']").Input("5551234567");

        cut.Find("[data-field-input='phone']").GetAttribute("value").ShouldBe("(555) 123-4567");
    }

    [Fact(DisplayName = "UAC02: Mask with letters only accepts letter input")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_MaskWithLetters()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "code")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Code")
            .Add(x => x.Mask, "AAA-000"));

        cut.Find("[data-field-input='code']").Input("ABC123");

        cut.Find("[data-field-input='code']").GetAttribute("value").ShouldBe("ABC-123");
    }

    [Fact(DisplayName = "UAC03: Incomplete mask shows validation error on blur")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_IncompleteMaskShowsError()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "ssn")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "SSN")
            .Add(x => x.Mask, "000-00-0000"));

        cut.Find("[data-field-input='ssn']").Input("123");
        cut.Find("[data-field-input='ssn']").Blur();

        cut.Find("[data-field-validation='ssn']").TextContent.ShouldContain("format");
    }

    [Fact(DisplayName = "UAC04: Complete mask passes validation")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_CompleteMaskPassesValidation()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "ssn")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "SSN")
            .Add(x => x.Mask, "000-00-0000"));

        cut.Find("[data-field-input='ssn']").Input("123456789");
        cut.Find("[data-field-input='ssn']").Blur();

        cut.FindAll("[data-field-validation='ssn']").Count.ShouldBe(0);
    }
}
