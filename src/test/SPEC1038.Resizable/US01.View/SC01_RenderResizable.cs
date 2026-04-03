using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US01.View;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Resizable component")]
public class SC01_RenderResizable : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resizable renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ResizableSut>();
        cut.Find("[data-slot='resizable-panel-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Resizable renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ResizableSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Resizable renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ResizableSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='resizable-panel-group']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Resizable renders with default horizontal direction")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithDefaultHorizontalDirection()
    {
        var cut = Ctx.Render<ResizableSut>();
        cut.Find("[data-orientation='horizontal']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Resizable renders with vertical direction")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithVerticalDirection()
    {
        var cut = Ctx.Render<ResizableSut>(p => p.Add(x => x.Direction, "vertical"));
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }
}
