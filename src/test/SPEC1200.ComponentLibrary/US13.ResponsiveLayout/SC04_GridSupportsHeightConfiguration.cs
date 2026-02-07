using Bunit;
using LowlandTech.Components.Components.Grid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US13.ResponsiveLayout;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "04")]
public class SC04_GridSupportsHeightConfiguration
{
    [Fact(DisplayName = "Grid container has specified height")]
    [Trait(Spec.UAC, "01")]
    public void Grid_ContainerHasSpecifiedHeight()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Height, "600px"));

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldNotBeNull();
        style.ShouldContain("height: 600px;");
    }

    [Fact(DisplayName = "Grid container supports viewport height units")]
    [Trait(Spec.UAC, "02")]
    public void Grid_ContainerSupportsViewportHeightUnits()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Height, "80vh"));

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldNotBeNull();
        style.ShouldContain("height: 80vh;");
    }

    [Fact(DisplayName = "Grid container without height property has no height style")]
    [Trait(Spec.UAC, "03")]
    public void Grid_ContainerWithoutHeightHasNoHeightStyle()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>();

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldNotBeNull();
        style.ShouldNotContain("height:");
    }

    [Fact(DisplayName = "Grid sections fill vertically within specified height")]
    [Trait(Spec.UAC, "04")]
    public void Grid_SectionsFillVerticallyWithinHeight()
    {
        using var ctx = new BunitContext();

        var cut = ctx.Render<Grid>(parameters => parameters
            .Add(p => p.Height, "500px")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<GridSection>(0);
                builder.AddAttribute(1, "Row", 1);
                builder.AddAttribute(2, "Column", 1);
                builder.CloseComponent();
            }));

        var gridElement = cut.Find(".grid-layout");
        var style = gridElement.GetAttribute("style");

        style.ShouldContain("height: 500px;");
        style.ShouldContain("grid-template-rows: repeat(4, minmax(0, 1fr));");
    }
}
