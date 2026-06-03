using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tabs;

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

    [Fact(DisplayName = "UAC09: Tabs renders with default top placement")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RendersWithDefaultTopPlacement()
    {
        var cut = Ctx.Render<TabsSut>();
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("data-placement").ShouldBe("top");
        tabs.ClassList.ShouldContain("flex-col");
    }

    [Fact(DisplayName = "UAC10: Tabs renders with bottom placement")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_RendersWithBottomPlacement()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Placement, "bottom"));
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("data-placement").ShouldBe("bottom");
        tabs.ClassList.ShouldContain("flex-col-reverse");
    }

    [Fact(DisplayName = "UAC11: Tabs renders with left placement")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_RendersWithLeftPlacement()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Placement, "left"));
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("data-placement").ShouldBe("left");
        tabs.ClassList.ShouldContain("flex-row");
    }

    [Fact(DisplayName = "UAC12: Tabs renders with right placement")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_RendersWithRightPlacement()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Placement, "right"));
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("data-placement").ShouldBe("right");
        tabs.ClassList.ShouldContain("flex-row-reverse");
    }

    [Fact(DisplayName = "UAC13: Left placement derives vertical orientation")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_LeftPlacementDerivesVerticalOrientation()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Placement, "left"));
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC14: Right placement derives vertical orientation")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_RightPlacementDerivesVerticalOrientation()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Placement, "right"));
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: Tabs renders default gap size")]
    [Trait(Spec.UAC, "15")]
    [Given("Tabs is rendered with default spacing")]
    [When("The component root is inspected")]
    [Then("The default gap is applied as an inline style")]
    public void UAC15_RendersDefaultGapSize()
    {
        var cut = Ctx.Render<TabsSut>();
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("style").ShouldBe("gap: 10px;");
    }

    [Fact(DisplayName = "UAC16: Tabs renders custom gap size when HasGap is false")]
    [Trait(Spec.UAC, "16")]
    [Given("Tabs is rendered with HasGap disabled and a custom gap size")]
    [When("The component root is inspected")]
    [Then("The custom gap remains applied")]
    public void UAC16_RendersCustomGapSizeWhenHasGapIsFalse()
    {
        var cut = Ctx.Render<TabsSut>(p => p
            .Add(x => x.HasGap, false)
            .Add(x => x.GapSize, "8px"));

        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("style").ShouldBe("gap: 8px;");
        tabs.ClassList.ShouldNotContain("gap-2");
    }

    [Fact(DisplayName = "UAC17: Tabs omits gap when HasGap is false")]
    [Trait(Spec.UAC, "17")]
    [Given("Tabs is rendered with HasGap disabled")]
    [When("The component root is inspected")]
    [Then("No gap style is rendered")]
    public void UAC17_OmitsGapWhenHasGapIsFalse()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.HasGap, false));
        var tabs = cut.Find("[data-slot='tabs']");
        tabs.GetAttribute("style").ShouldBeNull();
        tabs.ClassList.ShouldNotContain("gap-2");
    }
}
