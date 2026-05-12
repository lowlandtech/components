using LowlandTech.Components.EdgeForm;

namespace LowlandTech.Components.Tests.SPEC1065.EdgeForm.US01.View;

[Trait(Spec.SPEC, "1065")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
[UserStory("01", "DynamicEdgeForm custom rule validation")]
public class SC03_RuleValidation : ViewScenario
{
    private sealed class AlwaysFailRule : IEdgeRule
    {
        public string Name => "always-fail";
        public string FailMessage => "This always fails.";
        public bool Evaluate(IReadOnlyDictionary<string, object?> values) => false;
    }

    private sealed class AlwaysPassRule : IEdgeRule
    {
        public string Name => "always-pass";
        public string FailMessage => "This should never appear.";
        public bool Evaluate(IReadOnlyDictionary<string, object?> values) => true;
    }

    private sealed class MinLengthRule(string fieldName, int min) : IEdgeRule
    {
        public string Name => $"min-length:{fieldName}:{min}";
        public string FailMessage => $"{fieldName} must be at least {min} characters.";
        public bool Evaluate(IReadOnlyDictionary<string, object?> values)
            => values.TryGetValue(fieldName, out var v) && v?.ToString()?.Length >= min;
    }

    [Fact(DisplayName = "UAC01: failing rule sets IsValid=false")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_FailingRuleSetsInvalid()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("x", "X", FieldType.Text)])
            .Add(x => x.Rules, [new AlwaysFailRule()])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='x']").Input("anything");

        result.ShouldNotBeNull();
        result!.IsValid.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: failing rule adds FailMessage to Errors")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_FailingRuleAddsError()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("x", "X", FieldType.Text)])
            .Add(x => x.Rules, [new AlwaysFailRule()])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='x']").Input("anything");

        result!.Errors.ShouldContain("This always fails.");
    }

    [Fact(DisplayName = "UAC03: passing rule sets IsValid=true with empty Errors")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PassingRuleSetsValid()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("x", "X", FieldType.Text)])
            .Add(x => x.Rules, [new AlwaysPassRule()])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='x']").Input("anything");

        result.ShouldNotBeNull();
        result!.IsValid.ShouldBeTrue();
        result!.Errors.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC04: multiple rules — all must pass for IsValid=true")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_AllRulesMustPass()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text)])
            .Add(x => x.Rules, [new AlwaysPassRule(), new AlwaysFailRule()])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='name']").Input("anything");

        result!.IsValid.ShouldBeFalse();
        result!.Errors.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC05: MinLengthRule fails when value is too short")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_MinLengthRuleFailsWhenShort()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text)])
            .Add(x => x.Rules, [new MinLengthRule("name", 5)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='name']").Input("hi");

        result!.IsValid.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC06: MinLengthRule passes when value meets minimum")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_MinLengthRulePassesWhenLongEnough()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("name", "Name", FieldType.Text)])
            .Add(x => x.Rules, [new MinLengthRule("name", 5)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='name']").Input("hello world");

        result!.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC07: no rules → IsValid=true always")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NoRulesAlwaysValid()
    {
        EdgeFormResult? result = null;
        var cut = Ctx.Render<DynamicEdgeForm>(p => p
            .Add(x => x.Fields, [new EdgeFormField("x", "X", FieldType.Text)])
            .Add(x => x.OnChange, (EdgeFormResult r) => result = r));

        cut.Find("[data-field-input='x']").Input("anything");

        result!.IsValid.ShouldBeTrue();
    }
}
