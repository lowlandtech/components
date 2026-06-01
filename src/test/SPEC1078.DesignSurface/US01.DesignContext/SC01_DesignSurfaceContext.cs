namespace LowlandTech.Components.Tests.SPEC1078.DesignSurface.US01.DesignContext;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "DesignSurface provides a reusable design-mode context")]
public class SC01_DesignSurfaceContext : ViewScenario
{
    [Fact(DisplayName = "UAC01: DesignSurface cascades design mode to designable children")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CascadesDesignMode()
    {
        var cut = Ctx.Render<DesignSurface>(parameters => parameters
            .Add(p => p.IsDesignMode, true)
            .AddChildContent<LowlandTech.Components.Button.Button>());

        cut.Find("[data-slot='design-surface']").GetAttribute("data-design-mode").ShouldBe("true");
        cut.Find("[data-slot='button']").GetAttribute("data-design-selectable").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC02: selecting a child raises TinyDesignSelection")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RaisesSelection()
    {
        TinyDesignSelection? selection = null;
        var cut = Ctx.Render<DesignSurface>(parameters => parameters
            .Add(p => p.IsDesignMode, true)
            .Add(p => p.OnSelectionChanged, value => selection = value)
            .AddChildContent<LowlandTech.Components.Button.Button>());

        cut.Find("[data-slot='button']").Click();

        selection.ShouldNotBeNull();
        selection.ComponentType.ShouldBe(typeof(LowlandTech.Components.Button.Button));
        selection.Values.ShouldContainKey("Variant");
    }

    [Fact(DisplayName = "UAC03: selected child receives selected design marker")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_MarksSelectedChild()
    {
        var cut = Ctx.Render<DesignSurface>(parameters => parameters
            .Add(p => p.IsDesignMode, true)
            .AddChildContent<LowlandTech.Components.Button.Button>());

        cut.Find("[data-slot='button']").Click();

        cut.Find("[data-slot='button']").GetAttribute("data-design-selected").ShouldBe("true");
    }
}
