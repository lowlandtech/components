using LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US09.EmptyState;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "09")]
[Trait(Spec.SC, "01")]
[UserStory("09", "Render EmptyStateWithAction block")]
public class SC01_RenderEmptyStateWithAction : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<EmptyStateWithAction>();
        cut.Find("[data-slot='empty-state-action']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and description")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndDescription()
    {
        var cut = Ctx.Render<EmptyStateWithAction>(p =>
        {
            p.Add(x => x.Title, "Nothing here");
            p.Add(x => x.Description, "Add your first item.");
        });
        cut.Markup.ShouldContain("Nothing here");
        cut.Markup.ShouldContain("Add your first item.");
    }

    [Fact(DisplayName = "UAC03: renders icon when IconPath provided")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersIcon()
    {
        var cut = Ctx.Render<EmptyStateWithAction>(p => p.Add(x => x.IconPath, "<circle cx='12' cy='12' r='10'/>"));
        cut.Find("svg").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: renders child content")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersChildContent()
    {
        var cut = Ctx.Render<EmptyStateWithAction>(p => p.AddChildContent("<button>Create</button>"));
        cut.Markup.ShouldContain("Create");
    }

    [Fact(DisplayName = "UAC05: does not inject feature-state services")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<EmptyStateWithAction>();
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("EmptyStateWithAction must not expose stateful properties");
    }
}
