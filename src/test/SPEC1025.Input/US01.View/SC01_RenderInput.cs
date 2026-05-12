using Bunit;
using InputSut = LowlandTech.Components.Input.Input;

namespace LowlandTech.Components.Tests.SPEC1025.Input.US01.View;

[Trait(Spec.SPEC, "1025")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Input component")]
public class SC01_RenderInput : ViewScenario
{
    [Fact(DisplayName = "UAC01: Input renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Input renders Value")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains the value")]
    public void UAC02_RendersValue()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Value, "test-value"));
        cut.Find("[data-slot='input']").GetAttribute("value").ShouldBe("test-value");
    }

    [Fact(DisplayName = "UAC03: Input renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='input']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Input renders Type parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains the type attribute")]
    public void UAC04_RendersType()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Type, "password"));
        cut.Find("[data-slot='input']").GetAttribute("type").ShouldBe("password");
    }

    [Fact(DisplayName = "UAC05: Input renders default Type as text")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains type='text' by default")]
    public void UAC05_RendersDefaultType()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").GetAttribute("type").ShouldBe("text");
    }

    [Fact(DisplayName = "UAC06: Input renders Placeholder parameter")]
    [Trait(Spec.UAC, "06")]
    [Then("the rendered markup contains the placeholder attribute")]
    public void UAC06_RendersPlaceholder()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Placeholder, "Enter text..."));
        cut.Find("[data-slot='input']").GetAttribute("placeholder").ShouldBe("Enter text...");
    }

    [Fact(DisplayName = "UAC07: Input renders Disabled state")]
    [Trait(Spec.UAC, "07")]
    [Then("the rendered markup contains the disabled attribute")]
    public void UAC07_RendersDisabled()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Disabled, true));
        cut.Find("[data-slot='input']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC08: Input renders ReadOnly state")]
    [Trait(Spec.UAC, "08")]
    [Then("the rendered markup contains the readonly attribute")]
    public void UAC08_RendersReadOnly()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.ReadOnly, true));
        cut.Find("[data-slot='input']").HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: Input renders default Size")]
    [Trait(Spec.UAC, "09")]
    [Then("the rendered markup has data-size='default'")]
    public void UAC09_RendersDefaultSize()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").GetAttribute("data-size").ShouldBe("default");
    }

    [Fact(DisplayName = "UAC10: Input renders sm Size")]
    [Trait(Spec.UAC, "10")]
    [Then("the rendered markup has data-size='sm'")]
    public void UAC10_RendersSmSize()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Size, "sm"));
        cut.Find("[data-slot='input']").GetAttribute("data-size").ShouldBe("sm");
    }

    [Fact(DisplayName = "UAC11: Input renders lg Size")]
    [Trait(Spec.UAC, "11")]
    [Then("the rendered markup has data-size='lg'")]
    public void UAC11_RendersLgSize()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Size, "lg"));
        cut.Find("[data-slot='input']").GetAttribute("data-size").ShouldBe("lg");
    }

    [Fact(DisplayName = "UAC12: Input renders default Variant")]
    [Trait(Spec.UAC, "12")]
    [Then("the rendered markup has data-variant='default'")]
    public void UAC12_RendersDefaultVariant()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").GetAttribute("data-variant").ShouldBe("default");
    }

    [Fact(DisplayName = "UAC13: Input renders destructive Variant")]
    [Trait(Spec.UAC, "13")]
    [Then("the rendered markup has data-variant='destructive'")]
    public void UAC13_RendersDestructiveVariant()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Variant, "destructive"));
        cut.Find("[data-slot='input']").GetAttribute("data-variant").ShouldBe("destructive");
    }

    [Fact(DisplayName = "UAC14: Input has cn-input class")]
    [Trait(Spec.UAC, "14")]
    [Then("the rendered markup contains cn-input class")]
    public void UAC14_RendersCnInputClass()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").ClassList.ShouldContain("cn-input");
    }
}
