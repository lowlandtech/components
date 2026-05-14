using LowlandTech.Components.ContainerGrid;

namespace LowlandTech.Components.Tests.SPEC1066.ContainerGrid.US01.Structure;

[Trait(Spec.SPEC, "1066")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "00")]
[UserStory("01", "ContainerGrid structural contract")]
public class SC00_ContainerGridStructuralContract : JsViewScenario
{
    [Fact(DisplayName = "UAC01: ContainerGrid renders data-slot='container-grid'")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>();
        cut.Find("[data-slot='container-grid']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ContainerGrid renders overlay and placeholder divs")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersOverlayAndPlaceholder()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>();
        cut.Find(".lt-container-grid-overlay").ShouldNotBeNull();
        cut.Find(".lt-container-grid-placeholder").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: ContainerGrid default Columns=12 reflected in grid-template-columns style")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DefaultColumnsStyle()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>();
        var style = cut.Find("[data-slot='container-grid']").GetAttribute("style")!;
        style.ShouldContain("grid-template-columns:repeat(12,1fr)");
    }

    [Fact(DisplayName = "UAC04: ContainerGrid custom Columns reflected in style")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_CustomColumnsStyle()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.Add(x => x.Columns, 6));
        var style = cut.Find("[data-slot='container-grid']").GetAttribute("style")!;
        style.ShouldContain("grid-template-columns:repeat(6,1fr)");
    }

    [Fact(DisplayName = "UAC05: ContainerGrid default Rows=4 reflected in grid-template-rows style")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DefaultRowsStyle()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>();
        var style = cut.Find("[data-slot='container-grid']").GetAttribute("style")!;
        style.ShouldContain("grid-template-rows:repeat(4,1fr)");
    }

    [Fact(DisplayName = "UAC06: ContainerGrid renders ChildContent")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersChildContent()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.AddChildContent("<span id='test-child'>hello</span>"));
        cut.Find("#test-child").ShouldNotBeNull();
    }

    // ── CSS variable contract (consumed by .lt-container-grid-overlay rasters) ──
    // Drift guard for SPEC2100.US29 (Default Root Grid Seed).

    [Fact(DisplayName = "UAC11: ContainerGrid emits --lt-grid-cols matching Columns prop")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_EmitsGridColsCssVariable()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.Add(x => x.Columns, 12));
        var style = cut.Find("[data-slot='container-grid']").GetAttribute("style") ?? string.Empty;
        style.ShouldContain("--lt-grid-cols:12");
    }

    [Fact(DisplayName = "UAC12: ContainerGrid emits --lt-grid-rows matching Rows prop")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_EmitsGridRowsCssVariable()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.Add(x => x.Rows, 8));
        var style = cut.Find("[data-slot='container-grid']").GetAttribute("style") ?? string.Empty;
        style.ShouldContain("--lt-grid-rows:8");
    }
}
