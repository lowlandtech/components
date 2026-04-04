using Bunit;
using EmptySut = LowlandTech.Components.Empty.Empty;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1022.Empty.US01.View;

[Trait(Spec.SPEC, "1022")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Empty component")]
public class SC01_RenderEmpty : ViewScenario
{
    [Fact(DisplayName = "UAC01: Empty renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<EmptySut>();
        cut.Find("[data-slot='empty']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Empty renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<EmptySut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Empty renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<EmptySut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='empty']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Empty has default layout classes")]
    [Trait(Spec.UAC, "04")]
    [Then("the element always has the cn-empty base classes")]
    public void UAC04_HasDefaultClasses()
    {
        var cut = Ctx.Render<EmptySut>();
        var el = cut.Find("[data-slot='empty']");
        el.ClassList.ShouldContain("cn-empty");
        el.ClassList.ShouldContain("flex-col");
        el.ClassList.ShouldContain("items-center");
    }
}
