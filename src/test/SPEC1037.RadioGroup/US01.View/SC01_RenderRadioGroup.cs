using Bunit;
using RadioGroupSut = LowlandTech.Components.RadioGroup;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1037.RadioGroup.US01.View;

[Trait(Spec.SPEC, "1037")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render RadioGroup component")]
public class SC01_RenderRadioGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: RadioGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<RadioGroupSut>();
        cut.Find("[data-slot='radio-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: RadioGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<RadioGroupSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: RadioGroup renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<RadioGroupSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='radio-group']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: RadioGroup renders with default vertical orientation")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithDefaultVerticalOrientation()
    {
        var cut = Ctx.Render<RadioGroupSut>();
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: RadioGroup renders with horizontal orientation")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithHorizontalOrientation()
    {
        var cut = Ctx.Render<RadioGroupSut>(p => p.Add(x => x.Orientation, "horizontal"));
        cut.Find("[data-orientation='horizontal']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: RadioGroup renders with Name parameter")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWithNameParameter()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Name, "my-radio-group");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("name").ShouldBe("my-radio-group");
    }

    [Fact(DisplayName = "UAC07: RadioGroup renders with Value parameter")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersWithValueParameter()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Value, "option2");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option2"));
        });
        var radio = cut.FindAll("input[type='radio']")[1];
        radio.GetAttribute("checked").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: RadioGroup renders with Required state")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersWithRequiredState()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Required, true);
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var group = cut.Find("[data-slot='radio-group']");
        group.GetAttribute("aria-required").ShouldBe("true");
        group.HasAttribute("data-required").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: RadioGroup renders with Disabled state")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RendersWithDisabledState()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var group = cut.Find("[data-slot='radio-group']");
        group.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC10: RadioGroup has role=radiogroup attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_HasRoleRadiogroup()
    {
        var cut = Ctx.Render<RadioGroupSut>();
        cut.Find("[role='radiogroup']").ShouldNotBeNull();
    }
}
