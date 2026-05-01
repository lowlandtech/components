using Bunit;
using ButtonSut = LowlandTech.Components.Button.Button;
using LowlandTech.Components.Tests.Infrastructure;
using LowlandTech.Components.Button;

namespace LowlandTech.Components.Tests.SPEC1007.Button.US01.View;

[Trait(Spec.SPEC, "1007")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Button component")]
public class SC01_RenderButton : ViewScenario
{
    [Fact(DisplayName = "UAC01: Button renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ButtonSut>();
        cut.Find("[data-slot='button']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Button renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Button renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='button']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Button default variant renders btn and btn-default classes")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DefaultVariantClasses()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Default));
        var el = cut.Find("[data-slot='button']");
        el.ClassList.ShouldContain("btn");
        el.ClassList.ShouldContain("btn-default");
        el.GetAttribute("data-variant").ShouldBe(ButtonVariant.Default);
    }

    [Fact(DisplayName = "UAC05: Button Secondary variant renders btn-secondary class")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SecondaryVariantClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Secondary));
        cut.Find("[data-slot='button']").ClassList.ShouldContain("btn-secondary");
    }

    [Fact(DisplayName = "UAC06: Button Destructive variant renders btn-destructive class")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DestructiveVariantClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Destructive));
        cut.Find("[data-slot='button']").ClassList.ShouldContain("btn-destructive");
    }

    [Fact(DisplayName = "UAC07: Button Ghost variant renders btn-ghost class")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_GhostVariantClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Ghost));
        cut.Find("[data-slot='button']").ClassList.ShouldContain("btn-ghost");
    }

    [Fact(DisplayName = "UAC08: Button Icon size renders btn-icon class")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_IconSizeClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Size, ButtonSize.Icon));
        var el = cut.Find("[data-slot='button']");
        el.ClassList.ShouldContain("btn-icon");
        el.GetAttribute("data-size").ShouldBe(ButtonSize.Icon);
    }

    [Fact(DisplayName = "UAC09: Button Sm size renders btn-sm class")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_SmSizeClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Size, ButtonSize.Sm));
        cut.Find("[data-slot='button']").ClassList.ShouldContain("btn-sm");
    }

    [Fact(DisplayName = "UAC10: Button Outline and Link variants render correct classes")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_RemainingVariants()
    {
        var outline = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Outline));
        outline.Find("[data-slot='button']").ClassList.ShouldContain("btn-outline");

        var link = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Link));
        link.Find("[data-slot='button']").ClassList.ShouldContain("btn-link");
    }
}
