using Bunit;
using TextareaSut = LowlandTech.Components.Textarea.Textarea;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1051.Textarea.US01.View;

[Trait(Spec.SPEC, "1051")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Textarea component")]
public class SC01_RenderTextarea : ViewScenario
{
    [Fact(DisplayName = "UAC01: Textarea renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TextareaSut>();
        cut.Find("[data-slot='textarea']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Textarea renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Textarea renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='textarea']").ClassList.ShouldContain("test-class");
    }
}
