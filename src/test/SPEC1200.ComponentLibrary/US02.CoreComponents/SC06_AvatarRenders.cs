using Bunit;
using LowlandTech.Components.Components.Avatar;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Avatar component rendering and base functionality.
/// SC06 - Implement Avatar components
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "06")]
public class SC06_AvatarRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Avatar>? _avatarComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _avatarComponent = _ctx.Render<Avatar>(parameters => parameters
            .AddChildContent("<span>Test Avatar</span>")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Avatar, AvatarImage, and AvatarFallback components")]
    [Trait(Spec.UAC, "22")]
    public async Task UAC22_ShouldHaveAllAvatarComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // Avatar
        var avatar = ctx.Render<Avatar>(parameters => parameters
            .AddChildContent("<span>Test</span>"));
        avatar.ShouldNotBeNull();
        
        // AvatarImage
        var image = ctx.Render<AvatarImage>(parameters => parameters
            .Add(p => p.Src, "test.jpg")
            .Add(p => p.Alt, "Test"));
        image.ShouldNotBeNull();
        
        // AvatarFallback
        var fallback = ctx.Render<AvatarFallback>(parameters => parameters
            .AddChildContent("AB"));
        fallback.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And Avatar should render a wrapper with class \"avatar\"")]
    [Trait(Spec.UAC, "23")]
    public async Task UAC23_AvatarShouldHaveAvatarClass()
    {
        await ArrangeAndActAsync();
        
        var avatar = _avatarComponent!.Find("span");
        avatar.ClassList.ShouldContain("avatar");
    }

    [Fact(DisplayName = "And AvatarImage should render an <img> element and AvatarFallback should render a fallback container")]
    [Trait(Spec.UAC, "24")]
    public async Task UAC24_SubcomponentsShouldRenderCorrectElements()
    {
        using var ctx = new BunitContext();
        
        // Test AvatarImage renders <img>
        var image = ctx.Render<AvatarImage>(parameters => parameters
            .Add(p => p.Src, "/images/avatar.jpg")
            .Add(p => p.Alt, "User Avatar"));
        var imgElement = image.Find("img");
        imgElement.ShouldNotBeNull();
        imgElement.ClassList.ShouldContain("avatar-image");
        imgElement.GetAttribute("src").ShouldBe("/images/avatar.jpg");
        imgElement.GetAttribute("alt").ShouldBe("User Avatar");
        
        // Test AvatarFallback renders <span> container
        var fallback = ctx.Render<AvatarFallback>(parameters => parameters
            .AddChildContent("JD"));
        var fallbackElement = fallback.Find("span");
        fallbackElement.ShouldNotBeNull();
        fallbackElement.ClassList.ShouldContain("avatar-fallback");
        fallbackElement.TextContent.ShouldBe("JD");
    }

    [Fact(DisplayName = "And Avatar should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "25")]
    public async Task UAC25_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Avatar>(parameters => parameters
            .Add(p => p.Class, "custom-avatar-class")
            .AddChildContent("<span>Test</span>")
        );

        var avatar = component.Find("span");
        avatar.ClassList.ShouldContain("avatar");
        avatar.ClassList.ShouldContain("custom-avatar-class");
    }
}
