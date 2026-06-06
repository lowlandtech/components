

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

    [Fact(DisplayName = "UAC11: Button Unstyled variant emits .btn but no btn-* variant class")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_UnstyledVariantOmitsBtnVariantClass()
    {
        // Unstyled is the opt-out for consumers that fully style via a
        // custom class (e.g. canvas Property Grid's lt-style-section-header
        // and lt-property-panel-menu-item rules). The button still gets the
        // base `.btn` typography/transitions, but no `btn-default` /
        // `btn-secondary` / etc. visual chrome that would fight the consumer's
        // own styles. Reverting the switch arm silently re-introduces the
        // chunky bg-primary look on every section header / menu item.
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Variant, ButtonVariant.Unstyled));
        var el = cut.Find("[data-slot='button']");
        el.ClassList.ShouldContain("btn",
            customMessage:
                "Unstyled buttons must still get the base `.btn` class — " +
                "that's where typography, focus ring, transitions, and " +
                "disabled state live. Drop those and unstyled buttons stop " +
                "feeling like buttons.");
        el.ClassList.ShouldNotContain("btn-default",
            customMessage:
                "Unstyled MUST NOT emit btn-default. The whole point of this " +
                "variant is to suppress the default's bg-primary / h-9 / " +
                "px-4 py-2 chrome so the consumer's class fully controls " +
                "visuals (e.g. the canvas Property Grid section bars).");
        el.ClassList.ShouldNotContain("btn-secondary");
        el.ClassList.ShouldNotContain("btn-ghost");
        el.ClassList.ShouldNotContain("btn-outline");
        el.ClassList.ShouldNotContain("btn-link");
        el.ClassList.ShouldNotContain("btn-destructive");
    }

    [Fact(DisplayName = "UAC12: Button is design-selectable when design mode is enabled")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_DesignModeMakesPropertyGridComponentSelectable()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.IsDesignMode, true));
        var el = cut.Find("[data-slot='button']");

        el.GetAttribute("data-design-mode").ShouldBe("true");
        el.GetAttribute("data-design-selectable").ShouldBe("true");
        el.ClassList.ShouldContain("lt-design-selectable");
    }

    [Fact(DisplayName = "UAC13: Button is not design-selectable outside design mode")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_DesignSelectableAttributeIsAbsentOutsideDesignMode()
    {
        var cut = Ctx.Render<ButtonSut>();
        var el = cut.Find("[data-slot='button']");

        el.GetAttribute("data-design-mode").ShouldBeNull();
        el.GetAttribute("data-design-selectable").ShouldBeNull();
        el.ClassList.ShouldNotContain("lt-design-selectable");
    }

    [Fact(DisplayName = "UAC14: Design-mode click selects the Button")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_DesignModeClickSelectsButton()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.IsDesignMode, true));

        cut.Find("[data-slot='button']").Click();

        var el = cut.Find("[data-slot='button']");
        el.GetAttribute("data-design-selected").ShouldBe("true");
        el.ClassList.ShouldContain("lt-design-selected");
    }

    [Fact(DisplayName = "UAC15: Disabled Button remains selectable in design mode")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_DisabledButtonRemainsSelectableInDesignMode()
    {
        var cut = Ctx.Render<ButtonSut>(p => p
            .Add(x => x.IsDesignMode, true)
            .AddUnmatched("disabled", true));

        var el = cut.Find("[data-slot='button']");
        el.HasAttribute("disabled").ShouldBeFalse();
        el.GetAttribute("aria-disabled").ShouldBe("true");
        el.ClassList.ShouldContain("lt-design-disabled");

        el.Click();

        cut.Find("[data-slot='button']").GetAttribute("data-design-selected").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC18: Disabled parameter renders disabled state")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_DisabledParameterRendersDisabledState()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Disabled, true));
        var el = cut.Find("[data-slot='button']");

        el.HasAttribute("disabled").ShouldBeTrue();
        el.GetAttribute("aria-disabled").ShouldBe("true");
        el.GetAttribute("data-disabled").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC19: Disabled parameter remains selectable in design mode")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_DisabledParameterRemainsSelectableInDesignMode()
    {
        var cut = Ctx.Render<ButtonSut>(p => p
            .Add(x => x.IsDesignMode, true)
            .Add(x => x.Disabled, true));

        var el = cut.Find("[data-slot='button']");
        el.HasAttribute("disabled").ShouldBeFalse();
        el.GetAttribute("aria-disabled").ShouldBe("true");
        el.ClassList.ShouldContain("lt-design-disabled");

        el.Click();

        cut.Find("[data-slot='button']").GetAttribute("data-design-selected").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC20: Icon parameter renders core icon")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_IconParameterRendersCoreIcon()
    {
        var cut = Ctx.Render<ButtonSut>(p => p
            .Add(x => x.Icon, nameof(Icons.Search))
            .AddChildContent("Search"));

        cut.Find("[data-slot='icon']").ShouldNotBeNull();
        cut.Markup.ShouldContain(Icons.Search);
        cut.Find("[data-slot='button']").TextContent.ShouldContain("Search");
    }

    [Fact(DisplayName = "UAC21: Button descriptor exposes Disabled and core Icon options")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_ButtonDescriptorExposesDisabledAndCoreIconOptions()
    {
        var descriptor = new ComponentDescriptorRegistry().GetByType(typeof(ButtonSut));

        descriptor.ShouldNotBeNull();
        var disabled = descriptor.Properties.Single(p => p.Name == nameof(ButtonSut.Disabled));
        disabled.Editor.ShouldBe(PropertyEditor.Switch);
        disabled.DefaultValue.ShouldBe(false);

        var icon = descriptor.Properties.Single(p => p.Name == nameof(ButtonSut.Icon));
        icon.Editor.ShouldBe(PropertyEditor.Select);
        icon.DefaultValue.ShouldBe(ButtonIcon.None);
        icon.Options.First().ShouldBe(ButtonIcon.None);
        icon.Options.ShouldContain(nameof(Icons.Search));
    }

    [Fact(DisplayName = "UAC16: StoryCard toggles design mode and shows a property sidebar")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_StoryCardDesignModeShowsPropertySidebar()
    {
        var cut = RenderDesignableStoryCard();

        cut.FindAll("[data-slot='property-grid']").Count.ShouldBe(0);

        cut.Find("[data-slot='switch']").Click();

        cut.Find("[data-slot='property-grid']").ShouldNotBeNull();
        cut.Find("[data-slot='button']").GetAttribute("data-design-selectable").ShouldBe("true");
        cut.FindAll("[data-slot='property-grid-row']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC17: StoryCard design PropertyGrid edits only the selected component")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_StoryCardDesignPropertyGridUpdatesOnlySelectedPreviewComponent()
    {
        var cut = RenderDesignableStoryCard();

        cut.Find("[data-slot='switch']").Click();
        cut.FindAll("[data-slot='button']")[1].Click();

        cut.Find("[data-slot='select-trigger']").TextContent.ShouldContain(ButtonVariant.Secondary);

        cut.Find("[data-slot='select-trigger']").Click();
        cut.WaitForAssertion(() =>
            cut.FindAll(".node-menu-item").Count.ShouldBeGreaterThanOrEqualTo(1));
        cut.Find(".node-menu-item[data-value='ghost']").Click();

        var buttons = cut.FindAll("[data-slot='button']");
        buttons[0].GetAttribute("data-variant").ShouldBe(ButtonVariant.Default);
        buttons[1].GetAttribute("data-variant").ShouldBe(ButtonVariant.Ghost);
        buttons[1].GetAttribute("data-design-selected").ShouldBe("true");
        buttons[0].GetAttribute("data-design-selected").ShouldBeNull();
    }

    private IRenderedComponent<DesignableStoryHost> RenderDesignableStoryCard() =>
        Ctx.Render<DesignableStoryHost>();

    private sealed class DesignableStoryHost : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<StoryCard>(1);
            builder.AddAttribute(2, nameof(StoryCard.Title), "Button");
            builder.AddAttribute(3, nameof(StoryCard.DesignComponentType), typeof(ButtonSut));
            builder.AddAttribute(4, nameof(StoryCard.States), new[]
            {
                new StoryCard.State("default", "Default", Preview, [])
            });
            builder.CloseComponent();
        }

        private static RenderFragment Preview => builder =>
        {
            builder.OpenComponent<ButtonSut>(0);
            builder.AddAttribute(1, nameof(ButtonSut.ChildContent), (RenderFragment)(childBuilder =>
                childBuilder.AddContent(0, "Default")));
            builder.CloseComponent();

            builder.OpenComponent<ButtonSut>(2);
            builder.AddAttribute(3, nameof(ButtonSut.Variant), ButtonVariant.Secondary);
            builder.AddAttribute(4, nameof(ButtonSut.ChildContent), (RenderFragment)(childBuilder =>
                childBuilder.AddContent(0, "Secondary")));
            builder.CloseComponent();
        };
    }
}
