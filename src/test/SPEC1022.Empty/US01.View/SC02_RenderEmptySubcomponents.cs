using Bunit;
using LowlandTech.Components.Empty;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1022.Empty.US01.View;

[Trait(Spec.SPEC, "1022")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Empty subcomponents")]
public class SC02_RenderEmptySubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: EmptyHeader renders data-slot and default classes")]
    [Trait(Spec.UAC, "01")]
    [Then("EmptyHeader emits data-slot='empty-header' with layout classes")]
    public void UAC01_EmptyHeader()
    {
        var cut = Ctx.Render<EmptyHeader>();
        var el = cut.Find("[data-slot='empty-header']");
        el.ShouldNotBeNull();
        el.ClassList.ShouldContain("cn-empty-header");
        el.ClassList.ShouldContain("flex-col");
        el.ClassList.ShouldContain("items-center");
    }

    [Fact(DisplayName = "UAC02: EmptyMedia renders data-slot and default variant")]
    [Trait(Spec.UAC, "02")]
    [Then("EmptyMedia emits data-slot='empty-icon' with data-variant='default'")]
    public void UAC02_EmptyMedia_DefaultVariant()
    {
        var cut = Ctx.Render<EmptyMedia>();
        var el = cut.Find("[data-slot='empty-icon']");
        el.ShouldNotBeNull();
        el.GetAttribute("data-variant").ShouldBe("default");
        el.ClassList.ShouldContain("cn-empty-media");
        el.ClassList.ShouldContain("cn-empty-media-default");
    }

    [Fact(DisplayName = "UAC03: EmptyMedia icon variant applies icon class")]
    [Trait(Spec.UAC, "03")]
    [Then("EmptyMedia with Variant='icon' has data-variant='icon' and cn-empty-media-icon class")]
    public void UAC03_EmptyMedia_IconVariant()
    {
        var cut = Ctx.Render<EmptyMedia>(p => p.Add(x => x.Variant, "icon"));
        var el = cut.Find("[data-slot='empty-icon']");
        el.GetAttribute("data-variant").ShouldBe("icon");
        el.ClassList.ShouldContain("cn-empty-media-icon");
    }

    [Fact(DisplayName = "UAC04: EmptyTitle renders data-slot and heading class")]
    [Trait(Spec.UAC, "04")]
    [Then("EmptyTitle emits data-slot='empty-title' with cn-empty-title")]
    public void UAC04_EmptyTitle()
    {
        var cut = Ctx.Render<EmptyTitle>();
        var el = cut.Find("[data-slot='empty-title']");
        el.ShouldNotBeNull();
        el.ClassList.ShouldContain("cn-empty-title");
    }

    [Fact(DisplayName = "UAC05: EmptyDescription renders data-slot and muted class")]
    [Trait(Spec.UAC, "05")]
    [Then("EmptyDescription emits data-slot='empty-description' with text-muted-foreground")]
    public void UAC05_EmptyDescription()
    {
        var cut = Ctx.Render<EmptyDescription>();
        var el = cut.Find("[data-slot='empty-description']");
        el.ShouldNotBeNull();
        el.ClassList.ShouldContain("cn-empty-description");
        el.ClassList.ShouldContain("text-muted-foreground");
    }

    [Fact(DisplayName = "UAC06: EmptyContent renders data-slot")]
    [Trait(Spec.UAC, "06")]
    [Then("EmptyContent emits data-slot='empty-content' with layout classes")]
    public void UAC06_EmptyContent()
    {
        var cut = Ctx.Render<EmptyContent>();
        var el = cut.Find("[data-slot='empty-content']");
        el.ShouldNotBeNull();
        el.ClassList.ShouldContain("cn-empty-content");
        el.ClassList.ShouldContain("flex-col");
        el.ClassList.ShouldContain("items-center");
    }
}
