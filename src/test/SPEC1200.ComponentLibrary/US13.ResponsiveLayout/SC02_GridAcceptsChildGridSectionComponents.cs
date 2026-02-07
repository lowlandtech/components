using Bunit;
using LowlandTech.Components.Components.Grid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "02")]
public class SC02_GridAcceptsChildGridSectionComponents
{
    [Fact(DisplayName = "Each GridSection registers with the parent Grid")]
    [Trait(Spec.UAC, "01")]
    public void Grid_EachGridSectionRegistersWithParent()
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
                builder.AddAttribute(4, "Row", 1);
                builder.AddAttribute(5, "Column", 2);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(6);
                builder.AddAttribute(7, "Row", 2);
                builder.AddAttribute(8, "Column", 1);
                builder.CloseComponent();
            }));

        var sections = cut.FindAll("[data-grid-section]");
        sections.Count.ShouldBe(3);
    }

    [Fact(DisplayName = "Sections position according to their Row and Column properties")]
    [Trait(Spec.UAC, "02")]
    public void Grid_SectionsPositionAccordingToRowAndColumn()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 2);
                builder.AddAttribute(2, "Column", 3);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(3);
                builder.AddAttribute(4, "Row", 1);
                builder.AddAttribute(5, "Column", 1);
                builder.CloseComponent();
            }));

        var sections = cut.FindAll("[data-grid-section]");
        sections.Count.ShouldBe(2);

        var section1 = sections.First(s => s.GetAttribute("style")?.Contains("grid-row: 2") ?? false);
        var section2 = sections.First(s => s.GetAttribute("style")?.Contains("grid-row: 1") ?? false);

        section1.ShouldNotBeNull();
        section2.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Grid tracks all registered sections")]
    [Trait(Spec.UAC, "03")]
    public void Grid_TracksAllRegisteredSections()
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
                builder.AddAttribute(4, "Row", 1);
                builder.AddAttribute(5, "Column", 2);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(6);
                builder.AddAttribute(7, "Row", 2);
                builder.AddAttribute(8, "Column", 1);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(9);
                builder.AddAttribute(10, "Row", 2);
                builder.AddAttribute(11, "Column", 2);
                builder.CloseComponent();

                builder.OpenComponent<GridSection>(12);
                builder.AddAttribute(13, "Row", 3);
                builder.AddAttribute(14, "Column", 1);
                builder.CloseComponent();
            }));

        var sections = cut.FindAll("[data-grid-section]");
        sections.Count.ShouldBe(5);
    }
}
