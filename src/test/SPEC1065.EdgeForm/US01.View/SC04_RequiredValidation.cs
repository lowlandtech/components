using LowlandTech.Components.EdgeForm;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1065.EdgeForm.US01.View;

[Trait(Spec.SPEC, "1065")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "04")]
[UserStory("01", "EdgeRequiredRule validation")]
public class SC04_RequiredValidation : ViewScenario
{
    [Fact(DisplayName = "UAC01: required text field empty → IsValid=false")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RequiredEmptyTextIsInvalid()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text, Required: true)])
            .Add(x => x.Rules, [new EdgeRequiredRule("name")])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='name']").Input("");

        result.ShouldNotBeNull();
        result!.IsValid.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: required text field filled → IsValid=true")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RequiredFilledTextIsValid()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text, Required: true)])
            .Add(x => x.Rules, [new EdgeRequiredRule("name")])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='name']").Input("Alice");

        result.ShouldNotBeNull();
        result!.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: EdgeRequiredRule.Evaluate returns false for null")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_EvaluateFalseForNull()
    {
        var rule = new EdgeRequiredRule("name");
        var result = rule.Evaluate(new Dictionary<string, object?> { ["name"] = null });
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: EdgeRequiredRule.Evaluate returns false for empty string")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EvaluateFalseForEmpty()
    {
        var rule = new EdgeRequiredRule("name");
        var result = rule.Evaluate(new Dictionary<string, object?> { ["name"] = "" });
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC05: EdgeRequiredRule.Evaluate returns false for whitespace")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_EvaluateFalseForWhitespace()
    {
        var rule = new EdgeRequiredRule("name");
        var result = rule.Evaluate(new Dictionary<string, object?> { ["name"] = "   " });
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC06: EdgeRequiredRule.Evaluate returns false when key absent")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_EvaluateFalseWhenAbsent()
    {
        var rule = new EdgeRequiredRule("name");
        var result = rule.Evaluate(new Dictionary<string, object?>());
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC07: EdgeRequiredRule.Evaluate returns true for non-empty string")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_EvaluateTrueForValue()
    {
        var rule = new EdgeRequiredRule("name");
        var result = rule.Evaluate(new Dictionary<string, object?> { ["name"] = "Alice" });
        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC08: EdgeRequiredRule.Evaluate returns true for true boolean")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_EvaluateTrueForBoolTrue()
    {
        var rule = new EdgeRequiredRule("accepted");
        var result = rule.Evaluate(new Dictionary<string, object?> { ["accepted"] = true });
        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: EdgeRequiredRule.Evaluate returns false for false boolean")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_EvaluateFalseForBoolFalse()
    {
        var rule = new EdgeRequiredRule("accepted");
        var result = rule.Evaluate(new Dictionary<string, object?> { ["accepted"] = false });
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC10: required error message includes field name")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ErrorMessageIncludesFieldName()
    {
        var rule = new EdgeRequiredRule("customerName");
        rule.FailMessage.ShouldContain("customerName");
    }
}
