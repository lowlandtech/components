using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tabs;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US02.Behavior;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Tabs keyboard navigation")]
public class SC02_TabsKeyboardNavigation : ViewScenario
{
    [Fact(DisplayName = "UAC01: ArrowRight moves to next tab in automatic mode")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ArrowRightMovesToNextTab()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "automatic");
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

        // Press ArrowRight
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        // Tab 2 should be active
        cut.Markup.ShouldContain("Content 2");
    }

    [Fact(DisplayName = "UAC02: ArrowLeft moves to previous tab")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ArrowLeftMovesToPreviousTab()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab2");
            p.Add(x => x.KeyboardActivation, "automatic");
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

        // Press ArrowLeft
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });

        // Tab 1 should be active
        cut.Markup.ShouldContain("Content 1");
    }

    [Fact(DisplayName = "UAC03: Home key moves to first tab")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_HomeKeyMovesToFirstTab()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab2");
            p.Add(x => x.KeyboardActivation, "automatic");
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
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab3");
                    trigger.AddChildContent("Tab 3");
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
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab3");
                content.AddChildContent("Content 3");
            });
        });

        // Press Home
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "Home" });

        // Tab 1 should be active
        cut.Markup.ShouldContain("Content 1");
    }

    [Fact(DisplayName = "UAC04: End key moves to last tab")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EndKeyMovesToLastTab()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "automatic");
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
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab3");
                    trigger.AddChildContent("Tab 3");
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
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab3");
                content.AddChildContent("Content 3");
            });
        });

        // Press End
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "End" });

        // Tab 3 should be active
        cut.Markup.ShouldContain("Content 3");
    }

    [Fact(DisplayName = "UAC05: Navigation loops from last to first")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NavigationLoopsFromLastToFirst()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab2");
            p.Add(x => x.KeyboardActivation, "automatic");
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

        // Press ArrowRight from last tab
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        // Should loop to tab 1
        cut.Markup.ShouldContain("Content 1");
    }

    [Fact(DisplayName = "UAC06: Navigation loops from first to last")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NavigationLoopsFromFirstToLast()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "automatic");
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

        // Press ArrowLeft from first tab
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });

        // Should loop to tab 2
        cut.Markup.ShouldContain("Content 2");
    }

    [Fact(DisplayName = "UAC07: Manual mode requires Enter to activate")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ManualModeRequiresEnter()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "manual");
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

        // Press ArrowRight (should only move focus, not activate)
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        // Tab 1 should still be active (not Tab 2)
        cut.Markup.ShouldContain("Content 1");
    }

    [Fact(DisplayName = "UAC08: Enter activates tab in manual mode")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_EnterActivatesInManualMode()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "manual");
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

        var tablist = cut.Find("[role='tablist']");
        
        // Move focus to next tab
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        
        // Press Enter to activate
        tablist.KeyDown(new KeyboardEventArgs { Key = "Enter" });

        // Tab 2 should now be active
        cut.Markup.ShouldContain("Content 2");
    }

    [Fact(DisplayName = "UAC09: Skips disabled tabs during navigation")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_SkipsDisabledTabs()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "automatic");
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
                list.AddChildContent<TabsTrigger>(trigger =>
                {
                    trigger.Add(x => x.Value, "tab3");
                    trigger.AddChildContent("Tab 3");
                });
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab1");
                content.AddChildContent("Content 1");
            });
            p.AddChildContent<TabsContent>(content =>
            {
                content.Add(x => x.Value, "tab3");
                content.AddChildContent("Content 3");
            });
        });

        // Press ArrowRight - should skip disabled tab2 and go to tab3
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        // Tab 3 should be active (skipped tab2)
        cut.Markup.ShouldContain("Content 3");
    }

    [Fact(DisplayName = "UAC10: Vertical orientation uses ArrowUp and ArrowDown")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_VerticalOrientationUsesUpDown()
    {
        var cut = Ctx.Render<TabsSut>(p =>
        {
            p.Add(x => x.Orientation, "vertical");
            p.Add(x => x.DefaultValue, "tab1");
            p.Add(x => x.KeyboardActivation, "automatic");
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

        // Press ArrowDown
        var tablist = cut.Find("[role='tablist']");
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        // Tab 2 should be active
        cut.Markup.ShouldContain("Content 2");

        // Press ArrowUp
        tablist.KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });

        // Tab 1 should be active
        cut.Markup.ShouldContain("Content 1");
    }
}
