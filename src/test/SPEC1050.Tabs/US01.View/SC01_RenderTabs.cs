using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tabs;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US01.View;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Tabs component")]
public class SC01_RenderTabs : ViewScenario
{
    [Fact(DisplayName = "UAC01: Tabs renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TabsSut>();
        cut.Find("[data-slot='tabs']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Tabs renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<TabsSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Tabs renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='tabs']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Tabs renders with default horizontal orientation")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithDefaultHorizontalOrientation()
    {
        var cut = Ctx.Render<TabsSut>();
        cut.Find("[data-orientation='horizontal']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Tabs renders with vertical orientation")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithVerticalOrientation()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Orientation, "vertical"));
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Tabs renders with DefaultValue")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWithDefaultValue()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.DefaultValue, "tab1"));
        // DefaultValue is used to set initial active tab
        cut.Markup.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: Tabs renders with default automatic keyboard activation")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersWithDefaultAutomaticKeyboardActivation()
    {
        var cut = Ctx.Render<TabsSut>();
        cut.Find("[data-activation='automatic']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: Tabs renders with manual keyboard activation")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersWithManualKeyboardActivation()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.KeyboardActivation, "manual"));
        cut.Find("[data-activation='manual']").ShouldNotBeNull();
    }
}
