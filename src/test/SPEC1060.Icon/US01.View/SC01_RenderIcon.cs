using Bunit;
using IconSut = LowlandTech.Components.Icon.Icon;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1060.Icon.US01.View;

[Trait(Spec.SPEC, "1060")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Icon component")]
public class SC01_RenderIcon : ViewScenario
{
    [Fact(DisplayName = "UAC01: Icon renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        cut.Find("[data-slot='icon']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Icon renders svg element")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersSvgElement()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        cut.Find("svg").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Icon renders with default size")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersWithDefaultSize()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        var svg = cut.Find("svg");
        svg.GetAttribute("width").ShouldBe("20");
        svg.GetAttribute("height").ShouldBe("20");
    }

    [Fact(DisplayName = "UAC04: Icon renders with custom size")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithCustomSize()
    {
        var cut = Ctx.Render<IconSut>(p =>
        {
            p.Add(x => x.Name, "check");
            p.Add(x => x.Size, 32);
        });
        var svg = cut.Find("svg");
        svg.GetAttribute("width").ShouldBe("32");
        svg.GetAttribute("height").ShouldBe("32");
    }

    [Fact(DisplayName = "UAC05: Icon renders with default viewBox")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithDefaultViewBox()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        var svg = cut.Find("svg");
        svg.GetAttribute("viewBox").ShouldBe("0 0 24 24");
    }

    [Fact(DisplayName = "UAC06: Icon renders with custom viewBox")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWithCustomViewBox()
    {
        var cut = Ctx.Render<IconSut>(p =>
        {
            p.Add(x => x.Name, "check");
            p.Add(x => x.ViewBox, "0 0 20 20");
        });
        var svg = cut.Find("svg");
        svg.GetAttribute("viewBox").ShouldBe("0 0 20 20");
    }

    [Fact(DisplayName = "UAC07: Icon renders with default stroke-width")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersWithDefaultStrokeWidth()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        var svg = cut.Find("svg");
        svg.GetAttribute("stroke-width").ShouldBe("2");
    }

    [Fact(DisplayName = "UAC08: Icon renders with custom stroke-width")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersWithCustomStrokeWidth()
    {
        var cut = Ctx.Render<IconSut>(p =>
        {
            p.Add(x => x.Name, "check");
            p.Add(x => x.StrokeWidth, 1.5);
        });
        var svg = cut.Find("svg");
        var strokeWidth = svg.GetAttribute("stroke-width");
        (strokeWidth == "1.5" || strokeWidth == "1,5").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: Icon renders check icon paths")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RendersCheckIconPaths()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        cut.Find("polyline").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: Icon renders with custom paths")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_RendersWithCustomPaths()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Paths, "<circle cx='12' cy='12' r='10'/>"));
        cut.Find("circle").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: Icon is decorative by default")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_IsDecorativeByDefault()
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, "check"));
        var svg = cut.Find("svg");
        svg.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC12: Icon is informative when Label provided")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_IsInformativeWhenLabelProvided()
    {
        var cut = Ctx.Render<IconSut>(p =>
        {
            p.Add(x => x.Name, "check");
            p.Add(x => x.Label, "Checkmark");
        });
        var svg = cut.Find("svg");
        svg.GetAttribute("role").ShouldBe("img");
        svg.GetAttribute("aria-label").ShouldBe("Checkmark");
    }

    [Fact(DisplayName = "UAC13: Icon renders Class parameter")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_RendersClassParameter()
    {
        var cut = Ctx.Render<IconSut>(p =>
        {
            p.Add(x => x.Name, "check");
            p.Add(x => x.Class, "my-icon");
        });
        cut.Find("svg").ClassList.ShouldContain("my-icon");
    }

    [Theory(DisplayName = "UAC14: Icon renders built-in icons")]
    [Trait(Spec.UAC, "14")]
    [InlineData("check")]
    [InlineData("x")]
    [InlineData("plus")]
    [InlineData("menu")]
    [InlineData("search")]
    public void UAC14_RendersBuiltInIcons(string iconName)
    {
        var cut = Ctx.Render<IconSut>(p => p.Add(x => x.Name, iconName));
        cut.Find("svg").ShouldNotBeNull();
    }
}
