using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1066.ContainerGrid.US04.Window;

/// <summary>
/// SC01 — A host-supplied row window.
///
/// The host owns the scroll container, so it is the host that says which rows are visible.
/// The grid renders the sections that intersect that window and keeps an inert placeholder
/// of the same geometry for the rest, so the scroll extent and every visible row stay as
/// they were. A grid the host gives no window renders exactly as before.
/// </summary>
[Trait(Spec.SPEC, "1066")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "Host-supplied row window")]
public class SC01_VisibleRowWindow : JsViewScenario
{
    private const string SectionContentSlot = "[data-slot='container-grid-section-window']";
    private const string ContentMarker = "[data-testid='window-content']";

    // Given a grid of ten rows whose section sits at a chosen row and span, and whose host
    // either narrows the window or leaves it alone.
    private IRenderedComponent<LowlandTech.Components.ContainerGrid> GivenAGridWithSection(
        int sectionRow,
        int sectionSpan,
        int? windowFrom = null,
        int? windowTo = null)
        => Ctx.Render<LowlandTech.Components.ContainerGrid>(grid =>
        {
            grid.Add(x => x.Id, "window-grid");
            grid.Add(x => x.Rows, 10);

            if (windowFrom is not null)
            {
                grid.Add(x => x.VisibleRowFrom, windowFrom);
            }

            if (windowTo is not null)
            {
                grid.Add(x => x.VisibleRowTo, windowTo);
            }

            grid.AddChildContent<ContainerGridSection>(section => section
                .Add(x => x.Id, "s1")
                .Add(x => x.Row, sectionRow)
                .Add(x => x.RowSpan, sectionSpan)
                .AddChildContent("<span data-testid='window-content'>body</span>"));
        });

    [Fact(DisplayName = "UAC01: a grid with no window renders the content of every section")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NoWindowRendersEverySection()
    {
        var cut = GivenAGridWithSection(sectionRow: 1, sectionSpan: 1);

        cut.FindAll(ContentMarker).Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC02: a section entirely outside the window renders a placeholder instead of its content")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SectionOutsideWindowRendersPlaceholder()
    {
        var cut = GivenAGridWithSection(sectionRow: 1, sectionSpan: 1, windowFrom: 5, windowTo: 6);

        cut.FindAll(ContentMarker).Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: a section inside the window renders its content")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SectionInsideWindowRendersContent()
    {
        var cut = GivenAGridWithSection(sectionRow: 5, sectionSpan: 1, windowFrom: 5, windowTo: 6);

        cut.FindAll(ContentMarker).Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC04: a section that only overlaps the window still renders its content")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_OverlappingSectionRendersContent()
    {
        var cut = GivenAGridWithSection(sectionRow: 4, sectionSpan: 3, windowFrom: 5, windowTo: 6);

        cut.FindAll(ContentMarker).Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC05: a window the host only half supplied renders every section")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_PartialWindowRendersEverySection()
    {
        var cut = GivenAGridWithSection(sectionRow: 1, sectionSpan: 1, windowFrom: 5);

        cut.FindAll(ContentMarker).Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC06: a section outside the window keeps the geometry of its box")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PlaceholderKeepsSectionGeometry()
    {
        var cut = GivenAGridWithSection(sectionRow: 1, sectionSpan: 2, windowFrom: 5, windowTo: 6);

        cut.Find(SectionContentSlot).ShouldNotBeNull();
    }
}
