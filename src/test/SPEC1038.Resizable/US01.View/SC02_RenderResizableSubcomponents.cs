using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US01.View;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Resizable subcomponents")]
public class SC02_RenderResizableSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: ResizablePanel renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PanelRendersDataSlot()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.AddChildContent("Panel"));
        cut.Find("[data-slot='resizable-panel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ResizablePanel renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_PanelRendersChildContent()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.AddChildContent("Panel Content"));
        cut.Markup.ShouldContain("Panel Content");
    }

    [Fact(DisplayName = "UAC03: ResizablePanel has unique panel ID")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PanelHasUniqueId()
    {
        var cut = Ctx.Render<ResizablePanel>();
        var panel = cut.Find("[data-slot='resizable-panel']");
        panel.GetAttribute("data-panel").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: ResizablePanel renders with default size")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_PanelRendersWithDefaultSize()
    {
        var cut = Ctx.Render<ResizablePanel>();
        cut.Instance.DefaultSize.ShouldBe(50);
    }

    [Fact(DisplayName = "UAC05: ResizablePanel renders with custom size")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_PanelRendersWithCustomSize()
    {
        var cut = Ctx.Render<ResizablePanel>(p => p.Add(x => x.DefaultSize, 75));
        cut.Instance.DefaultSize.ShouldBe(75);
    }

    [Fact(DisplayName = "UAC06: ResizablePanel renders with min/max size")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PanelRendersWithMinMaxSize()
    {
        var cut = Ctx.Render<ResizablePanel>(p =>
        {
            p.Add(x => x.MinSize, 20);
            p.Add(x => x.MaxSize, 80);
        });
        cut.Instance.MinSize.ShouldBe(20);
        cut.Instance.MaxSize.ShouldBe(80);
    }

    [Fact(DisplayName = "UAC07: ResizableHandle renders data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_HandleRendersDataSlot()
    {
        var cut = Ctx.Render<ResizableHandle>();
        cut.Find("[data-slot='resizable-handle']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: ResizableHandle has separator role")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_HandleHasSeparatorRole()
    {
        var cut = Ctx.Render<ResizableHandle>();
        cut.Find("[role='separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: ResizableHandle is focusable")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_HandleIsFocusable()
    {
        var cut = Ctx.Render<ResizableHandle>();
        var handle = cut.Find("[data-slot='resizable-handle']");
        handle.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC10: ResizableHandle inherits orientation from parent")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_HandleInheritsOrientation()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.Add(x => x.Direction, "vertical");
            p.AddChildContent<ResizableHandle>();
        });
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: ResizableHandle renders with handle indicator")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_HandleRendersWithIndicator()
    {
        var cut = Ctx.Render<ResizableHandle>(p => p.Add(x => x.WithHandle, true));
        cut.Find(".resizable-handle-indicator").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: Resizable composition works together")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_CompositionWorksTogether()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.AddChildContent<ResizablePanel>(panel =>
            {
                panel.AddChildContent("Panel 1");
            });
            p.AddChildContent<ResizableHandle>();
            p.AddChildContent<ResizablePanel>(panel =>
            {
                panel.AddChildContent("Panel 2");
            });
        });

        cut.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(2);
        cut.Find("[data-slot='resizable-handle']").ShouldNotBeNull();
    }
}
