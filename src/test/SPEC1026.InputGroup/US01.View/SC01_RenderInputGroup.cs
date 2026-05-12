using Bunit;
using InputGroupSut = LowlandTech.Components.InputGroup.InputGroup;

namespace LowlandTech.Components.Tests.SPEC1026.InputGroup.US01.View;

[Trait(Spec.SPEC, "1026")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render InputGroup component")]
public class SC01_RenderInputGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputGroupSut>();
        cut.Find("[data-slot='input-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: InputGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<InputGroupSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: InputGroup renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<InputGroupSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='input-group']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: InputGroup has role='group' and cn-input-group base class")]
    [Trait(Spec.UAC, "04")]
    [Then("the element has role='group' and cn-input-group class")]
    public void UAC04_HasRoleAndBaseClass()
    {
        var cut = Ctx.Render<InputGroupSut>();
        var el = cut.Find("[data-slot='input-group']");
        el.GetAttribute("role").ShouldBe("group");
        el.ClassList.ShouldContain("cn-input-group");
    }

    [Fact(DisplayName = "UAC05: InputGroupAddon defaults to inline-start align")]
    [Trait(Spec.UAC, "05")]
    [Then("InputGroupAddon data-align defaults to 'inline-start'")]
    public void UAC05_AddonDefaultAlign()
    {
        var cut = Ctx.Render<LowlandTech.Components.InputGroup.InputGroupAddon>();
        var el = cut.Find("[data-slot='input-group-addon']");
        el.GetAttribute("data-align").ShouldBe("inline-start");
        el.ClassList.ShouldContain("cn-input-group-addon");
        el.ClassList.ShouldContain("order-first");
    }

    [Fact(DisplayName = "UAC06: InputGroupAddon inline-end align applies correct classes")]
    [Trait(Spec.UAC, "06")]
    [Then("align='inline-end' gives order-last class")]
    public void UAC06_AddonInlineEndAlign()
    {
        var cut = Ctx.Render<LowlandTech.Components.InputGroup.InputGroupAddon>(
            p => p.Add(x => x.Align, "inline-end"));
        var el = cut.Find("[data-slot='input-group-addon']");
        el.GetAttribute("data-align").ShouldBe("inline-end");
        el.ClassList.ShouldContain("order-last");
    }
}
