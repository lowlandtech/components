using Bunit;
using LowlandTech.Components.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US02.Behavior;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "03")]
[UserStory("02", "ResizablePanel respects MinSize and MaxSize")]
public class SC03_ResizablePanelSizeConstraints : ViewScenario
{
    [Fact(DisplayName = "UAC01: ResizablePanel has MinSize parameter")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_HasMinSizeParameter()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.Add(x => x.MinSize, 20));
        cut.Instance.MinSize.ShouldBe(20);
    }

    [Fact(DisplayName = "UAC02: ResizablePanel has MaxSize parameter")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasMaxSizeParameter()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.Add(x => x.MaxSize, 80));
        cut.Instance.MaxSize.ShouldBe(80);
    }

    [Fact(DisplayName = "UAC03: ResizablePanel MinSize defaults to 10")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_MinSizeDefaultsTo10()
    {
        var cut = Ctx.Render<ResizablePanel>();
        cut.Instance.MinSize.ShouldBe(10);
    }

    [Fact(DisplayName = "UAC04: ResizablePanel MaxSize defaults to 100")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_MaxSizeDefaultsTo100()
    {
        var cut = Ctx.Render<ResizablePanel>();
        cut.Instance.MaxSize.ShouldBe(100);
    }

    [Fact(DisplayName = "UAC05: ResizablePanel DefaultSize defaults to 50")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DefaultSizeDefaultsTo50()
    {
        var cut = Ctx.Render<ResizablePanel>();
        cut.Instance.DefaultSize.ShouldBe(50);
    }

    [Fact(DisplayName = "UAC06: ResizablePanel Id parameter works")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_IdParameterWorks()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.Add(x => x.Id, "custom-panel"));
        cut.Instance.Id.ShouldBe("custom-panel");
    }

    [Fact(DisplayName = "UAC07: ResizablePanel generates unique PanelId")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_GeneratesUniquePanelId()
    {
        var cut1 = Ctx.Render<ResizablePanel>();
        var cut2 = Ctx.Render<ResizablePanel>();

        cut1.Instance.PanelId.ShouldNotBe(cut2.Instance.PanelId);
    }

    [Fact(DisplayName = "UAC08: ResizablePanel respects custom PanelId from Id parameter")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RespectsCustomPanelId()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.Add(x => x.Id, "my-panel"));
        cut.Instance.PanelId.ShouldBe("my-panel");
    }
}
