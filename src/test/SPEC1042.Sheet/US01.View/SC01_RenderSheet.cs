using Bunit;
using SheetComponent = LowlandTech.Components.Sheet;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1042.Sheet.US01.View;

[Trait(Spec.SPEC, "1042")]
[Trait(Spec.US, "01")]
[UserStory("01", "Sheet component rendering")]
public class SC01_RenderSheet : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sheet renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SheetRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetComponent>();
        cut.Find("[data-slot='sheet']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Sheet renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SheetRendersChildContent()
    {
        var cut = Ctx.Render<SheetComponent>(p => p.AddChildContent("<span class='test'>Content</span>"));
        cut.Find(".test").TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "UAC03: SheetTrigger renders with data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SheetTriggerRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetTrigger>();
        var trigger = cut.Find("[data-slot='sheet-trigger']");
        trigger.ShouldNotBeNull();
        trigger.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC04: SheetTrigger renders ChildContent")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SheetTriggerRendersChildContent()
    {
        var cut = Ctx.Render<SheetTrigger>(p => p.AddChildContent("Open"));
        cut.Find("[data-slot='sheet-trigger']").TextContent.ShouldBe("Open");
    }

    [Fact(DisplayName = "UAC05: SheetContent renders with data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SheetContentRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetContent>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='sheet-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: SheetContent renders overlay when IsOpen")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SheetContentRendersOverlayWhenIsOpen()
    {
        var cut = Ctx.Render<SheetContent>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='sheet-overlay']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: SheetContent does not render when IsOpen is false")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_SheetContentDoesNotRenderWhenIsOpenFalse()
    {
        var cut = Ctx.Render<SheetContent>(p => p.Add(x => x.IsOpen, false));
        cut.FindAll("[data-slot='sheet-content']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: SheetHeader renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_SheetHeaderRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetHeader>();
        cut.Find("[data-slot='sheet-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: SheetFooter renders with data-slot attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_SheetFooterRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetFooter>();
        cut.Find("[data-slot='sheet-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: SheetTitle renders with data-slot attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_SheetTitleRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetTitle>();
        cut.Find("[data-slot='sheet-title']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: SheetTitle renders ChildContent")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_SheetTitleRendersChildContent()
    {
        var cut = Ctx.Render<SheetTitle>(p => p.AddChildContent("Title"));
        cut.Find("[data-slot='sheet-title']").TextContent.ShouldBe("Title");
    }

    [Fact(DisplayName = "UAC12: SheetDescription renders with data-slot attribute")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_SheetDescriptionRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetDescription>();
        cut.Find("[data-slot='sheet-description']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: SheetDescription renders ChildContent")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_SheetDescriptionRendersChildContent()
    {
        var cut = Ctx.Render<SheetDescription>(p => p.AddChildContent("Description"));
        cut.Find("[data-slot='sheet-description']").TextContent.ShouldBe("Description");
    }

    [Fact(DisplayName = "UAC14: SheetClose renders with data-slot attribute")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_SheetCloseRendersWithDataSlot()
    {
        var cut = Ctx.Render<SheetClose>();
        cut.Find("[data-slot='sheet-close']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: SheetClose has sr-only text")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_SheetCloseHasSrOnlyText()
    {
        var cut = Ctx.Render<SheetClose>();
        cut.Find(".sr-only").TextContent.ShouldBe("Close");
    }

    [Theory(DisplayName = "UAC16: SheetContent supports different sides")]
    [Trait(Spec.UAC, "16")]
    [InlineData("right")]
    [InlineData("left")]
    [InlineData("top")]
    [InlineData("bottom")]
    public void UAC16_SheetContentSupportsDifferentSides(string side)
    {
        var cut = Ctx.Render<SheetContent>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.Side, side)
        );
        cut.Find("[data-slot='sheet-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC17: Full Sheet SUT renders correctly")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_FullSheetSutRenders()
    {
        var cut = Ctx.Render<SheetSut>();
        cut.Find("[data-slot='sheet']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-trigger']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-content']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-overlay']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-header']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-footer']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-title']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-description']").ShouldNotBeNull();
        cut.Find("[data-slot='sheet-close']").ShouldNotBeNull();
    }
}
