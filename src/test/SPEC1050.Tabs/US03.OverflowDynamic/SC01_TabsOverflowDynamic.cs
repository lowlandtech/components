namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US03.OverflowDynamic;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "Overflow and dynamic IDE tabs")]
public class SC01_TabsOverflowDynamic : ViewScenario
{
    [Fact(DisplayName = "UAC01: TabsList hides scroll buttons until overflow is detected")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TabsListHidesScrollButtonsUntilOverflow()
    {
        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p.AddChildContent<TabsList>(list => list
            .Add(l => l.ShowScrollButtons, true)
            .AddChildContent<TabsTrigger>(trigger => trigger
                .Add(t => t.Value, "tab-1")
                .AddChildContent("Tab 1"))));

        cut.Find("[data-slot='tabs-list-shell']").ShouldNotBeNull();
        cut.Find("[data-slot='tabs-list-viewport']").ShouldNotBeNull();
        cut.FindAll("[data-slot='tabs-scroll-button']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC07: TabsList shows scroll buttons after overflow is detected")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_TabsListShowsScrollButtonsAfterOverflow()
    {
        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p.AddChildContent<TabsList>(list => list
            .Add(l => l.ShowScrollButtons, true)
            .AddChildContent<TabsTrigger>(trigger => trigger
                .Add(t => t.Value, "tab-1")
                .AddChildContent("Tab 1"))));

        var tabsList = cut.FindComponent<TabsList>();
        await tabsList.Instance.OnTabsOverflowChanged(true);

        cut.Find("[data-slot='tabs-scroll-button'][data-direction='back']").ShouldNotBeNull();
        cut.Find("[data-slot='tabs-scroll-button'][data-direction='next']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Dynamic tabs render from item collection")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DynamicTabsRenderItems()
    {
        var items = new[]
        {
            new TabsItem { Value = "one", Title = "One" },
            new TabsItem { Value = "two", Title = "Two" }
        };

        RenderFragment<TabsItem> content = item => builder => builder.AddContent(0, $"Content {item.Title}");

        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p
            .Add(t => t.Items, items)
            .Add(t => t.ItemContent, content));

        cut.FindAll("[data-slot='tabs-trigger']").Count.ShouldBe(2);
        cut.Markup.ShouldContain("One");
        cut.Markup.ShouldContain("Content One");
    }

    [Fact(DisplayName = "UAC03: Closeable tabs emit ItemsChanged without the closed item")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CloseableTabRemovesItem()
    {
        IReadOnlyList<TabsItem>? changed = null;
        var items = new[]
        {
            new TabsItem { Value = "one", Title = "One", Closeable = true },
            new TabsItem { Value = "two", Title = "Two", Closeable = true }
        };

        var callback = EventCallback.Factory.Create<IReadOnlyList<TabsItem>>(this, next => changed = next);

        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p
            .Add(t => t.Items, items)
            .Add(t => t.ItemsChanged, callback));

        cut.Find("[data-slot='tabs-close-button']").Click();

        changed.ShouldNotBeNull();
        changed.Count.ShouldBe(1);
        changed[0].Value.ShouldBe("two");
    }

    [Fact(DisplayName = "UAC04: Pinnable tabs emit pinned update")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_PinButtonTogglesPinnedState()
    {
        IReadOnlyList<TabsItem>? changed = null;
        var items = new[]
        {
            new TabsItem { Value = "one", Title = "One", Pinnable = true },
            new TabsItem { Value = "two", Title = "Two" }
        };

        var callback = EventCallback.Factory.Create<IReadOnlyList<TabsItem>>(this, next => changed = next);

        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p
            .Add(t => t.Items, items)
            .Add(t => t.ItemsChanged, callback));

        cut.Find("[data-slot='tabs-pin-button']").Click();

        changed.ShouldNotBeNull();
        changed.Single(x => x.Value == "one").Pinned.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC05: IDE variant marks active tab with IDE state classes")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_IdeVariantMarksActiveTab()
    {
        var items = new[]
        {
            new TabsItem { Value = "one", Title = "One" },
            new TabsItem { Value = "two", Title = "Two" }
        };

        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p
            .Add(t => t.Items, items)
            .Add(t => t.Value, "two")
            .Add(t => t.DynamicVariant, "ide"));

        var active = cut.Find("[data-slot='tabs-trigger'][data-state='active']");
        active.TextContent.ShouldContain("Two");
        active.ClassList.ShouldContain("cn-tabs-trigger-ide");
    }

    [Fact(DisplayName = "UAC06: Dropping one dynamic tab on another emits reordered items")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DropReordersDynamicTabs()
    {
        IReadOnlyList<TabsItem>? changed = null;
        var items = new[]
        {
            new TabsItem { Value = "one", Title = "One", Draggable = true },
            new TabsItem { Value = "two", Title = "Two", Draggable = true },
            new TabsItem { Value = "three", Title = "Three", Draggable = true }
        };

        var callback = EventCallback.Factory.Create<IReadOnlyList<TabsItem>>(this, next => changed = next);

        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p
            .Add(t => t.Items, items)
            .Add(t => t.ItemsChanged, callback));

        var triggers = cut.FindAll("[data-slot='tabs-trigger']");
        triggers[2].DragStart(new DragEventArgs());
        triggers[0].Drop(new DragEventArgs());

        changed.ShouldNotBeNull();
        changed.Select(x => x.Value).ShouldBe(["three", "one", "two"]);
    }

    [Fact(DisplayName = "UAC25: Tab header keeps the default cursor")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_TabHeaderKeepsDefaultCursor()
    {
        var cut = Ctx.Render<LowlandTech.Components.Tabs.Tabs>(p => p.AddChildContent<TabsList>(list => list
            .AddChildContent<TabsTrigger>(trigger => trigger
                .Add(t => t.Value, "tab-1")
                .Add(t => t.Draggable, true)
                .AddChildContent("Tab 1"))));

        cut.Find("[data-slot='tabs-trigger']").ClassList.ShouldContain("cursor-default");
    }
}
