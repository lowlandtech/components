

namespace LowlandTech.Components.Tests.SPEC1068.Designable.US01.Registry;

/// <summary>
/// SC03 — When a component is selected in design mode, <see cref="TinyComponentBase"/>
/// must automatically reflect over every <see cref="PropertyGridAttribute"/>-decorated
/// parameter and capture its current value. This scenario guards against the loop
/// silently dropping properties (e.g. because of an incorrect <c>inherit</c> flag,
/// a missing <c>ParameterAttribute</c> check, or a broken <c>GetValue</c> call).
/// </summary>
[Trait(Spec.SPEC, "1068")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Design-mode selection automatically captures all [PropertyGrid] parameter values")]
public class SC03_DesignValueCapture
{
    private static readonly MethodInfo CaptureMethod = typeof(TinyComponentBase)
        .GetMethod("CaptureDesignValues", BindingFlags.NonPublic | BindingFlags.Instance)!;

    [Fact(DisplayName = "UAC21: CaptureDesignValues loops every [PropertyGrid] parameter on Button")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_ButtonCapturesAllPropertyGridValues()
    {
        var button = new ButtonSut
        {
            Variant = LowlandTech.Components.ButtonVariant.Outline,
            Size = LowlandTech.Components.ButtonSize.Sm,
            Disabled = true,
            Icon = nameof(Icons.Search)
        };

        var values = (IReadOnlyDictionary<string, string>)CaptureMethod.Invoke(button, null)!;

        values.ShouldContainKey("Variant",
            customMessage: "Variant is decorated with [PropertyGrid] and must be captured.");
        values["Variant"].ShouldBe(LowlandTech.Components.ButtonVariant.Outline);

        values.ShouldContainKey("Size",
            customMessage: "Size is decorated with [PropertyGrid] and must be captured.");
        values["Size"].ShouldBe(LowlandTech.Components.ButtonSize.Sm);

        values.ShouldContainKey("Disabled",
            customMessage: "Disabled is decorated with [PropertyGrid] and must be captured.");
        values["Disabled"].ShouldBe("True");

        values.ShouldContainKey("Icon",
            customMessage: "Icon is decorated with [PropertyGrid] and must be captured.");
        values["Icon"].ShouldBe(nameof(Icons.Search));
    }

    [Fact(DisplayName = "UAC22: CaptureDesignValues includes default values for unset parameters")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_UnsetParametersCaptureDefaults()
    {
        var button = new ButtonSut();

        var values = (IReadOnlyDictionary<string, string>)CaptureMethod.Invoke(button, null)!;

        values["Variant"].ShouldBe("default");
        values["Size"].ShouldBe("default");
        values["Disabled"].ShouldBe("False");
        values["Icon"].ShouldBe(LowlandTech.Components.ButtonIcon.None);
    }

    [Fact(DisplayName = "UAC23: CaptureDesignValues skips plain [Parameter] properties without [PropertyGrid]")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_PlainParametersAreSkipped()
    {
        var button = new ButtonSut();

        var values = (IReadOnlyDictionary<string, string>)CaptureMethod.Invoke(button, null)!;

        values.ShouldNotContainKey("OnClick",
            customMessage: "OnClick has [Parameter] but no [PropertyGrid] — it must NOT leak into the captured values.");
        values.ShouldNotContainKey("Class",
            customMessage: "Class has [Parameter] but no [PropertyGrid] — it must NOT leak into the captured values.");
    }

    [Fact(DisplayName = "UAC24: design-mode click on Button emits TinyDesignSelection with all values")]
    [Trait(Spec.UAC, "24")]
    [Given("A Button is rendered in design mode with multiple [PropertyGrid] values")]
    [When("The Button is clicked")]
    [Then("The emitted design selection includes the selected component type and captured values")]
    public void UAC24_DesignModeClickEmitsAllValues()
    {
        var ctx = new BunitContext();
        ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        ctx.Services.AddSingleton<IPopoverService, PopoverService>();

        TinyDesignSelection? captured = null;
        var callback = EventCallback.Factory.Create<TinyDesignSelection>(this, s => captured = s);

        var cut = ctx.Render(builder =>
        {
            builder.OpenComponent<CascadingValue<bool>>(0);
            builder.AddAttribute(1, "Name", "TinyDesignMode");
            builder.AddAttribute(2, "Value", true);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<CascadingValue<EventCallback<TinyDesignSelection>>>(4);
                childBuilder.AddAttribute(5, "Name", "TinyDesignSelected");
                childBuilder.AddAttribute(6, "Value", callback);
                childBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(innerBuilder =>
                {
                    innerBuilder.OpenComponent<ButtonSut>(8);
                    innerBuilder.AddAttribute(9, nameof(ButtonSut.Variant), LowlandTech.Components.ButtonVariant.Ghost);
                    innerBuilder.AddAttribute(10, nameof(ButtonSut.Size), LowlandTech.Components.ButtonSize.Lg);
                    innerBuilder.AddAttribute(11, nameof(ButtonSut.Disabled), true);
                    innerBuilder.AddAttribute(12, nameof(ButtonSut.Icon), nameof(Icons.Search));
                    innerBuilder.CloseComponent();
                }));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("[data-slot='button']").Click();

        captured.ShouldNotBeNull();
        captured!.ComponentType.ShouldBe(typeof(ButtonSut));
        captured.Values.ShouldContainKey("Variant");
        captured.Values["Variant"].ShouldBe(LowlandTech.Components.ButtonVariant.Ghost);
        captured.Values.ShouldContainKey("Size");
        captured.Values["Size"].ShouldBe(LowlandTech.Components.ButtonSize.Lg);
        captured.Values.ShouldContainKey("Disabled");
        captured.Values["Disabled"].ShouldBe("True");
        captured.Values.ShouldContainKey("Icon");
        captured.Values["Icon"].ShouldBe(nameof(Icons.Search));
    }

    [Fact(DisplayName = "UAC25: design-mode click includes stable design persistence key")]
    [Trait(Spec.UAC, "25")]
    [Given("A Button is rendered in design mode with a stable design persistence key")]
    [When("The Button is clicked")]
    [Then("The emitted design selection carries the same persistence key for reload hydration")]
    public void UAC25_DesignModeClickEmitsPersistenceKey()
    {
        var ctx = new BunitContext();
        ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        ctx.Services.AddSingleton<IPopoverService, PopoverService>();

        TinyDesignSelection? captured = null;
        var callback = EventCallback.Factory.Create<TinyDesignSelection>(this, s => captured = s);

        var cut = ctx.Render(builder =>
        {
            builder.OpenComponent<CascadingValue<bool>>(0);
            builder.AddAttribute(1, "Name", "TinyDesignMode");
            builder.AddAttribute(2, "Value", true);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<CascadingValue<EventCallback<TinyDesignSelection>>>(4);
                childBuilder.AddAttribute(5, "Name", "TinyDesignSelected");
                childBuilder.AddAttribute(6, "Value", callback);
                childBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(innerBuilder =>
                {
                    innerBuilder.OpenComponent<ButtonSut>(8);
                    innerBuilder.AddAttribute(9, nameof(ButtonSut.DesignPersistenceKey), "button:primary");
                    innerBuilder.CloseComponent();
                }));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("[data-slot='button']").Click();

        captured.ShouldNotBeNull();
        captured!.PersistenceKey.ShouldBe("button:primary");
    }

    [Fact(DisplayName = "UAC26: design registry resolves a live instance by stable persistence key")]
    [Trait(Spec.UAC, "26")]
    [Given("A designable Button is registered on a design surface with a stable persistence key")]
    [When("The runtime instance id is resolved by that persistence key")]
    [Then("The registry returns the live instance id created for the rendered Button")]
    public void UAC26_DesignRegistryResolvesInstanceByPersistenceKey()
    {
        const string surfaceId = "surface:selection-reload";

        var ctx = new BunitContext();
        ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        ctx.Services.AddSingleton<IPopoverService, PopoverService>();

        var cut = ctx.Render(builder =>
        {
            builder.OpenComponent<CascadingValue<bool>>(0);
            builder.AddAttribute(1, "Name", "TinyDesignMode");
            builder.AddAttribute(2, "Value", true);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<CascadingValue<string>>(4);
                childBuilder.AddAttribute(5, "Name", "TinyDesignSurfaceId");
                childBuilder.AddAttribute(6, "Value", surfaceId);
                childBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(innerBuilder =>
                {
                    innerBuilder.OpenComponent<ButtonSut>(8);
                    innerBuilder.AddAttribute(9, nameof(ButtonSut.DesignPersistenceKey), "button:primary");
                    innerBuilder.CloseComponent();
                }));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var instanceId = cut.Find("[data-slot='button']").GetAttribute("data-design-instance-id");
        var resolved = TinyComponentBase.FindInstanceIdByPersistenceKey(
            typeof(ButtonSut),
            surfaceId,
            "button:primary");

        resolved.ShouldBe(instanceId);
    }

    [Fact(DisplayName = "UAC27: TreeViewItem emits cascaded node persistence key")]
    [Trait(Spec.UAC, "27")]
    [Given("A TreeViewItem is rendered inside a TreeNode design persistence key cascade")]
    [When("The TreeViewItem is clicked in design mode")]
    [Then("The emitted design selection uses the cascaded tree node key")]
    public void UAC27_TreeViewItemEmitsCascadedNodePersistenceKey()
    {
        var ctx = new BunitContext();
        ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        ctx.Services.AddSingleton<IPopoverService, PopoverService>();

        TinyDesignSelection? captured = null;
        var callback = EventCallback.Factory.Create<TinyDesignSelection>(this, s => captured = s);

        var cut = ctx.Render(builder =>
        {
            builder.OpenComponent<CascadingValue<bool>>(0);
            builder.AddAttribute(1, "Name", "TinyDesignMode");
            builder.AddAttribute(2, "Value", true);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<CascadingValue<EventCallback<TinyDesignSelection>>>(4);
                childBuilder.AddAttribute(5, "Name", "TinyDesignSelected");
                childBuilder.AddAttribute(6, "Value", callback);
                childBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(selectionBuilder =>
                {
                    selectionBuilder.OpenComponent<CascadingValue<string>>(8);
                    selectionBuilder.AddAttribute(9, "Name", "TreeViewNodeDesignPersistenceKey");
                    selectionBuilder.AddAttribute(10, "Value", "treeview-item:Button.razor");
                    selectionBuilder.AddAttribute(11, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<TreeViewItem>(12);
                        itemBuilder.AddAttribute(13, nameof(TreeViewItem.Icon), "file");
                        itemBuilder.AddAttribute(14, nameof(TreeViewItem.Suffix), "42");
                        itemBuilder.CloseComponent();
                    }));
                    selectionBuilder.CloseComponent();
                }));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var item = cut.Find("[data-slot='tree-view-item']");
        item.GetAttribute("data-design-persistence-key").ShouldBe("treeview-item:Button.razor");

        item.Click();

        captured.ShouldNotBeNull();
        captured!.PersistenceKey.ShouldBe("treeview-item:Button.razor");
        captured.ComponentType.ShouldBe(typeof(TreeViewItem));
        captured.Values["Suffix"].ShouldBe("42");
    }

    [Fact(DisplayName = "UAC28: stale instance id cannot hydrate values onto mismatched TreeViewItem key")]
    [Trait(Spec.UAC, "28")]
    [Given("A stale TreeViewItem instance id is restored with a different persisted tree node key")]
    [When("The design surface hydrates persisted property values")]
    [Then("The mismatched instance is not selected and does not receive the persisted values")]
    public void UAC28_StaleInstanceIdDoesNotHydrateMismatchedTreeViewItem()
    {
        var ctx = new BunitContext();
        ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        ctx.Services.AddSingleton<IPopoverService, PopoverService>();

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Icon"] = "file",
            ["Suffix"] = "42"
        };

        var cut = ctx.Render<TreeViewItemHydrationHarness>();

        var staleSrcInstanceId = cut.Find("[data-design-persistence-key='treeview-item:src']")
            .GetAttribute("data-design-instance-id");
        var buttonInstanceId = cut.Find("[data-design-persistence-key='treeview-item:Button.razor']")
            .GetAttribute("data-design-instance-id");

        cut.Instance.SelectedInstanceId = staleSrcInstanceId;
        cut.Instance.SelectedPersistenceKey = "treeview-item:Button.razor";
        cut.Instance.PropertyValues = values;
        cut.Render();

        var src = cut.Find("[data-design-persistence-key='treeview-item:src']");
        src.GetAttribute("data-design-selected").ShouldBeNull();
        src.TextContent.ShouldNotContain("42");

        cut.Instance.SelectedInstanceId = buttonInstanceId;
        cut.Render();

        var button = cut.Find("[data-design-persistence-key='treeview-item:Button.razor']");
        button.GetAttribute("data-design-selected").ShouldBe("true");
        button.TextContent.ShouldContain("42");
    }

    private static RenderFragment RenderTreeViewItems() => builder =>
    {
        builder.OpenComponent<CascadingValue<string>>(0);
        builder.AddAttribute(1, "Name", "TreeViewNodeDesignPersistenceKey");
        builder.AddAttribute(2, "Value", "treeview-item:src");
        builder.AddAttribute(3, "ChildContent", (RenderFragment)(srcBuilder =>
        {
            srcBuilder.OpenComponent<TreeViewItem>(4);
            srcBuilder.AddAttribute(5, nameof(TreeViewItem.Icon), "folder");
            srcBuilder.AddAttribute(6, "ChildContent", (RenderFragment)(labelBuilder =>
            {
                labelBuilder.AddContent(7, "src");
            }));
            srcBuilder.CloseComponent();
        }));
        builder.CloseComponent();

        builder.OpenComponent<CascadingValue<string>>(8);
        builder.AddAttribute(9, "Name", "TreeViewNodeDesignPersistenceKey");
        builder.AddAttribute(10, "Value", "treeview-item:Button.razor");
        builder.AddAttribute(11, "ChildContent", (RenderFragment)(buttonBuilder =>
        {
            buttonBuilder.OpenComponent<TreeViewItem>(12);
            buttonBuilder.AddAttribute(13, nameof(TreeViewItem.Icon), "file");
            buttonBuilder.AddAttribute(14, "ChildContent", (RenderFragment)(labelBuilder =>
            {
                labelBuilder.AddContent(15, "Button.razor");
            }));
            buttonBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    };

    private sealed class TreeViewItemHydrationHarness : ComponentBase
    {
        public string? SelectedInstanceId { get; set; }
        public string? SelectedPersistenceKey { get; set; }
        public IReadOnlyDictionary<string, string>? PropertyValues { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<bool>>(0);
            builder.AddAttribute(1, "Name", "TinyDesignMode");
            builder.AddAttribute(2, "Value", true);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(designBuilder =>
            {
                designBuilder.OpenComponent<CascadingValue<string?>>(4);
                designBuilder.AddAttribute(5, "Name", "TinySelectedDesignInstanceId");
                designBuilder.AddAttribute(6, "Value", SelectedInstanceId);
                designBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(selectedBuilder =>
                {
                    selectedBuilder.OpenComponent<CascadingValue<string?>>(8);
                    selectedBuilder.AddAttribute(9, "Name", "TinySelectedDesignPersistenceKey");
                    selectedBuilder.AddAttribute(10, "Value", SelectedPersistenceKey);
                    selectedBuilder.AddAttribute(11, "ChildContent", (RenderFragment)(keyBuilder =>
                    {
                        keyBuilder.OpenComponent<CascadingValue<IReadOnlyDictionary<string, string>?>>(12);
                        keyBuilder.AddAttribute(13, "Name", "TinyDesignPropertyValues");
                        keyBuilder.AddAttribute(14, "Value", PropertyValues);
                        keyBuilder.AddAttribute(15, "ChildContent", RenderTreeViewItems());
                        keyBuilder.CloseComponent();
                    }));
                    selectedBuilder.CloseComponent();
                }));
                designBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }
}
