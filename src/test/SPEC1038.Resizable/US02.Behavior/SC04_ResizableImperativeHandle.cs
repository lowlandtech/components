using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US02.Behavior;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "04")]
[UserStory("02", "Resizable imperative handle can resize programmatically")]
public class SC04_ResizableImperativeHandle : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resizable has SetPanelSizeAsync method")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_HasSetPanelSizeAsyncMethod()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.AddChildContent<ResizablePanel>(panel => panel.Add(x => x.Id, "panel1"));
        });

        // Method exists on component instance
        cut.Instance.GetType().GetMethod("SetPanelSizeAsync").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Resizable has GetPanelSize method")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasGetPanelSizeMethod()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.AddChildContent<ResizablePanel>(panel => panel.Add(x => x.Id, "panel1"));
        });

        cut.Instance.GetType().GetMethod("GetPanelSize").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: ResizablePanel has SetSizeAsync method")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PanelHasSetSizeAsyncMethod()
    {
        var cut = Ctx.Render<ResizablePanel>();
        cut.Instance.GetType().GetMethod("SetSizeAsync").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: ResizablePanel GetPanelSize returns correct size")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_GetPanelSizeReturnsCorrectSize()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.AddChildContent<ResizablePanel>(panel =>
            {
                panel.Add(x => x.Id, "panel1");
                panel.Add(x => x.DefaultSize, 60);
            });
        });

        var size = cut.Instance.GetPanelSize("panel1");
        size.ShouldBe(60);
    }

    [Fact(DisplayName = "UAC05: ResizablePanel GetPanelSize returns 0 for unknown panel")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_GetPanelSizeReturns0ForUnknownPanel()
    {
        var cut = Ctx.Render<ResizableSut>();

        var size = cut.Instance.GetPanelSize("nonexistent");
        size.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: ResizablePanel UpdateSize method exists")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_UpdateSizeMethodExists()
    {
        var cut = Ctx.Render<ResizablePanel>();
        cut.Instance.GetType().GetMethod("UpdateSize", new[] { typeof(int) }).ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: ResizablePanel CurrentSize is publicly accessible")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CurrentSizeIsAccessible()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.Add(x => x.DefaultSize, 75));
        cut.Instance.CurrentSize.ShouldBe(75);
    }
}
