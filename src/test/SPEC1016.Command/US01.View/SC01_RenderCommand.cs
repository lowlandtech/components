using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1016.Command.US01.View;

[Trait(Spec.SPEC, "1016")]
[Trait(Spec.US, "01")]
[UserStory("01", "Command component rendering")]
public class SC01_RenderCommand : ViewScenario
{
    [Fact(DisplayName = "UAC01: CommandContainer renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CommandContainerRendersWithDataSlot()
    {
        var cut = Ctx.Render<CommandContainer>();
        cut.Find("[data-slot='command']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CommandContainer renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CommandContainerRendersChildContent()
    {
        var cut = Ctx.Render<CommandContainer>(p => p.AddChildContent("<div class='test-content'>Test</div>"));
        cut.Find(".test-content").TextContent.ShouldBe("Test");
    }

    [Fact(DisplayName = "UAC03: CommandInput renders with data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CommandInputRendersWithDataSlot()
    {
        var cut = Ctx.Render<CommandInput>();
        cut.Find("[data-slot='command-input-wrapper']").ShouldNotBeNull();
        cut.Find("[data-slot='command-input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: CommandInput renders with placeholder")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_CommandInputRendersWithPlaceholder()
    {
        var cut = Ctx.Render<CommandInput>(p => p.Add(x => x.Placeholder, "Search..."));
        cut.Find("[data-slot='command-input']").GetAttribute("placeholder").ShouldBe("Search...");
    }

    [Fact(DisplayName = "UAC05: CommandList renders with data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_CommandListRendersWithDataSlot()
    {
        var cut = Ctx.Render<CommandList>();
        cut.Find("[data-slot='command-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: CommandGroup renders with data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_CommandGroupRendersWithDataSlot()
    {
        var cut = Ctx.Render<CommandGroup>();
        cut.Find("[data-slot='command-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: CommandGroup renders heading when provided")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CommandGroupRendersHeading()
    {
        var cut = Ctx.Render<CommandGroup>(p => p.Add(x => x.Heading, "Test Group"));
        cut.Find("[data-slot='command-group']").TextContent.ShouldContain("Test Group");
    }

    [Fact(DisplayName = "UAC08: CommandItem renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_CommandItemRendersWithDataSlot()
    {
        var cut = Ctx.Render<CommandItem>();
        var item = cut.Find("[data-slot='command-item']");
        item.ShouldNotBeNull();
        item.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC09: CommandItem renders with data-value attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_CommandItemRendersWithDataValue()
    {
        var cut = Ctx.Render<CommandItem>(p => p.Add(x => x.Value, "test-value"));
        cut.Find("[data-slot='command-item']").GetAttribute("data-value").ShouldBe("test-value");
    }

    [Fact(DisplayName = "UAC10: CommandItem respects Disabled parameter")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_CommandItemRespectsDisabled()
    {
        var cut = Ctx.Render<CommandItem>(p => p.Add(x => x.Disabled, true));
        cut.Find("[data-slot='command-item']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC11: CommandSeparator renders")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_CommandSeparatorRenders()
    {
        var cut = Ctx.Render<CommandSeparator>();
        cut.Find("[data-slot='command-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: CommandShortcut renders content")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_CommandShortcutRendersContent()
    {
        var cut = Ctx.Render<CommandShortcut>(p => p.AddChildContent("Ctrl+K"));
        cut.Find("[data-slot='command-shortcut']").TextContent.ShouldContain("Ctrl+K");
    }

    [Fact(DisplayName = "UAC13: CommandEmpty renders with data-slot attribute")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_CommandEmptyRendersWithDataSlot()
    {
        var cut = Ctx.Render<CommandEmpty>();
        cut.Find("[data-slot='command-empty']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC14: Full Command SUT renders correctly")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_FullCommandSutRenders()
    {
        var cut = Ctx.Render<CommandContainer>(p => p.AddChildContent(@"
            <div data-slot='command-input'></div>
            <div data-slot='command-list'>
                <div data-slot='command-group'>
                    <button data-slot='command-item'></button>
                    <button data-slot='command-item'></button>
                </div>
                <div data-slot='command-separator'></div>
                <div data-slot='command-group'>
                    <button data-slot='command-item'></button>
                </div>
            </div>"));
        cut.Find("[data-slot='command']").ShouldNotBeNull();
        cut.Find("[data-slot='command-input']").ShouldNotBeNull();
        cut.Find("[data-slot='command-list']").ShouldNotBeNull();
        cut.FindAll("[data-slot='command-group']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='command-item']").Count.ShouldBe(3);
        cut.Find("[data-slot='command-separator']").ShouldNotBeNull();
    }
}
