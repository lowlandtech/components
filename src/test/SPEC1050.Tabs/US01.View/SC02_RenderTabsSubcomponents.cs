using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tabs;

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

    [Fact(DisplayName = "UAC11: TabsTrigger has id attribute")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_TabsTriggerHasId()
    {
        var cut = Ctx.Render<TabsTrigger>(p => p.Add(x => x.Value, "my-tab"));
        var trigger = cut.Find("[data-slot='tabs-trigger']");
        trigger.GetAttribute("id").ShouldBe("tabs-trigger-my-tab");
    }

    [Fact(DisplayName = "UAC12: TabsTrigger has aria-controls attribute")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_TabsTriggerHasAriaControls()
    {
        var cut = Ctx.Render<TabsTrigger>(p => p.Add(x => x.Value, "my-tab"));
        var trigger = cut.Find("[data-slot='tabs-trigger']");
        trigger.GetAttribute("aria-controls").ShouldBe("tabs-content-my-tab");
    }

    [Fact(DisplayName = "UAC13: TabsContent has id matching trigger aria-controls")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_TabsContentHasId()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(cp =>
            {
                cp.Add(x => x.Value, "tab1");
            });
        });
        var content = cut.Find("[data-slot='tabs-content']");
        content.GetAttribute("id").ShouldBe("tabs-content-tab1");
    }

    [Fact(DisplayName = "UAC14: TabsContent has aria-labelledby attribute")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_TabsContentHasAriaLabelledBy()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(cp =>
            {
                cp.Add(x => x.Value, "tab1");
            });
        });
        var content = cut.Find("[data-slot='tabs-content']");
        content.GetAttribute("aria-labelledby").ShouldBe("tabs-trigger-tab1");
    }

    [Fact(DisplayName = "UAC15: TabsContent has tabindex 0")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_TabsContentHasTabindex()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(cp =>
            {
                cp.Add(x => x.Value, "tab1");
            });
        });
        var content = cut.Find("[data-slot='tabs-content']");
        content.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC16: Active trigger has tabindex 0")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_ActiveTriggerHasTabindexZero()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsList>(list =>
            {
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab1");
                });
            });
        });
        var trigger = cut.Find("[data-slot='tabs-trigger']");
        trigger.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC17: Inactive trigger has tabindex -1")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_InactiveTriggerHasTabindexNegativeOne()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsList>(list =>
            {
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab1");
                });
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab2");
                });
            });
        });
        var triggers = cut.FindAll("[data-slot='tabs-trigger']");
        triggers[1].GetAttribute("tabindex").ShouldBe("-1");
    }

    [Fact(DisplayName = "UAC18: TabsList default variant applies default data-variant")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_TabsListDefaultVariant()
    {
        var cut = Ctx.Render<TabsList>();
        var list = cut.Find("[data-slot='tabs-list']");
        list.GetAttribute("data-variant").ShouldBe("default");
    }

    [Fact(DisplayName = "UAC19: TabsList line variant applies line styling")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_TabsListLineVariant()
    {
        var cut = Ctx.Render<TabsList>(p => p.Add(x => x.Variant, "line"));
        var list = cut.Find("[data-slot='tabs-list']");
        list.GetAttribute("data-variant").ShouldBe("line");
        list.ClassList.ShouldContain("bg-transparent");
    }

    [Fact(DisplayName = "UAC20: TabsTrigger active in line variant gets primary underline indicator")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_LineVariantActiveTriggerHasUnderlineIndicator()
    {
        // Line variant flattens the TabsList background to transparent so it
        // matches a peer flat header (e.g. the canvas designer's left "Canvases"
        // header). Without a strong active-state indicator, the active tab is
        // indistinguishable from the inactive tab — the user can't tell which
        // panel they're looking at. This UAC pins the primary-colored bottom
        // border that signals the active tab in line mode.
        var cut = Ctx.Render<TabsSut>(p => p
            .Add(x => x.Value, "first")
            .AddChildContent<TabsList>(list => list
                .Add(l => l.Variant, "line")
                .AddChildContent<TabsTrigger>(t => t
                    .Add(tt => tt.Value, "first")
                    .AddChildContent("First"))
                .AddChildContent<TabsTrigger>(t => t
                    .Add(tt => tt.Value, "second")
                    .AddChildContent("Second"))));

        var triggers = cut.FindAll("[data-slot='tabs-trigger']");
        var activeClasses = triggers[0].ClassList;
        var inactiveClasses = triggers[1].ClassList;

        activeClasses.ShouldContain("border-b-2",
            customMessage:
                "Active line-variant trigger must carry a 2px bottom border " +
                "to draw the underline indicator. Drop it and the active tab " +
                "is indistinguishable from inactive in line mode.");
        activeClasses.ShouldContain("border-primary",
            customMessage:
                "Underline indicator must use the primary brand colour so it " +
                "reads as the active accent against the muted-foreground text " +
                "of inactive triggers.");
        inactiveClasses.ShouldNotContain("border-primary",
            customMessage:
                "Inactive triggers must NOT carry the primary indicator — " +
                "otherwise every tab looks active.");
    }

    [Fact(DisplayName = "UAC21: TabsList can stretch its back panel to fill its parent")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_TabsListCanStretch()
    {
        var cut = Ctx.Render<TabsList>(p => p.Add(x => x.Stretch, true));

        var list = cut.Find("[data-slot='tabs-list']");
        list.GetAttribute("data-stretch").ShouldBe("true");
        list.ClassList.ShouldContain("w-full");
    }

    [Fact(DisplayName = "UAC22: TabsList supports centered trigger alignment")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_TabsListSupportsCenteredAlignment()
    {
        var cut = Ctx.Render<TabsList>(p => p.Add(x => x.Align, "center"));

        var list = cut.Find("[data-slot='tabs-list']");
        list.GetAttribute("data-align").ShouldBe("center");
        list.ClassList.ShouldContain("justify-center");
        list.ClassList.ShouldNotContain("justify-start");
    }
}
