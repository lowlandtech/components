using Bunit;
using LowlandTech.Components.Components.Card;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Card component rendering and base functionality.
/// SC03 - Implement Card components
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "03")]
public class SC03_CardRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Card>? _cardComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _cardComponent = _ctx.Render<Card>(parameters => parameters
            .Add(p => p.ChildContent, "Card content")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Card, CardHeader, CardTitle, CardDescription, CardContent, and CardFooter components")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_ShouldHaveAllCardComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // Card
        var card = ctx.Render<Card>(parameters => parameters
            .Add(p => p.ChildContent, "Test"));
        card.ShouldNotBeNull();
        
        // CardHeader
        var header = ctx.Render<CardHeader>(parameters => parameters
            .Add(p => p.ChildContent, "Header"));
        header.ShouldNotBeNull();
        
        // CardTitle
        var title = ctx.Render<CardTitle>(parameters => parameters
            .Add(p => p.ChildContent, "Title"));
        title.ShouldNotBeNull();
        
        // CardDescription
        var description = ctx.Render<CardDescription>(parameters => parameters
            .Add(p => p.ChildContent, "Description"));
        description.ShouldNotBeNull();
        
        // CardContent
        var content = ctx.Render<CardContent>(parameters => parameters
            .Add(p => p.ChildContent, "Content"));
        content.ShouldNotBeNull();
        
        // CardFooter
        var footer = ctx.Render<CardFooter>(parameters => parameters
            .Add(p => p.ChildContent, "Footer"));
        footer.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And Card should render a wrapper with class \"card\"")]
    [Trait(Spec.UAC, "12")]
    public async Task UAC12_CardShouldHaveCardClass()
    {
        await ArrangeAndActAsync();
        
        var card = _cardComponent!.Find("div");
        card.ClassList.ShouldContain("card");
    }

    [Fact(DisplayName = "And Card subcomponents should render wrappers with their respective classes (card-header, card-title, card-description, card-content, card-footer)")]
    [Trait(Spec.UAC, "13")]
    public async Task UAC13_SubcomponentsShouldHaveRespectiveClasses()
    {
        using var ctx = new BunitContext();
        
        // Test CardHeader
        var header = ctx.Render<CardHeader>(parameters => parameters
            .Add(p => p.ChildContent, "Header"));
        header.Find("div").ClassList.ShouldContain("card-header");
        
        // Test CardTitle
        var title = ctx.Render<CardTitle>(parameters => parameters
            .Add(p => p.ChildContent, "Title"));
        title.Find("h3").ClassList.ShouldContain("card-title");
        
        // Test CardDescription
        var description = ctx.Render<CardDescription>(parameters => parameters
            .Add(p => p.ChildContent, "Description"));
        description.Find("p").ClassList.ShouldContain("card-description");
        
        // Test CardContent
        var content = ctx.Render<CardContent>(parameters => parameters
            .Add(p => p.ChildContent, "Content"));
        content.Find("div").ClassList.ShouldContain("card-content");
        
        // Test CardFooter
        var footer = ctx.Render<CardFooter>(parameters => parameters
            .Add(p => p.ChildContent, "Footer"));
        footer.Find("div").ClassList.ShouldContain("card-footer");
    }

    [Fact(DisplayName = "And Card should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "14")]
    public async Task UAC14_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Card>(parameters => parameters
            .Add(p => p.Class, "custom-card-class")
        );

        var card = component.Find("div");
        card.ClassList.ShouldContain("card");
        card.ClassList.ShouldContain("custom-card-class");
    }
}
