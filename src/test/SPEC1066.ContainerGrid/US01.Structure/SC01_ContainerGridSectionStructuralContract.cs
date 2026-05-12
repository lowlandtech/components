using LowlandTech.Components.ContainerGrid;

namespace LowlandTech.Components.Tests.SPEC1066.ContainerGrid.US01.Structure;

[Trait(Spec.SPEC, "1066")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "ContainerGridSection structural contract")]
public class SC01_ContainerGridSectionStructuralContract : JsViewScenario
{
    private IRenderedComponent<LowlandTech.Components.ContainerGrid.ContainerGrid> RenderWithSection(
        Action<Bunit.ComponentParameterCollectionBuilder<ContainerGridSection>> sectionConfig)
        => Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            g => g.AddChildContent<ContainerGridSection>(sectionConfig));

    [Fact(DisplayName = "UAC01: ContainerGridSection renders data-slot='container-grid-section'")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = RenderWithSection(s => s.Add(x => x.Id, "s1"));
        cut.Find("[data-slot='container-grid-section']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ContainerGridSection applies grid-column style")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_AppliesGridColumnStyle()
    {
        var cut = RenderWithSection(s => s
            .Add(x => x.Id, "s1")
            .Add(x => x.Column, 3)
            .Add(x => x.ColSpan, 4));

        var style = cut.Find("[data-slot='container-grid-section']").GetAttribute("style");
        style.ShouldContain("grid-column:3 / span 4");
    }

    [Fact(DisplayName = "UAC03: ContainerGridSection applies grid-row style")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_AppliesGridRowStyle()
    {
        var cut = RenderWithSection(s => s
            .Add(x => x.Id, "s1")
            .Add(x => x.Row, 2)
            .Add(x => x.RowSpan, 2));

        var style = cut.Find("[data-slot='container-grid-section']").GetAttribute("style");
        style.ShouldContain("grid-row:2 / span 2");
    }

    [Fact(DisplayName = "UAC04: Resizable ContainerGridSection renders resize handles")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ResizableRendersHandles()
    {
        var cut = RenderWithSection(s => s
            .Add(x => x.Id, "s1")
            .Add(x => x.Resizable, true));

        cut.FindAll(".lt-container-grid-handle").Count.ShouldBe(3);
    }

    [Fact(DisplayName = "UAC05: Non-resizable ContainerGridSection renders no resize handles")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NonResizableNoHandles()
    {
        var cut = RenderWithSection(s => s
            .Add(x => x.Id, "s1")
            .Add(x => x.Resizable, false));

        cut.FindAll(".lt-container-grid-handle").Count.ShouldBe(0);
    }
}

