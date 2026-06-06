using Bunit;
using FieldSut = LowlandTech.Components.Field;

namespace LowlandTech.Components.Tests.SPEC1023.Field.US01.View;

[Trait(Spec.SPEC, "1023")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Field component")]
public class SC01_RenderField : ViewScenario
{
    [Fact(DisplayName = "UAC01: Field renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<FieldSut>();
        cut.Find("[data-slot='field']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Field renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<FieldSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Field renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<FieldSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='field']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Field renders with Orientation parameter")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithOrientation()
    {
        var cut = Ctx.Render<FieldSut>(p => p.Add(x => x.Orientation, "horizontal"));
        cut.Find("[data-orientation='horizontal']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Field has group role")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_HasGroupRole()
    {
        var cut = Ctx.Render<FieldSut>();
        cut.Find("[role='group']").ShouldNotBeNull();
    }
}
