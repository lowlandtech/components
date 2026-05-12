using Bunit;
using TextareaSut = LowlandTech.Components.Textarea.Textarea;

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

    [Fact(DisplayName = "UAC02: Textarea renders Value")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains the value")]
    public void UAC02_RendersValue()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Value, "test-value"));
        cut.Find("[data-slot='textarea']").TextContent.ShouldBe("test-value");
    }

    [Fact(DisplayName = "UAC03: Textarea renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='textarea']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Textarea renders default Rows")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains rows='3' by default")]
    public void UAC04_RendersDefaultRows()
    {
        var cut = Ctx.Render<TextareaSut>();
        cut.Find("[data-slot='textarea']").GetAttribute("rows").ShouldBe("3");
    }

    [Fact(DisplayName = "UAC05: Textarea renders custom Rows")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains the custom rows attribute")]
    public void UAC05_RendersCustomRows()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Rows, 5));
        cut.Find("[data-slot='textarea']").GetAttribute("rows").ShouldBe("5");
    }

    [Fact(DisplayName = "UAC06: Textarea renders Placeholder parameter")]
    [Trait(Spec.UAC, "06")]
    [Then("the rendered markup contains the placeholder attribute")]
    public void UAC06_RendersPlaceholder()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Placeholder, "Enter text..."));
        cut.Find("[data-slot='textarea']").GetAttribute("placeholder").ShouldBe("Enter text...");
    }

    [Fact(DisplayName = "UAC07: Textarea renders Disabled state")]
    [Trait(Spec.UAC, "07")]
    [Then("the rendered markup contains the disabled attribute")]
    public void UAC07_RendersDisabled()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Disabled, true));
        cut.Find("[data-slot='textarea']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC08: Textarea renders ReadOnly state")]
    [Trait(Spec.UAC, "08")]
    [Then("the rendered markup contains the readonly attribute")]
    public void UAC08_RendersReadOnly()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.ReadOnly, true));
        cut.Find("[data-slot='textarea']").HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: Textarea has cn-textarea class")]
    [Trait(Spec.UAC, "09")]
    [Then("the rendered markup contains cn-textarea class")]
    public void UAC09_RendersCnTextareaClass()
    {
        var cut = Ctx.Render<TextareaSut>();
        cut.Find("[data-slot='textarea']").ClassList.ShouldContain("cn-textarea");
    }
}
