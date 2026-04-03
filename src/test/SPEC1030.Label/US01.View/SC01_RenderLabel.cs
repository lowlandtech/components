using Bunit;
using LabelSut = LowlandTech.Components.Label.Label;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1030.Label.US01.View;

[Trait(Spec.SPEC, "1030")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Label component")]
public class SC01_RenderLabel : ViewScenario
{
    [Fact(DisplayName = "UAC01: Label renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<LabelSut>();
        cut.Find("[data-slot='label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Label renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<LabelSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Label renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<LabelSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='label']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Label includes sample default styling")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains label default classes")]
    public void UAC04_IncludesDefaultClasses()
    {
        var cut = Ctx.Render<LabelSut>();
        var element = cut.Find("[data-slot='label']");

        element.ClassList.ShouldContain("cn-label");
        element.ClassList.ShouldContain("flex");
        element.ClassList.ShouldContain("select-none");
    }

    [Fact(DisplayName = "UAC05: Label forwards for attribute")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains the for attribute")]
    public void UAC05_ForwardsForAttribute()
    {
        var cut = Ctx.Render<LabelSut>(p => p.AddUnmatched("for", "email"));
        cut.Find("[data-slot='label']").GetAttribute("for").ShouldBe("email");
    }
}
