

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
            Variant = LowlandTech.Components.Button.ButtonVariant.Outline,
            Size = LowlandTech.Components.Button.ButtonSize.Sm,
            Disabled = true,
            Icon = nameof(Icons.Search)
        };

        var values = (IReadOnlyDictionary<string, string>)CaptureMethod.Invoke(button, null)!;

        values.ShouldContainKey("Variant",
            customMessage: "Variant is decorated with [PropertyGrid] and must be captured.");
        values["Variant"].ShouldBe(LowlandTech.Components.Button.ButtonVariant.Outline);

        values.ShouldContainKey("Size",
            customMessage: "Size is decorated with [PropertyGrid] and must be captured.");
        values["Size"].ShouldBe(LowlandTech.Components.Button.ButtonSize.Sm);

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
        values["Icon"].ShouldBe(LowlandTech.Components.Button.ButtonIcon.None);
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
                    innerBuilder.AddAttribute(9, nameof(ButtonSut.Variant), LowlandTech.Components.Button.ButtonVariant.Ghost);
                    innerBuilder.AddAttribute(10, nameof(ButtonSut.Size), LowlandTech.Components.Button.ButtonSize.Lg);
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
        captured.Values["Variant"].ShouldBe(LowlandTech.Components.Button.ButtonVariant.Ghost);
        captured.Values.ShouldContainKey("Size");
        captured.Values["Size"].ShouldBe(LowlandTech.Components.Button.ButtonSize.Lg);
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
}
