using Bunit;
using LowlandTech.Components.Components.Breadcrumb;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US06.NavigationComponents;

/// <summary>
/// Tests for Breadcrumb component rendering and base functionality.
/// SC03 - Implement Breadcrumb component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "03")]
public class SC03_BreadcrumbRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Breadcrumb>? _breadcrumbComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        // Simple render for initial test - details tested in individual tests
        _breadcrumbComponent = _ctx.Render<Breadcrumb>(parameters => parameters
            .AddChildContent("<span>Test</span>")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, and BreadcrumbSeparator components")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldHaveAllBreadcrumbComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // Breadcrumb
        var breadcrumb = ctx.Render<Breadcrumb>(parameters => parameters
            .AddChildContent("<span>Test</span>"));
        breadcrumb.ShouldNotBeNull();
        
        // BreadcrumbList
        var list = ctx.Render<BreadcrumbList>(parameters => parameters
            .AddChildContent("<li>Test</li>"));
        list.ShouldNotBeNull();
        
        // BreadcrumbItem
        var item = ctx.Render<BreadcrumbItem>(parameters => parameters
            .AddChildContent("<a>Test</a>"));
        item.ShouldNotBeNull();
        
        // BreadcrumbLink
        var link = ctx.Render<BreadcrumbLink>(parameters => parameters
            .Add(p => p.Href, "/test")
            .AddChildContent("Test"));
        link.ShouldNotBeNull();
        
        // BreadcrumbSeparator
        var separator = ctx.Render<BreadcrumbSeparator>();
        separator.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should render semantic navigation markup")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_ShouldRenderSemanticNavigation()
    {
        using var ctx = new BunitContext();
        
        // Test Breadcrumb renders as <nav> with aria-label
        var breadcrumb = ctx.Render<Breadcrumb>(parameters => parameters
            .AddChildContent("<span>Test</span>"));
        var nav = breadcrumb.Find("nav");
        nav.ShouldNotBeNull();
        nav.GetAttribute("aria-label").ShouldBe("breadcrumb");
        nav.ClassList.ShouldContain("breadcrumb");
        
        // Test BreadcrumbList renders as <ol>
        var list = ctx.Render<BreadcrumbList>(parameters => parameters
            .AddChildContent("<li>Test</li>"));
        var ol = list.Find("ol");
        ol.ShouldNotBeNull();
        ol.ClassList.ShouldContain("breadcrumb-list");
        
        // Test BreadcrumbItem renders as <li>
        var item = ctx.Render<BreadcrumbItem>(parameters => parameters
            .AddChildContent("<a>Test</a>"));
        var li = item.Find("li");
        li.ShouldNotBeNull();
        li.ClassList.ShouldContain("breadcrumb-item");
        
        // Test BreadcrumbLink renders as <a>
        var link = ctx.Render<BreadcrumbLink>(parameters => parameters
            .Add(p => p.Href, "/")
            .AddChildContent("Home"));
        var a = link.Find("a");
        a.ShouldNotBeNull();
        a.ClassList.ShouldContain("breadcrumb-link");
        a.GetAttribute("href").ShouldBe("/");
        a.TextContent.ShouldBe("Home");
    }

    [Fact(DisplayName = "And it should support custom separators")]
    [Trait(Spec.UAC, "12")]
    public async Task UAC12_ShouldSupportCustomSeparators()
    {
        using var ctx = new BunitContext();
        
        // Test default separator (/)
        var defaultSeparator = ctx.Render<BreadcrumbSeparator>();
        var defaultSpan = defaultSeparator.Find("span");
        defaultSpan.ClassList.ShouldContain("breadcrumb-separator");
        defaultSpan.TextContent.ShouldBe("/");
        
        // Test custom separator
        var customSeparator = ctx.Render<BreadcrumbSeparator>(parameters => parameters
            .AddChildContent("›"));
        var customSpan = customSeparator.Find("span");
        customSpan.ClassList.ShouldContain("breadcrumb-separator");
        customSpan.TextContent.ShouldBe("›");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "13")]
    public async Task UAC13_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        
        // Test Breadcrumb
        var breadcrumb = ctx.Render<Breadcrumb>(parameters => parameters
            .Add(p => p.Class, "custom-breadcrumb")
            .AddChildContent("<span>Test</span>"));
        var nav = breadcrumb.Find("nav");
        nav.ClassList.ShouldContain("breadcrumb");
        nav.ClassList.ShouldContain("custom-breadcrumb");
        
        // Test BreadcrumbList
        var list = ctx.Render<BreadcrumbList>(parameters => parameters
            .Add(p => p.Class, "custom-list")
            .AddChildContent("<li>Test</li>"));
        var ol = list.Find("ol");
        ol.ClassList.ShouldContain("breadcrumb-list");
        ol.ClassList.ShouldContain("custom-list");
        
        // Test BreadcrumbItem
        var item = ctx.Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Class, "custom-item")
            .AddChildContent("<span>Test</span>"));
        var li = item.Find("li");
        li.ClassList.ShouldContain("breadcrumb-item");
        li.ClassList.ShouldContain("custom-item");
        
        // Test BreadcrumbLink
        var link = ctx.Render<BreadcrumbLink>(parameters => parameters
            .Add(p => p.Class, "custom-link")
            .Add(p => p.Href, "/test")
            .AddChildContent("Test"));
        var a = link.Find("a");
        a.ClassList.ShouldContain("breadcrumb-link");
        a.ClassList.ShouldContain("custom-link");
        
        // Test BreadcrumbSeparator
        var separator = ctx.Render<BreadcrumbSeparator>(parameters => parameters
            .Add(p => p.Class, "custom-separator"));
        var span = separator.Find("span");
        span.ClassList.ShouldContain("breadcrumb-separator");
        span.ClassList.ShouldContain("custom-separator");
    }
}
