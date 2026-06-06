using LowlandTech.Components.Core;
using LowlandTech.Components.Core.Validation;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US02.FormFieldComponent;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "02")]
[UserStory("02", "FormField Validation")]
public class SC02_FormFieldValidation : ViewScenario
{
    [Fact(DisplayName = "UAC01: Required field shows error when empty")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RequiredFieldShowsErrorWhenEmpty()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "email")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Email")
            .Add(x => x.Required, true));

        cut.Find("[data-field-input='email']").Blur();

        cut.Find("[data-field-validation='email']").TextContent.ShouldContain("required");
    }

    [Fact(DisplayName = "UAC02: Validator pattern shows error on mismatch")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_PatternValidatorShowsError()
    {
        var validator = new TestValidatorDescription
        {
            FieldName = "code",
            Pattern = "^[A-Z]{3}$",
            ErrorMessage = "Code must be 3 uppercase letters"
        };

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "code")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Code")
            .Add(x => x.Validator, validator));

        cut.Find("[data-field-input='code']").Input("abc");
        cut.Find("[data-field-input='code']").Blur();

        cut.Find("[data-field-validation='code']").TextContent.ShouldContain("uppercase");
    }

    [Fact(DisplayName = "UAC03: MaxLength validator shows error when exceeded")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_MaxLengthValidatorShowsError()
    {
        var validator = new TestValidatorDescription
        {
            FieldName = "name",
            MaxLength = 5
        };

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.Validator, validator));

        cut.Find("[data-field-input='name']").Input("toolong");
        cut.Find("[data-field-input='name']").Blur();

        cut.Find("[data-field-validation='name']").TextContent.ShouldContain("5");
    }

    [Fact(DisplayName = "UAC04: NCalc rule validator evaluates condition")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NCalcRuleValidator()
    {
        var validator = new TestValidatorDescription
        {
            FieldName = "age",
            Rules = new List<ValidationRule>
            {
                new("MinAge", "value >= 18", "Must be at least 18")
            }
        };

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "age")
            .Add(x => x.FieldType, FormFieldType.Number)
            .Add(x => x.Label, "Age")
            .Add(x => x.Validator, validator));

        cut.Find("[data-field-input='age']").Input("15");
        cut.Find("[data-field-input='age']").Blur();

        cut.Find("[data-field-validation='age']").TextContent.ShouldContain("18");
    }

    [Fact(DisplayName = "UAC05: Valid input clears error state")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ValidInputClearsError()
    {
        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.Required, true));

        cut.Find("[data-field-input='name']").Blur();
        cut.Find("[data-field-validation='name']").ShouldNotBeNull();

        cut.Find("[data-field-input='name']").Input("John");
        cut.Find("[data-field-input='name']").Blur();

        cut.FindAll("[data-field-validation='name']").Count.ShouldBe(0);
    }

    private class TestValidatorDescription : IValidatorDescription
    {
        public string FieldName { get; set; } = string.Empty;
        public IReadOnlyList<ValidationRule> Rules { get; set; } = new List<ValidationRule>();
        public string? Mask { get; set; }
        public int? MaxLength { get; set; }
        public string? Pattern { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
