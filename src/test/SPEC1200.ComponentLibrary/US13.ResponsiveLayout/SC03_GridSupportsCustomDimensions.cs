using Bunit;
using LowlandTech.Components.Components.Grid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "03")]
public class SC03_GridSupportsCustomDimensions
{
    [Fact(DisplayName = "Grid renders a 6x12 layout")]
    [Trait(Spec.UAC, "01")]
    public void Grid_Renders6x12Layout()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Row, 6)
            .Add(p => p.Column, 12));

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldNotBeNull();
        style.ShouldContain("grid-template-rows: repeat(6, minmax(0, 1fr));");
        style.ShouldContain("grid-template-columns: repeat(12, minmax(0, 1fr));");
    }

    [Fact(DisplayName = "Sections can be positioned anywhere within the 6x12 bounds")]
    [Trait(Spec.UAC, "02")]
    public void Grid_SectionsCanBePositionedAnywhereWithinBounds()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Row, 6)
            .Add(p => p.Column, 12)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 5);
                builder.AddAttribute(2, "Column", 10);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(3);
                builder.AddAttribute(4, "Row", 6);
                builder.AddAttribute(5, "Column", 12);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(6);
                builder.AddAttribute(7, "Row", 1);
                builder.AddAttribute(8, "Column", 1);
                builder.CloseComponent();
            }));

        var sections = cut.FindAll("[data-grid-section]");
        sections.Count.ShouldBe(3);

        var section1Style = sections[0].GetAttribute("style");
        section1Style.ShouldContain("grid-row: 5");
        section1Style.ShouldContain("grid-column: 10");

        var section2Style = sections[1].GetAttribute("style");
        section2Style.ShouldContain("grid-row: 6");
        section2Style.ShouldContain("grid-column: 12");

        var section3Style = sections[2].GetAttribute("style");
        section3Style.ShouldContain("grid-row: 1");
        section3Style.ShouldContain("grid-column: 1");
    }

    [Fact(DisplayName = "Grid template styles update when dimensions change")]
    [Trait(Spec.UAC, "03")]
    public void Grid_TemplateStylesUpdateWhenDimensionsChange()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Row, 8)
            .Add(p => p.Column, 16));

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldContain("grid-template-rows: repeat(8, minmax(0, 1fr));");
        style.ShouldContain("grid-template-columns: repeat(16, minmax(0, 1fr));");
    }
}
