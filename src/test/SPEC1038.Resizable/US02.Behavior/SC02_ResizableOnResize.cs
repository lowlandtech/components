using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Resizable;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US02.Behavior;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Resizable onResize callback fires during resize")]
public class SC02_ResizableOnResize : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resizable has OnResize parameter")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_HasOnResizeParameter()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.Add(x => x.OnResize, EventCallback.Factory.Create<ResizeEventArgs>(this, args => { }));
        });

        // OnResize parameter exists
        cut.Instance.OnResize.HasDelegate.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: Resizable has OnResizeStart parameter")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasOnResizeStartParameter()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.Add(x => x.OnResizeStart, EventCallback.Factory.Create<ResizeEventArgs>(this, args => { }));
        });

        cut.Instance.OnResizeStart.HasDelegate.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Resizable has OnResizeEnd parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_HasOnResizeEndParameter()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.Add(x => x.OnResizeEnd, EventCallback.Factory.Create<ResizeEventArgs>(this, args => { }));
        });

        cut.Instance.OnResizeEnd.HasDelegate.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: ResizeEventArgs has PanelId property")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ResizeEventArgsHasPanelId()
    {
        var args = new ResizeEventArgs
        {
            PanelId = "panel-1",
            Size = 50,
            Direction = "horizontal"
        };

        args.PanelId.ShouldBe("panel-1");
        args.Size.ShouldBe(50);
        args.Direction.ShouldBe("horizontal");
    }

    [Fact(DisplayName = "UAC05: ResizablePanel has OnResize parameter")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_PanelHasOnResizeParameter()
    {
        var cut = Ctx.Render<ResizablePanel>(p =>
        {
            p.Add(x => x.OnResize, EventCallback.Factory.Create<ResizeEventArgs>(this, args => { }));
        });

        cut.Instance.OnResize.HasDelegate.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC06: ResizeEventArgs Group property exists")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ResizeEventArgsGroupProperty()
    {
        var args = new ResizeEventArgs
        {
            PanelId = "panel-1",
            Size = 50,
            Group = "group1"
        };

        args.Group.ShouldBe("group1");
    }
}
