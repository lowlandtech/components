using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US02.Behavior;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "01")]
[UserStory("02", "ResizableHandle Group works with multiple handles")]
public class SC01_ResizableHandleGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: ResizableHandle has Group parameter")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_HasGroupParameter()
    {
        var cut = Ctx.Render<ResizableHandle>(p => p.Add(x => x.Group, "group1"));
        cut.Instance.Group.ShouldBe("group1");
    }

    [Fact(DisplayName = "UAC02: ResizableHandle renders data-group attribute")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersDataGroup()
    {
        var cut = Ctx.Render<ResizableHandle>(p => p.Add(x => x.Group, "group1"));
        var handle = cut.Find("[data-slot='resizable-handle']");
        handle.GetAttribute("data-group").ShouldBe("group1");
    }

    [Fact(DisplayName = "UAC03: ResizableHandle has HandleIndex parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_HasHandleIndexParameter()
    {
        var cut = Ctx.Render<ResizableHandle>(p => p.Add(x => x.HandleIndex, 0));
        cut.Instance.HandleIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC04: ResizableHandle Group parameter is passed correctly")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_GroupParameterPassedCorrectly()
    {
        var cut = Ctx.Render<ResizableSut>(p =>
        {
            p.AddChildContent<ResizableHandle>(h => h.Add(x => x.Group, "g1"));
            p.AddChildContent<ResizableHandle>(h => h.Add(x => x.Group, "g2"));
        });

        cut.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC05: ResizableHandle Group can be empty")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_GroupCanBeEmpty()
    {
        var cut = Ctx.Render<ResizableHandle>();
        cut.Instance.Group.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC06: ResizableHandle BeforePanel and AfterPanel parameters work")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_BeforeAfterPanelParameters()
    {
        var panel1 = new ResizablePanel();
        var panel2 = new ResizablePanel();

        var cut = Ctx.Render<ResizableHandle>(p =>
        {
            p.Add(x => x.BeforePanel, panel1);
            p.Add(x => x.AfterPanel, panel2);
        });

        cut.Instance.BeforePanel.ShouldBe(panel1);
        cut.Instance.AfterPanel.ShouldBe(panel2);
    }
}
