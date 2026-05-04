using Bunit;
using ButtonSut = LowlandTech.Components.Button.Button;
using LowlandTech.Components.Tests.Infrastructure;
using LowlandTech.Components.Button;

namespace LowlandTech.Components.Tests.SPEC1007.Button.US02.Variant;

[Trait(Spec.SPEC, "1007")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Button Variant + Color scheme parameters")]
public class SC01_ButtonVariantAndColorScheme : ViewScenario
{
    [Fact(DisplayName = "UAC01: Button renders all variant data attributes")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_AllVariantsRenderDataAttribute()
    {
        foreach (var variant in new[] {
            ButtonVariant.Default,
            ButtonVariant.Secondary,
            ButtonVariant.Destructive,
            ButtonVariant.Outline,
            ButtonVariant.Ghost,
            ButtonVariant.Link,
            ButtonVariant.Unstyled
        })
        {
            var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, variant));
            cut.Find("[data-slot='button']").GetAttribute("data-variant").ShouldBe(variant);
        }
    }

    [Fact(DisplayName = "UAC02: Button renders all size data attributes")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_AllSizesRenderDataAttribute()
    {
        foreach (var size in new[] {
            ButtonSize.Default,
            ButtonSize.Sm,
            ButtonSize.Lg,
            ButtonSize.Icon,
            ButtonSize.IconSm
        })
        {
            var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Size, size));
            cut.Find("[data-slot='button']").GetAttribute("data-size").ShouldBe(size);
        }
    }

    [Fact(DisplayName = "UAC03: Button Primary alias maps to default")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PrimaryAliasMapsToDefault()
    {
        ButtonVariant.Primary.ShouldBe(ButtonVariant.Default);
    }

    [Fact(DisplayName = "UAC04: Button variant classes map correctly")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_VariantClassesMapCorrectly()
    {
        var cases = new (string Variant, string ExpectedClass)[]
        {
            (ButtonVariant.Default,     "btn-default"),
            (ButtonVariant.Secondary,   "btn-secondary"),
            (ButtonVariant.Destructive, "btn-destructive"),
            (ButtonVariant.Outline,     "btn-outline"),
            (ButtonVariant.Ghost,       "btn-ghost"),
            (ButtonVariant.Link,        "btn-link"),
        };

        foreach (var (variant, expected) in cases)
        {
            var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, variant));
            cut.Find("[data-slot='button']").ClassList.ShouldContain(expected);
        }
    }

    [Fact(DisplayName = "UAC05: Button size classes map correctly")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SizeClassesMapCorrectly()
    {
        var cases = new (string Size, string ExpectedClass)[]
        {
            (ButtonSize.Sm,      "btn-sm"),
            (ButtonSize.Lg,      "btn-lg"),
            (ButtonSize.Icon,    "btn-icon"),
            (ButtonSize.IconSm,  "btn-icon-sm"),
        };

        foreach (var (size, expected) in cases)
        {
            var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Size, size));
            cut.Find("[data-slot='button']").ClassList.ShouldContain(expected);
        }
    }

    [Fact(DisplayName = "UAC06: Button Unstyled variant omits variant class")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_UnstyledOmitsVariantClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Unstyled));
        var el = cut.Find("[data-slot='button']");
        el.ClassList.ShouldContain("btn");
        el.ClassList.ShouldNotContain("btn-default");
    }

    [Fact(DisplayName = "UAC07: Button default size omits size class")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_DefaultSizeOmitsSizeClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Size, ButtonSize.Default));
        var el = cut.Find("[data-slot='button']");
        el.ClassList.ShouldNotContain("btn-sm");
        el.ClassList.ShouldNotContain("btn-lg");
        el.ClassList.ShouldNotContain("btn-icon");
    }
}
