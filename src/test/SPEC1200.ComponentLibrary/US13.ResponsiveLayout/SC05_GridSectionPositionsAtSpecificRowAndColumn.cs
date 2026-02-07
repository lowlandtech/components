using Bunit;
using LowlandTech.Components.Components.Grid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "05")]
public class SC05_GridSectionPositionsAtSpecificRowAndColumn
{
    [Fact(DisplayName = "GridSection starts at specified row and column")]
    [Trait(Spec.UAC, "01")]
    public void GridSection_StartsAtSpecifiedPosition()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 2);
                builder.AddAttribute(2, "Column", 3);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        section.ShouldNotBeNull();

        var style = section.GetAttribute("style");
        style.ShouldNotBeNull();
        style.ShouldContain("grid-row: 2");
        style.ShouldContain("grid-column: 3");
    }

    [Fact(DisplayName = "GridSection applies correct grid-area CSS")]
    [Trait(Spec.UAC, "02")]
    public void GridSection_AppliesCorrectGridAreaCSS()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 3);
                builder.AddAttribute(2, "Column", 4);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-row: 3");
        style.ShouldContain("grid-column: 4");
    }

    [Fact(DisplayName = "GridSection renders at default position (1, 1)")]
    [Trait(Spec.UAC, "03")]
    public void GridSection_RendersAtDefaultPosition()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.CloseComponent();
            }));

        var section = cut.Find("[data-grid-section]");
        var style = section.GetAttribute("style");

        style.ShouldContain("grid-row: 1");
        style.ShouldContain("grid-column: 1");
        style.ShouldContain("grid-row: 1 / span 1");
        style.ShouldContain("grid-column: 1 / span 1");
    }

    [Fact(DisplayName = "Multiple GridSections render at their positions")]
    [Trait(Spec.UAC, "04")]
    public void GridSection_MultipleSectionsRenderAtPositions()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 1);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(3);
                builder.AddAttribute(4, "Row", 3);
                builder.AddAttribute(5, "Column", 2);
                builder.CloseComponent();
            }));

        var sections = cut.FindAll("[data-grid-section]");
        sections.Count.ShouldBe(2);

        var firstStyle = sections[0].GetAttribute("style");
        firstStyle.ShouldContain("grid-row: 1");
        firstStyle.ShouldContain("grid-column: 1");

        var secondStyle = sections[1].GetAttribute("style");
        secondStyle.ShouldContain("grid-row: 3");
        secondStyle.ShouldContain("grid-column: 2");
    }
}
