using Bunit;
using AlertSut = LowlandTech.Components.Alert;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1001.Alert.US01.View;

[Trait(Spec.SPEC, "1001")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Alert component")]
public class SC01_RenderAlert : ViewScenario
{
    [Fact(DisplayName = "UAC01: Alert renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AlertSut>();
        cut.Find("[data-slot='alert']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Alert renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<AlertSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Alert renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<AlertSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='alert']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Alert has role alert")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HasRoleAlert()
    {
        var cut = Ctx.Render<AlertSut>();
        cut.Find("[role='alert']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Alert renders with default variant")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithDefaultVariant()
    {
        var cut = Ctx.Render<AlertSut>();
        cut.Find("[data-variant='default']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Alert renders with destructive variant")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWithDestructiveVariant()
    {
        var cut = Ctx.Render<AlertSut>(p => p.Add(x => x.Variant, "destructive"));
        cut.Find("[data-variant='destructive']").ShouldNotBeNull();
    }
}
