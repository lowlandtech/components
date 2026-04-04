using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tabs;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US02.Behavior;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Tabs activation behavior")]
public class SC01_TabsActivation : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking trigger activates tab")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ClickingTriggerActivatesTab()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsList>(list =>
            {
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab1");
                    trigger.AddChildContent("Tab 1");
                });
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab2");
                    trigger.AddChildContent("Tab 2");
                });
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab1");
                content.AddChildContent("Content 1");
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab2");
                content.AddChildContent("Content 2");
            });
        });

        // Initially tab1 should be active
        cut.Markup.ShouldContain("Content 1");

        // Click on tab2 trigger
        cut.FindAll("[data-slot='tabs-trigger']")[1].Click();

        // Now tab2 should be active
        cut.Markup.ShouldContain("Content 2");
    }

    [Fact(DisplayName = "UAC02: Active trigger has active state")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ActiveTriggerHasActiveState()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsList>(list =>
            {
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab1");
                    trigger.AddChildContent("Tab 1");
                });
            });
        });

        var trigger = cut.Find("[data-slot='tabs-trigger']");
        trigger.GetAttribute("data-state").ShouldBe("active");
        trigger.GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC03: Inactive trigger has inactive state")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_InactiveTriggerHasInactiveState()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsList>(list =>
            {
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab1");
                    trigger.AddChildContent("Tab 1");
                });
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab2");
                    trigger.AddChildContent("Tab 2");
                });
            });
        });

        var triggers = cut.FindAll("[data-slot='tabs-trigger']");
        triggers[1].GetAttribute("data-state").ShouldBe("inactive");
        triggers[1].GetAttribute("aria-selected").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC04: Disabled trigger cannot be clicked")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DisabledTriggerCannotBeClicked()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsList>(list =>
            {
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab1");
                    trigger.AddChildContent("Tab 1");
                });
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab2");
                    trigger.Add(x => x.Disabled, true);
                    trigger.AddChildContent("Tab 2");
                });
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab1");
                content.AddChildContent("Content 1");
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab2");
                content.AddChildContent("Content 2");
            });
        });

        // Try clicking disabled trigger
        cut.FindAll("[data-slot='tabs-trigger']")[1].Click();

        // Should still show content 1 (tab1 still active because tab2 is disabled)
        cut.Markup.ShouldContain("Content 1");
        // Tab 1 content should still be active
        var content1 = cut.Find("[data-slot='tabs-content'][data-state='active']");
        content1.TextContent.ShouldContain("Content 1");
    }

    [Fact(DisplayName = "UAC05: Content shows active state")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ContentShowsActiveState()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab1");
                content.AddChildContent("Content 1");
            });
        });

        var content = cut.Find("[data-slot='tabs-content']");
        content.GetAttribute("data-state").ShouldBe("active");
    }

    [Fact(DisplayName = "UAC06: Only active content is rendered")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_OnlyActiveContentIsRendered()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab1");
                content.AddChildContent("Content 1");
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab2");
                content.AddChildContent("Content 2");
            });
        });

        var contents = cut.FindAll("[data-slot='tabs-content']");
        contents.Count.ShouldBe(1);
        contents[0].GetAttribute("data-state").ShouldBe("active");
    }

    [Fact(DisplayName = "UAC07: Inactive content is not rendered")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_InactiveContentIsNotRendered()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab1");
                content.AddChildContent("Content 1");
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab2");
                content.AddChildContent("Content 2");
            });
        });

        cut.Markup.ShouldContain("Content 1");
        cut.Markup.ShouldNotContain("Content 2");
        cut.FindAll("[data-slot='tabs-content']").Count.ShouldBe(1);
    }
}
