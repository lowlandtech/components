using Bunit;
using LowlandTech.Components.Components.Pagination;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US06.NavigationComponents;

/// <summary>
/// Tests for Pagination component rendering and base functionality.
/// SC04 - Implement Pagination component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "04")]
public class SC04_PaginationRenders : Scenario
{
    private BunitContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationPrevious, and PaginationNext components")]
    [Trait(Spec.UAC, "14")]
    public async Task UAC14_ShouldHaveAllPaginationComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // Pagination
        var pagination = ctx.Render<Pagination>(parameters => parameters
            .Add(p => p.ChildContent, "Test"));
        pagination.ShouldNotBeNull();
        
        // PaginationContent
        var content = ctx.Render<PaginationContent>(parameters => parameters
            .Add(p => p.ChildContent, "Content"));
        content.ShouldNotBeNull();
        
        // PaginationItem
        var item = ctx.Render<PaginationItem>(parameters => parameters
            .Add(p => p.ChildContent, "Item"));
        item.ShouldNotBeNull();
        
        // PaginationLink
        var link = ctx.Render<PaginationLink>(parameters => parameters
            .Add(p => p.ChildContent, "Link"));
        link.ShouldNotBeNull();
        
        // PaginationPrevious
        var previous = ctx.Render<PaginationPrevious>();
        previous.ShouldNotBeNull();
        
        // PaginationNext
        var next = ctx.Render<PaginationNext>();
        next.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support active and disabled states for links")]
    [Trait(Spec.UAC, "15")]
    public async Task UAC15_ShouldSupportActiveAndDisabledStates()
    {
        using var ctx = new BunitContext();
        
        // Test active state
        var activeLink = ctx.Render<PaginationLink>(parameters => parameters
            .Add(p => p.IsActive, true)
            .Add(p => p.ChildContent, "1"));
        activeLink.Find("a").ClassList.ShouldContain("pagination-link-active");
        
        // Test disabled state
        var disabledLink = ctx.Render<PaginationLink>(parameters => parameters
            .Add(p => p.IsDisabled, true)
            .Add(p => p.ChildContent, "2"));
        disabledLink.Find("a").ClassList.ShouldContain("pagination-link-disabled");
    }

    [Fact(DisplayName = "And it should support a custom page size and current page")]
    [Trait(Spec.UAC, "16")]
    public async Task UAC16_ShouldSupportCustomPageAndSize()
    {
        // Test creating a complete pagination structure
        using var ctx = new BunitContext();
        
        var pagination = ctx.Render<Pagination>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<PaginationContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    // Page 1 (active)
                    contentBuilder.OpenComponent<PaginationItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<PaginationLink>(0);
                        itemBuilder.AddAttribute(1, "IsActive", true);
                        itemBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(linkBuilder =>
                        {
                            linkBuilder.AddContent(0, "1");
                        }));
                        itemBuilder.CloseComponent();
                    }));
                    contentBuilder.CloseComponent();
                    
                    // Page 2
                    contentBuilder.OpenComponent<PaginationItem>(2);
                    contentBuilder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<PaginationLink>(0);
                        itemBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(linkBuilder =>
                        {
                            linkBuilder.AddContent(0, "2");
                        }));
                        itemBuilder.CloseComponent();
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            })));
        
        pagination.ShouldNotBeNull();
        var links = pagination.FindAll("a");
        links.Count.ShouldBe(2);
        links[0].ClassList.ShouldContain("pagination-link-active");
        links[0].TextContent.ShouldBe("1");
        links[1].TextContent.ShouldBe("2");
    }

    [Fact(DisplayName = "And it should render semantic navigation markup")]
    [Trait(Spec.UAC, "17")]
    public async Task UAC17_ShouldRenderSemanticNavigationMarkup()
    {
        using var ctx = new BunitContext();
        
        var pagination = ctx.Render<Pagination>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<PaginationContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<PaginationItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<PaginationPrevious>(0);
                        itemBuilder.CloseComponent();
                    }));
                    contentBuilder.CloseComponent();
                    
                    contentBuilder.OpenComponent<PaginationItem>(2);
                    contentBuilder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<PaginationLink>(0);
                        itemBuilder.AddAttribute(1, "IsActive", true);
                        itemBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(linkBuilder =>
                        {
                            linkBuilder.AddContent(0, "1");
                        }));
                        itemBuilder.CloseComponent();
                    }));
                    contentBuilder.CloseComponent();
                    
                    contentBuilder.OpenComponent<PaginationItem>(4);
                    contentBuilder.AddAttribute(5, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<PaginationNext>(0);
                        itemBuilder.CloseComponent();
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            })));
        
        // Should render as nav element
        var nav = pagination.Find("nav");
        nav.ShouldNotBeNull();
        nav.ClassList.ShouldContain("pagination");
        
        // Should contain ul for content
        var ul = nav.QuerySelector("ul");
        ul.ShouldNotBeNull();
        ul.ClassList.ShouldContain("pagination-content");
        
        // Should contain li items
        var items = ul.QuerySelectorAll("li");
        items.Length.ShouldBe(3);
        items[0].ClassList.ShouldContain("pagination-item");
    }
}
