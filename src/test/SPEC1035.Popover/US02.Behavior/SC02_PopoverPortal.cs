using Bunit;
using PopoverSut = LowlandTech.Components.Popover.Popover;
using LowlandTech.Components.Popover;

namespace LowlandTech.Components.Tests.SPEC1035.Popover.US02.Behavior;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Popover Portal renders outside parent")]
public class SC02_PopoverPortal : ViewScenario
{
    [Fact(DisplayName = "UAC01: Popover Portal parameter is available")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PortalParameterAvailable()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.Portal, true);
        });

        cut.Instance.Portal.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: PopoverContent Portal parameter is available")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ContentPortalParameterAvailable()
    {
        var cut = Ctx.Render<PopoverContent>(p =>
        {
            p.Add(x => x.Portal, true);
        });

        cut.Instance.Portal.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Popover Portal inherits from parent")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PortalInheritsFromParent()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.Portal, true);
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Portaled Content");
            });
        });

        cut.Instance.Portal.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: PopoverContent has Portal parameter default false")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_PortalDefaultFalse()
    {
        var cut = Ctx.Render<PopoverContent>();

        cut.Instance.Portal.ShouldBeFalse();
    }
}
