using Bunit;
using SwitchSut = LowlandTech.Components.Switch.Switch;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1048.Switch.US01.View;

[Trait(Spec.SPEC, "1048")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Switch component")]
public class SC01_RenderSwitch : ViewScenario
{
    [Fact(DisplayName = "UAC01: Switch renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SwitchSut>();
        cut.Find("[data-slot='switch']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Switch renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Switch renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='switch']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Switch renders as button element")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersAsButton()
    {
        var cut = Ctx.Render<SwitchSut>();
        cut.Find("button[data-slot='switch']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Switch renders aria-checked attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersAriaChecked()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.IsChecked, true));
        var button = cut.Find("[data-slot='switch']");
        button.GetAttribute("aria-checked").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC06: Switch renders data-state attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersDataState()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.IsChecked, true));
        var button = cut.Find("[data-slot='switch']");
        button.GetAttribute("data-state").ShouldBe("checked");
    }

    [Fact(DisplayName = "UAC07: Switch renders Name parameter")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersName()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.Name, "switch-name"));
        var button = cut.Find("[data-slot='switch']");
        button.GetAttribute("name").ShouldBe("switch-name");
    }

    [Fact(DisplayName = "UAC08: Switch renders Value parameter")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersValue()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.Value, "switch-value"));
        var button = cut.Find("[data-slot='switch']");
        button.GetAttribute("value").ShouldBe("switch-value");
    }
}
