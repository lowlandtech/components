using Bunit;
using LowlandTech.Components.Components.Grid;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "01")]
public class SC01_GridRendersBasicConfiguration
{
    [Fact(DisplayName = "Grid renders a 4x4 grid container with default configuration")]
    [Trait(Spec.UAC, "01")]
    public void Grid_Renders4x4GridContainerWithDefaults()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>();

        var gridElement = cut.Find(".grid-layout");
        gridElement.ShouldNotBeNull();

        var style = gridElement.GetAttribute("style");
        style.ShouldNotBeNull();
        style.ShouldContain("grid-template-rows: repeat(4, minmax(0, 1fr));");
        style.ShouldContain("grid-template-columns: repeat(4, minmax(0, 1fr));");
    }

    [Fact(DisplayName = "Grid renders with custom row and column properties")]
    [Trait(Spec.UAC, "02")]
    public void Grid_RendersWithCustomRowAndColumnProperties()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Row, 6)
            .Add(p => p.Column, 8));

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldContain("grid-template-rows: repeat(6, minmax(0, 1fr));");
        style.ShouldContain("grid-template-columns: repeat(8, minmax(0, 1fr));");
    }

    [Fact(DisplayName = "Grid generates appropriate CSS grid styles")]
    [Trait(Spec.UAC, "03")]
    public void Grid_GeneratesAppropriateCSSGridStyles()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>();

        var gridElement = cut.Find(".grid-layout");
        var classes = gridElement.GetAttribute("class");

        classes.ShouldContain("grid-layout");
    }

    [Fact(DisplayName = "Grid is ready to accept GridSection children")]
    [Trait(Spec.UAC, "04")]
    public void Grid_IsReadyToAcceptGridSectionChildren()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.CloseComponent();
            }));

        var gridSection = cut.Find("[data-grid-section]");
        gridSection.ShouldNotBeNull();
    }
}
