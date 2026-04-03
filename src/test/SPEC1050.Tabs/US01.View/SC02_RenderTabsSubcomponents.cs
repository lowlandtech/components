using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tabs;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US01.View;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Tabs subcomponents")]
public class SC02_RenderTabsSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: TabsList renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TabsListRendersDataSlot()
    {
        var cut = Ctx.Render<TabsList>();
        cut.Find("[data-slot='tabs-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: TabsList has tablist role")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TabsListHasTablistRole()
    {
        var cut = Ctx.Render<TabsList>();
        cut.Find("[role='tablist']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: TabsList inherits orientation from parent")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_TabsListInheritsOrientation()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.Orientation, "vertical");
            p.AddChildContent<TabsList>();
        });
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: TabsTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_TabsTriggerRendersDataSlot()
    {
        var cut = Ctx.Render<TabsTrigger>(p => p.Add(x => x.Value, "tab1"));
        cut.Find("[data-slot='tabs-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: TabsTrigger has role tab")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_TabsTriggerHasRoleTab()
    {
        var cut = Ctx.Render<TabsTrigger>(p => p.Add(x => x.Value, "tab1"));
        cut.Find("[role='tab']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: TabsTrigger renders ChildContent")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_TabsTriggerRendersChildContent()
    {
        var cut = Ctx.Render<TabsTrigger>(p =>
        {
            p.Add(x => x.Value, "tab1");
            p.AddChildContent("Tab Label");
        });
        cut.Markup.ShouldContain("Tab Label");
    }

    [Fact(DisplayName = "UAC07: TabsTrigger renders with Value parameter")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_TabsTriggerRendersWithValue()
    {
        var cut = Ctx.Render<TabsTrigger>(p => p.Add(x => x.Value, "my-tab"));
        cut.Instance.Value.ShouldBe("my-tab");
    }

    [Fact(DisplayName = "UAC08: TabsContent renders when active")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_TabsContentRendersWhenActive()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(cp =>
            {
                cp.Add(x => x.Value, "tab1");
                cp.AddChildContent("Content");
            });
        });
        cut.Markup.ShouldContain("Content");
        cut.Find("[data-slot='tabs-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: TabsContent has tabpanel role")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_TabsContentHasTabpanelRole()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(cp =>
            {
                cp.Add(x => x.Value, "tab1");
            });
        });
        cut.Find("[role='tabpanel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: TabsTrigger disabled state")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_TabsTriggerDisabledState()
    {
        var cut = Ctx.Render<TabsTrigger>(p =>
        {
            p.Add(x => x.Value, "tab1");
            p.Add(x => x.Disabled, true);
        });
        var button = cut.Find("[data-slot='tabs-trigger']");
        button.HasAttribute("disabled").ShouldBeTrue("disabled attribute should be present");
    }
}
