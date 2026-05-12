using Bunit;
using SliderSut = LowlandTech.Components.Slider.Slider;
using LowlandTech.Components.Slider;

namespace LowlandTech.Components.Tests.SPEC1045.Slider.US01.View;

[Trait(Spec.SPEC, "1045")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Slider component")]
public class SC01_RenderSlider : ViewScenario
{
    [Fact(DisplayName = "UAC01: Slider renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SliderSut>();
        cut.Find("[data-slot='slider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Slider renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SliderSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Slider renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SliderSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='slider']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Slider renders with default horizontal orientation")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithDefaultHorizontalOrientation()
    {
        var cut = Ctx.Render<SliderSut>();
        cut.Find("[data-orientation='horizontal']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Slider renders with vertical orientation")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithVerticalOrientation()
    {
        var cut = Ctx.Render<SliderSut>(p => p.Add(x => x.Orientation, "vertical"));
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Slider renders with default Min, Max, Step values")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWithDefaultValues()
    {
        var cut = Ctx.Render<SliderSut>();
        var slider = cut.Find("[role='slider']");
        slider.GetAttribute("aria-valuemin").ShouldBe("0");
        slider.GetAttribute("aria-valuemax").ShouldBe("100");
    }

    [Fact(DisplayName = "UAC07: Slider renders with custom Min, Max, Step values")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersWithCustomValues()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Min, 10);
            p.Add(x => x.Max, 50);
            p.Add(x => x.Step, 5);
        });
        var slider = cut.Find("[role='slider']");
        slider.GetAttribute("aria-valuemin").ShouldBe("10");
        slider.GetAttribute("aria-valuemax").ShouldBe("50");
    }

    [Fact(DisplayName = "UAC08: Slider renders with Value parameter")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersWithValue()
    {
        var cut = Ctx.Render<SliderSut>(p => p.Add(x => x.Value, 50));
        var slider = cut.Find("[role='slider']");
        slider.GetAttribute("aria-valuenow").ShouldBe("50");
    }

    [Fact(DisplayName = "UAC09: Slider renders with Disabled state")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RendersWithDisabledState()
    {
        var cut = Ctx.Render<SliderSut>(p => p.Add(x => x.Disabled, true));
        var slider = cut.Find("[data-slot='slider']");
        slider.HasAttribute("data-disabled").ShouldBeTrue();
        slider.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC10: Slider has role=slider attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_HasRoleSlider()
    {
        var cut = Ctx.Render<SliderSut>();
        cut.Find("[role='slider']").ShouldNotBeNull();
    }
}
