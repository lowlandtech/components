using Bunit;
using LowlandTech.Components.Components.Grid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "08")]
public class SC08_GridSectionSupportsBothRowAndColumnSpans
{
    [Fact(DisplayName = "GridSection starts at specified position with both spans")]
    [Trait(Spec.UAC, "01")]
    public void GridSection_StartsAtPositionWithBothSpans()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 1);
                builder.AddAttribute(3, "RowSpan", 2);
                builder.AddAttribute(4, "ColSpan", 3);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-row: 1 / span 2");
        style.ShouldContain("grid-column: 1 / span 3");
    }

    [Fact(DisplayName = "GridSection spans both rows and columns correctly")]
    [Trait(Spec.UAC, "02")]
    public void GridSection_SpansBothRowsAndColumnsCorrectly()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 2);
                builder.AddAttribute(2, "Column", 3);
                builder.AddAttribute(3, "RowSpan", 4);
                builder.AddAttribute(4, "ColSpan", 5);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-row: 2 / span 4");
        style.ShouldContain("grid-column: 3 / span 5");
    }

    [Fact(DisplayName = "GridSection with spans occupies 6 total grid cells")]
    [Trait(Spec.UAC, "03")]
    public void GridSection_WithSpansOccupiesSixCells()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 1);
                builder.AddAttribute(3, "RowSpan", 2);
                builder.AddAttribute(4, "ColSpan", 3);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-row: 1 / span 2");
        style.ShouldContain("grid-column: 1 / span 3");
    }
}
