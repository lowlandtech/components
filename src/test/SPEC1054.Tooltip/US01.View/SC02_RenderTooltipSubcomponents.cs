using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using TooltipSut = LowlandTech.Components.Tooltip.Tooltip;
using LowlandTech.Components.Tooltip;

namespace LowlandTech.Components.Tests.SPEC1054.Tooltip.US01.View;

[Trait(Spec.SPEC, "1054")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Tooltip subcomponents")]
public class SC02_RenderTooltipSubcomponents : ViewScenario
{
    #region TooltipTrigger Tests

    [Fact(DisplayName = "UAC01: TooltipTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TriggerRendersDataSlot()
    {
        var cut = Ctx.Render<TooltipTrigger>();
        cut.Find("[data-slot='tooltip-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: TooltipTrigger renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TriggerRendersChildContent()
    {
        var cut = Ctx.Render<TooltipTrigger>(p => p.AddChildContent("Hover me"));
        cut.Markup.ShouldContain("Hover me");
    }

    [Fact(DisplayName = "UAC03: TooltipTrigger renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_TriggerRendersClass()
    {
        var cut = Ctx.Render<TooltipTrigger>(p => p.Add(x => x.Class, "trigger-class"));
        cut.Find("[data-slot='tooltip-trigger']").ClassList.ShouldContain("trigger-class");
    }

    [Fact(DisplayName = "UAC04: TooltipTrigger renders as button element")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_TriggerRendersAsButton()
    {
        var cut = Ctx.Render<TooltipTrigger>();
        cut.Find("button[data-slot='tooltip-trigger']").ShouldNotBeNull();
    }

    #endregion

    #region TooltipContent Tests

    [Fact(DisplayName = "UAC05: TooltipContent has Side parameter with default 'top'")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ContentHasSideParameter()
    {
        var cut = Ctx.Render<TooltipContent>();
        cut.Instance.Side.ShouldBe("top");
    }

    [Fact(DisplayName = "UAC06: TooltipContent has Align parameter with default 'center'")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ContentHasAlignParameter()
    {
        var cut = Ctx.Render<TooltipContent>();
        cut.Instance.Align.ShouldBe("center");
    }

    [Fact(DisplayName = "UAC07: TooltipContent Side can be customized")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ContentSideCanBeCustomized()
    {
        var cut = Ctx.Render<TooltipContent>(p => p.Add(x => x.Side, "bottom"));
        cut.Instance.Side.ShouldBe("bottom");
    }

    [Fact(DisplayName = "UAC08: TooltipContent Align can be customized")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ContentAlignCanBeCustomized()
    {
        var cut = Ctx.Render<TooltipContent>(p => p.Add(x => x.Align, "start"));
        cut.Instance.Align.ShouldBe("start");
    }

    [Fact(DisplayName = "UAC09: TooltipContent renders role='tooltip'")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ContentHasTooltipRole()
    {
        var cut = Ctx.Render<TooltipSut>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<TooltipTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(2, "Trigger")));
                builder.CloseComponent();
                builder.OpenComponent<TooltipContent>(3);
                builder.AddAttribute(4, "ChildContent", (RenderFragment)(b => b.AddContent(5, "Content")));
                builder.CloseComponent();
            });
        });
        
        // Open the tooltip directly to bypass async delay
        cut.Instance.Open();
        cut.Find("[role='tooltip']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: TooltipContent renders when parent IsOpen is true")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ContentRendersWhenOpen()
    {
        var cut = Ctx.Render<TooltipSut>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<TooltipTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(2, "Trigger")));
                builder.CloseComponent();
                builder.OpenComponent<TooltipContent>(3);
                builder.AddAttribute(4, "ChildContent", (RenderFragment)(b => b.AddContent(5, "Tooltip text")));
                builder.CloseComponent();
            });
        });

        // Open the tooltip directly to bypass async delay
        cut.Instance.Open();
        cut.Markup.ShouldContain("Tooltip text");
    }

    #endregion

    #region TooltipProvider Tests

    [Fact(DisplayName = "UAC11: TooltipProvider renders data-slot attribute")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ProviderRendersDataSlot()
    {
        var cut = Ctx.Render<TooltipProvider>();
        cut.Find("[data-slot='tooltip-provider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: TooltipProvider has Delay parameter with default 200")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_ProviderHasDelayParameter()
    {
        var cut = Ctx.Render<TooltipProvider>();
        cut.Instance.Delay.ShouldBe(200);
    }

    [Fact(DisplayName = "UAC13: TooltipProvider has DelayHide parameter with default 0")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ProviderHasDelayHideParameter()
    {
        var cut = Ctx.Render<TooltipProvider>();
        cut.Instance.DelayHide.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC14: TooltipProvider has Disabled parameter with default false")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ProviderHasDisabledParameter()
    {
        var cut = Ctx.Render<TooltipProvider>();
        cut.Instance.Disabled.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC15: TooltipProvider renders ChildContent")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ProviderRendersChildContent()
    {
        var cut = Ctx.Render<TooltipProvider>(p => p.AddChildContent("Provider content"));
        cut.Markup.ShouldContain("Provider content");
    }

    #endregion

    #region Side and Align Variations

    [Theory(DisplayName = "UAC16: TooltipContent supports all Side values")]
    [Trait(Spec.UAC, "16")]
    [InlineData("top")]
    [InlineData("bottom")]
    [InlineData("left")]
    [InlineData("right")]
    public void UAC16_ContentSupportsAllSides(string side)
    {
        var cut = Ctx.Render<TooltipContent>(p => p.Add(x => x.Side, side));
        cut.Instance.Side.ShouldBe(side);
    }

    [Theory(DisplayName = "UAC17: TooltipContent supports all Align values")]
    [Trait(Spec.UAC, "17")]
    [InlineData("start")]
    [InlineData("center")]
    [InlineData("end")]
    public void UAC17_ContentSupportsAllAligns(string align)
    {
        var cut = Ctx.Render<TooltipContent>(p => p.Add(x => x.Align, align));
        cut.Instance.Align.ShouldBe(align);
    }

    #endregion
}
