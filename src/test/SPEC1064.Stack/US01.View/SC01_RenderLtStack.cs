using LowlandTech.Components.Grid;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1064.Stack.US01.View;

[Trait(Spec.SPEC, "1064")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render LtStack component")]
public class SC01_RenderLtStack : ViewScenario
{
    [Fact(DisplayName = "UAC01: LtStack renders data-slot='stack'")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<LtStack>();
        cut.Find("[data-slot='stack']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: LtStack renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<LtStack>(p => p.AddChildContent("<span>item</span>"));
        cut.Find("[data-slot='stack']").InnerHtml.ShouldContain("item");
    }

    [Fact(DisplayName = "UAC03: LtStack default is flex column")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DefaultIsColumn()
    {
        var cut = Ctx.Render<LtStack>();
        var style = cut.Find("[data-slot='stack']").GetAttribute("style")!;
        style.ShouldContain("display:flex");
        style.ShouldContain("flex-direction:column");
    }

    [Fact(DisplayName = "UAC04: LtStack Row=true renders flex-row")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RowIsFlexRow()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Row, true));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("flex-direction:row");
    }

    [Fact(DisplayName = "UAC05: LtStack Row=true Reverse=true renders row-reverse")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RowReverseIsRowReverse()
    {
        var cut = Ctx.Render<LtStack>(p =>
            p.Add(x => x.Row, true)
             .Add(x => x.Reverse, true));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("flex-direction:row-reverse");
    }

    [Fact(DisplayName = "UAC06: LtStack Reverse=true (column) renders column-reverse")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ColumnReverseIsColumnReverse()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Reverse, true));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("flex-direction:column-reverse");
    }

    [Fact(DisplayName = "UAC07: LtStack Spacing=3 applies 12px gap")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_SpacingAppliesGap()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Spacing, 3));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("gap:12px");
    }

    [Fact(DisplayName = "UAC08: LtStack Spacing=0 emits no gap style")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ZeroSpacingNoGap()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Spacing, 0));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldNotContain("gap");
    }

    [Fact(DisplayName = "UAC09: LtStack Justify.Center applies justify-content:center")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_JustifyCenter()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Justify, (Justify?)Justify.Center));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("justify-content:center");
    }

    [Fact(DisplayName = "UAC10: LtStack Justify.SpaceBetween applies justify-content:space-between")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_JustifySpaceBetween()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Justify, (Justify?)Justify.SpaceBetween));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("justify-content:space-between");
    }

    [Fact(DisplayName = "UAC11: LtStack AlignItems.Center applies align-items:center")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_AlignItemsCenter()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.AlignItems, (AlignItems?)AlignItems.Center));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("align-items:center");
    }

    [Fact(DisplayName = "UAC12: LtStack AlignItems.Baseline applies align-items:baseline")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_AlignItemsBaseline()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.AlignItems, (AlignItems?)AlignItems.Baseline));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("align-items:baseline");
    }

    [Fact(DisplayName = "UAC13: LtStack Wrap=true applies flex-wrap:wrap")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_WrapAppliesFlexWrap()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Wrap, true));
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldContain("flex-wrap:wrap");
    }

    [Fact(DisplayName = "UAC14: LtStack without Wrap emits no flex-wrap")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_NoWrapByDefault()
    {
        var cut = Ctx.Render<LtStack>();
        cut.Find("[data-slot='stack']").GetAttribute("style").ShouldNotContain("flex-wrap");
    }

    [Fact(DisplayName = "UAC15: LtStack applies additional CSS class")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_AppliesClass()
    {
        var cut = Ctx.Render<LtStack>(p => p.Add(x => x.Class, "my-stack"));
        cut.Find("[data-slot='stack']").ClassList.ShouldContain("my-stack");
    }

    [Fact(DisplayName = "UAC16: LtStack forwards additional attributes")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_ForwardsAdditionalAttributes()
    {
        var cut = Ctx.Render<LtStack>(p => p.AddUnmatched("data-testid", "my-stack"));
        cut.Find("[data-testid='my-stack']").ShouldNotBeNull();
    }
}
