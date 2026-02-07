using Bunit;
using LowlandTech.Components.Components.Grid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "06")]
public class SC06_GridSectionSupportsColumnSpan
{
    [Fact(DisplayName = "GridSection spans specified number of columns")]
    [Trait(Spec.UAC, "01")]
    public void GridSection_SpansColumns()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 1);
                builder.AddAttribute(3, "ColSpan", 2);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-column: 1 / span 2");
    }

    [Fact(DisplayName = "GridSection occupies correct grid cells for column span")]
    [Trait(Spec.UAC, "02")]
    public void GridSection_OccupiesCorrectCellsForColumnSpan()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 2);
                builder.AddAttribute(3, "ColSpan", 3);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-column: 2 / span 3");
        style.ShouldContain("grid-row: 1 / span 1");
    }

    [Fact(DisplayName = "GridSection with column span 1 occupies single column")]
    [Trait(Spec.UAC, "03")]
    public void GridSection_ColumnSpan1OccupiesSingleColumn()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 1);
                builder.AddAttribute(3, "ColSpan", 1);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-column: 1 / span 1");
    }
}
