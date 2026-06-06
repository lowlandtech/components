namespace LowlandTech.Components.Tests.SPEC1078.Session.US03.BlockDiscovery;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "03")]
[UserStory("03", "Session block descriptors resolve component types through DRD discovery")]
public class SC01_SessionBlockDiscovery : ViewScenario
{
    private sealed class PluginSessionBlockRegistry : ISessionBlockRegistry
    {
        public int Priority => 100;

        public IReadOnlyList<ISessionBlockDescriptor> GetDescriptors() =>
        [
            new SessionBlockDescriptor("plugin.order", typeof(SessionCard), "Order Card", "shopping-cart", Priority: 100)
        ];
    }

    [Fact(DisplayName = "UAC01: Default registry resolves session node type to SessionCard")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DefaultRegistryResolvesSessionNode()
    {
        var discovery = Ctx.Services.GetRequiredService<ISessionBlockDiscovery>();
        var type = discovery.ResolveComponentType("session");

        type.ShouldNotBeNull();
        type.ShouldBe(typeof(SessionCard));
    }

    [Fact(DisplayName = "UAC02: Default registry resolves session.pin node type to SessionContextPin")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DefaultRegistryResolvesSessionPinNode()
    {
        var discovery = Ctx.Services.GetRequiredService<ISessionBlockDiscovery>();
        var type = discovery.ResolveComponentType("session.pin");

        type.ShouldNotBeNull();
        type.ShouldBe(typeof(SessionContextPin));
    }

    [Fact(DisplayName = "UAC03: Discovery returns null for unknown node type keys")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_UnknownNodeTypeReturnsNull()
    {
        var discovery = Ctx.Services.GetRequiredService<ISessionBlockDiscovery>();
        var type = discovery.ResolveComponentType("unknown.type");

        type.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC04: Discovery returns null for empty or null keys")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EmptyKeyReturnsNull()
    {
        var discovery = Ctx.Services.GetRequiredService<ISessionBlockDiscovery>();

        discovery.ResolveComponentType(null).ShouldBeNull();
        discovery.ResolveComponentType("").ShouldBeNull();
        discovery.ResolveComponentType("   ").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC05: Plugin registry descriptors are discoverable")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_PluginRegistryDescriptorsAreDiscoverable()
    {
        Ctx.Services.AddSingleton<ISessionBlockRegistry, PluginSessionBlockRegistry>();
        var discovery = new SessionBlockDiscovery(
            Ctx.Services.GetServices<ISessionBlockRegistry>());

        var descriptors = discovery.GetDescriptors();

        descriptors.ShouldContain(d => d.NodeTypeKey == "plugin.order");
    }

    [Fact(DisplayName = "UAC06: Plugin registry takes priority over default registry")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PluginRegistryTakesPriority()
    {
        Ctx.Services.AddSingleton<ISessionBlockRegistry, PluginSessionBlockRegistry>();
        var discovery = new SessionBlockDiscovery(
            Ctx.Services.GetServices<ISessionBlockRegistry>());

        var descriptor = discovery.GetDescriptor("plugin.order");

        descriptor.ShouldNotBeNull();
        descriptor.ComponentType.ShouldBe(typeof(SessionCard));
        descriptor.Label.ShouldBe("Order Card");
    }

    [Fact(DisplayName = "UAC07: SessionContextBlock resolves component through injected discovery")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_SessionContextBlockResolvesComponentThroughDiscovery()
    {
        var node = new SessionNodeInput("test", "session", "Test");
        var block = new SessionContextBlockModel("test-block", node);
        var cut = Ctx.Render<SessionContextBlock>(parameters => parameters
            .Add(p => p.Block, block)
            .Add(p => p.Mode, SessionContextBlock.ModeNormal));

        cut.ShouldNotBeNull();
        cut.Find("[data-slot='session-context-block']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: SessionContextBlockOverlayContent resolves component through injected discovery")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_OverlayContentResolvesComponentThroughDiscovery()
    {
        var node = new SessionNodeInput("test", "session.pin", "Pinned Test");
        var block = new SessionContextBlockModel("test-block", node);
        var cut = Ctx.Render<SessionContextBlockOverlayContent>(parameters => parameters
            .Add(p => p.Block, block));

        cut.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: GetDescriptor returns correct descriptor metadata")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_GetDescriptorReturnsMetadata()
    {
        var discovery = Ctx.Services.GetRequiredService<ISessionBlockDiscovery>();
        var descriptor = discovery.GetDescriptor("workitem");

        descriptor.ShouldNotBeNull();
        descriptor.NodeTypeKey.ShouldBe("workitem");
        descriptor.ComponentType.ShouldBe(typeof(SessionCard));
        descriptor.Label.ShouldBe("Workitem");
        descriptor.Icon.ShouldBe("briefcase");
    }

    [Fact(DisplayName = "UAC10: Discovery is case-insensitive for node type keys")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_DiscoveryIsCaseInsensitive()
    {
        var discovery = Ctx.Services.GetRequiredService<ISessionBlockDiscovery>();

        discovery.ResolveComponentType("SESSION").ShouldBe(typeof(SessionCard));
        discovery.ResolveComponentType("Session").ShouldBe(typeof(SessionCard));
        discovery.ResolveComponentType("session").ShouldBe(typeof(SessionCard));
    }
}
