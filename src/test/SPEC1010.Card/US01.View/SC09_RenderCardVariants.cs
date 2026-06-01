

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "09")]
[UserStory("01", "Card supports display modes and mode menu")]
public class SC09_RenderCardVariants : ViewScenario
{
    [Fact(DisplayName = "UAC01: Card renders data-mode attribute for normal mode")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataModeNormal()
    {
        var cut = Ctx.Render<CardSut>();
        cut.Find("[data-slot='card']").GetAttribute("data-mode").ShouldBe("normal");
    }

    [Fact(DisplayName = "UAC02: Card renders data-mode attribute for minimized mode")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersDataModeMinimized()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Mode, CardSut.ModeMinimized));
        cut.Find("[data-slot='card']").GetAttribute("data-mode").ShouldBe("minimized");
    }

    [Fact(DisplayName = "UAC03: Card renders data-mode attribute for maximized mode")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersDataModeMaximized()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Mode, CardSut.ModeMaximized));
        cut.Find("[data-slot='card']").GetAttribute("data-mode").ShouldBe("maximized");
    }

    [Fact(DisplayName = "UAC04: Card renders data-mode attribute for overlay-page mode")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersDataModeOverlayPage()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Mode, CardSut.ModeOverlayPage));
        cut.Find("[data-slot='card']").GetAttribute("data-mode").ShouldBe("overlay-page");
    }

    [Fact(DisplayName = "UAC05: Card renders data-mode attribute for overlay-app mode")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersDataModeOverlayApp()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Mode, CardSut.ModeOverlayApp));
        cut.Find("[data-slot='card']").GetAttribute("data-mode").ShouldBe("overlay-app");
    }

    [Fact(DisplayName = "UAC06: Card maximized mode applies flex layout class")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_MaximizedAppliesFlexLayout()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Mode, CardSut.ModeMaximized));
        var card = cut.Find("[data-slot='card']");
        card.ClassList.ShouldContain("flex");
        card.ClassList.ShouldContain("min-h-full");
        card.ClassList.ShouldContain("w-full");
    }

    [Fact(DisplayName = "UAC07: Card overlay-app mode applies fixed viewport class")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_OverlayAppAppliesFixedViewport()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Mode, CardSut.ModeOverlayApp));
        var card = cut.Find("[data-slot='card']");
        card.ClassList.ShouldContain("fixed");
        card.ClassList.ShouldContain("inset-0");
        card.ClassList.ShouldContain("z-50");
    }

    [Fact(DisplayName = "UAC08: CardModeMenu renders trigger button")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_CardModeMenuRendersTrigger()
    {
        var cut = Ctx.Render<CardModeMenu>();
        cut.Find("[data-testid='card-mode-menu-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: CardModeMenu registers a popover on initialization")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_CardModeMenuRegistersPopover()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var initialCount = popoverService.ActivePopovers.Count;

        var cut = Ctx.Render<CardModeMenu>(p => p.Add(x => x.Mode, CardSut.ModeNormal));

        popoverService.ActivePopovers.Count.ShouldBe(initialCount + 1);
    }

    [Fact(DisplayName = "UAC10: CardModeMenu opens popover on trigger click")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_CardModeMenuOpensPopoverOnClick()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render<CardModeMenu>(p => p.Add(x => x.Mode, CardSut.ModeNormal));

        var trigger = cut.Find("[data-testid='card-mode-menu-trigger']");
        trigger.Click();

        var popover = popoverService.ActivePopovers.Last();
        popover.State.Open.ShouldBeTrue();
        popover.State.Fragment.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: CardModeMenu closes popover and emits mode change on selection")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_CardModeMenuEmitsModeChange()
    {
        string? changedMode = null;
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render<CardModeMenu>(p => p
            .Add(x => x.Mode, CardSut.ModeNormal)
            .Add(x => x.ModeChanged, mode => changedMode = mode));

        var trigger = cut.Find("[data-testid='card-mode-menu-trigger']");
        trigger.Click();

        var popover = popoverService.ActivePopovers.Last();
        popover.State.Open.ShouldBeTrue();

        // Render the popover fragment so we can interact with it
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });
        var items = fragmentCut.FindAll(".node-menu-item");
        items.Count.ShouldBe(5);
        items[2].Click(); // Maximized

        changedMode.ShouldBe("maximized");
        popover.State.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC12: CardModeMenu emits mode change when clicked through PopoverProvider")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_CardModeMenuEmitsModeChangeThroughPopoverProvider()
    {
        string? changedMode = null;

        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<CardModeMenu>(1);
            builder.AddAttribute(2, nameof(CardModeMenu.Mode), CardSut.ModeNormal);
            builder.AddAttribute(3, nameof(CardModeMenu.ModeChanged),
                EventCallback.Factory.Create<string>(this, mode => changedMode = mode));
            builder.CloseComponent();
        });

        cut.Find("[data-testid='card-mode-menu-trigger']").Click();

        cut.WaitForAssertion(() =>
            cut.FindAll(".node-menu-item").Count.ShouldBe(5));

        cut.FindAll(".node-menu-item")
            .Single(item => string.Equals(item.TextContent.Trim(), "Minimized", StringComparison.Ordinal))
            .Click();

        changedMode.ShouldBe("minimized");
    }
}
