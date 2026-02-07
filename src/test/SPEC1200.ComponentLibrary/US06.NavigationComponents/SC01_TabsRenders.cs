using Bunit;
using LowlandTech.Components.Components.Tabs;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US06.NavigationComponents;

/// <summary>
/// Tests for Tabs component functionality.
/// SC01 - Implement Tabs component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "01")]
public class SC01_TabsRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Tabs>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Tabs>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Tabs, TabsList, TabsTrigger, and TabsContent components")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldHaveAllComponents()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();
        
        // Verify Tabs container exists
        var tabsContainer = _component!.Find(".tabs");
        tabsContainer.ShouldNotBeNull();
        
        // Test TabsList
        using var ctx1 = new BunitContext();
        var tabsList = ctx1.Render<TabsList>();
        tabsList.ShouldNotBeNull();
        tabsList.Find(".tabs-list").ShouldNotBeNull();
        
        // Test TabsTrigger
        using var ctx2 = new BunitContext();
        var trigger = ctx2.Render<TabsTrigger>(p => p.Add(x => x.Value, "tab1"));
        trigger.ShouldNotBeNull();
        trigger.Find(".tabs-trigger").ShouldNotBeNull();
        
        // Test TabsContent
        using var ctx3 = new BunitContext();
        var content = ctx3.Render<TabsContent>(p => p.Add(x => x.Value, "tab1"));
        content.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support a default active tab")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldSupportDefaultActiveTab()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Tabs>(parameters => parameters
            .Add(p => p.DefaultValue, "tab1")
        );

        // Verify component accepted the parameter
        component.Instance.Value.ShouldBe("tab1");
    }

    [Fact(DisplayName = "And it should support a controlled active tab via Value and ValueChanged")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ShouldSupportControlledTab()
    {
        var activeTab = "tab1";
        
        using var ctx = new BunitContext();
        var component = ctx.Render<Tabs>(parameters => parameters
            .Add(p => p.Value, activeTab)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => activeTab = value
            ))
        );

        // Initially tab1 should be active
        component.Instance.Value.ShouldBe("tab1");
        
        // Change the value programmatically
        await component.Instance.SetValueAsync("tab2");
        
        // Verify activeTab changed
        activeTab.ShouldBe("tab2");
    }

    [Fact(DisplayName = "And it should render only the active tab content")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldRenderOnlyActiveContent()
    {
        using var ctx = new BunitContext();
        
        // Create a Tabs component with DefaultValue
        var tabs = ctx.Render<Tabs>(p => p.Add(x => x.DefaultValue, "tab1"));
        
        // Create TabsContent with matching value - should render
        var content1 = ctx.Render<TabsContent>(p => p
            .Add(x => x.Value, "tab1")
            .AddCascadingValue(tabs.Instance)
            .AddChildContent("<div>Content 1</div>")
        );
        
        // Content should be visible  
        var div = content1.Find("div");
        div.ShouldNotBeNull();
        div.TextContent.ShouldBe("Content 1");
        
        // Create TabsContent with non-matching value - should not render
        var content2 = ctx.Render<TabsContent>(p => p
            .Add(x => x.Value, "tab2")
            .AddCascadingValue(tabs.Instance)
            .AddChildContent("<div>Content 2</div>")
        );
        
        // Content2 should not exist in DOM
        var divs = content2.FindAll("div");
        divs.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "And it should use appropriate ARIA roles (tablist, tab, tabpanel)")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldUseARIARoles()
    {
        using var ctx = new BunitContext();
        
        // Verify TabsList has tablist role
        var tabsList = ctx.Render<TabsList>();
        var listElement = tabsList.Find("[role='tablist']");
        listElement.ShouldNotBeNull();
        
        // Verify TabsTrigger has tab role
        var trigger = ctx.Render<TabsTrigger>(p => p.Add(x => x.Value, "tab1"));
        var triggerElement = trigger.Find("[role='tab']");
        triggerElement.ShouldNotBeNull();
        
        // Verify TabsContent has tabpanel role when active
        var tabs = ctx.Render<Tabs>(p => p.Add(x => x.DefaultValue, "tab1"));
        var content = ctx.Render<TabsContent>(p => p
            .Add(x => x.Value, "tab1")
            .AddCascadingValue(tabs.Instance)
        );
        var panelElement = content.Find("[role='tabpanel']");
        panelElement.ShouldNotBeNull();
    }
}
