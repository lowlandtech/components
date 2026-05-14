using LowlandTech.Components;
using LowlandTech.Components.Grid;
using GridComponent = global::LowlandTech.Components.Grid.Grid;

namespace LowlandTech.Components.Tests.SPEC1063.Grid.US01.View;

[Trait(Spec.SPEC, "1063")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Grid component")]
public class SC01_RenderGrid : ViewScenario
{
    [Fact(DisplayName = "UAC01: Grid renders data-slot='grid'")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<GridComponent>();
        cut.Find("[data-slot='grid']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Grid renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<GridComponent>(p => p.AddChildContent("<span>content</span>"));
        cut.Find("[data-slot='grid']").InnerHtml.ShouldContain("content");
    }

    [Fact(DisplayName = "UAC03: Grid applies CSS grid display via inline style")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_AppliesGridStyle()
    {
        var cut = Ctx.Render<GridComponent>();
        cut.Find("[data-slot='grid']").GetAttribute("style")!.ShouldContain("display:grid");
    }

    [Fact(DisplayName = "UAC04: Grid applies 12-column template via inline style")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_Applies12Columns()
    {
        var cut = Ctx.Render<GridComponent>();
        cut.Find("[data-slot='grid']").GetAttribute("style")!.ShouldContain("grid-template-columns:repeat(12,1fr)");
    }

    [Fact(DisplayName = "UAC05: Grid with Spacing applies gap style")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_AppliesSpacingGap()
    {
        var cut = Ctx.Render<GridComponent>(p => p.Add(x => x.Spacing, 4));
        cut.Find("[data-slot='grid']").GetAttribute("style")!.ShouldContain("gap:16px");
    }

    [Fact(DisplayName = "UAC06: Grid with Spacing=0 emits no gap style")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ZeroSpacingNoGap()
    {
        var cut = Ctx.Render<GridComponent>(p => p.Add(x => x.Spacing, 0));
        cut.Find("[data-slot='grid']").GetAttribute("style")!.ShouldNotContain("gap");
    }

    [Fact(DisplayName = "UAC07: Grid applies Justify.Center")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_AppliesJustifyCenter()
    {
        var cut = Ctx.Render<GridComponent>(p => p.Add(x => x.Justify, Justify.Center));
        cut.Find("[data-slot='grid']").GetAttribute("style")!.ShouldContain("justify-content:center");
    }

    [Fact(DisplayName = "UAC08: Grid applies Justify.SpaceBetween")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_AppliesJustifySpaceBetween()
    {
        var cut = Ctx.Render<GridComponent>(p => p.Add(x => x.Justify, Justify.SpaceBetween));
        cut.Find("[data-slot='grid']").GetAttribute("style")!.ShouldContain("justify-content:space-between");
    }

    [Fact(DisplayName = "UAC09: Grid applies additional CSS class")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_AppliesClass()
    {
        var cut = Ctx.Render<GridComponent>(p => p.Add(x => x.Class, "my-grid"));
        cut.Find("[data-slot='grid']").ClassList.ShouldContain("my-grid");
    }

    [Fact(DisplayName = "UAC10: Grid forwards additional attributes")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ForwardsAdditionalAttributes()
    {
        var cut = Ctx.Render<GridComponent>(p => p.AddUnmatched("data-testid", "my-grid"));
        cut.Find("[data-testid='my-grid']").ShouldNotBeNull();
    }
}
