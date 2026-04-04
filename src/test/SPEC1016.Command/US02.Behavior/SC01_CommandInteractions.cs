using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Command;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1016.Command.US02.Behavior;

[Trait(Spec.SPEC, "1016")]
[Trait(Spec.US, "02")]
[UserStory("02", "Command component behavior")]
public class SC01_CommandInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: CommandInput triggers ValueChanged on input")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CommandInputTriggersValueChanged()
    {
        var valueChanged = false;
        string? changedValue = null;

        var cut = Ctx.Render<CommandInput>(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v =>
            {
                valueChanged = true;
                changedValue = v;
            }))
        );

        var input = cut.Find("[data-slot='command-input']");
        await input.InputAsync(new ChangeEventArgs { Value = "test query" });

        valueChanged.ShouldBeTrue();
        changedValue.ShouldBe("test query");
    }

    [Fact(DisplayName = "UAC02: CommandInput triggers OnValueChange on input")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CommandInputTriggersOnValueChange()
    {
        var valueChanged = false;
        string? changedValue = null;

        var cut = Ctx.Render<CommandInput>(p => p
            .Add(x => x.OnValueChange, EventCallback.Factory.Create<string>(this, v =>
            {
                valueChanged = true;
                changedValue = v;
            }))
        );

        var input = cut.Find("[data-slot='command-input']");
        await input.InputAsync(new ChangeEventArgs { Value = "search term" });

        valueChanged.ShouldBeTrue();
        changedValue.ShouldBe("search term");
    }

    [Fact(DisplayName = "UAC03: CommandItem triggers OnSelect when clicked")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_CommandItemTriggersOnSelect()
    {
        var selected = false;

        var cut = Ctx.Render<CommandItem>(p => p
            .Add(x => x.OnSelect, EventCallback.Factory.Create(this, () => selected = true))
        );

        var item = cut.Find("[data-slot='command-item']");
        await item.ClickAsync();

        selected.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: Disabled CommandItem does not trigger OnSelect")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_DisabledCommandItemDoesNotTriggerOnSelect()
    {
        var selected = false;

        var cut = Ctx.Render<CommandItem>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnSelect, EventCallback.Factory.Create(this, () => selected = true))
        );

        var item = cut.Find("[data-slot='command-item']");
        await item.ClickAsync();

        selected.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC05: CommandItem triggers OnHighlight on mouse enter")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_CommandItemTriggersOnHighlight()
    {
        var highlighted = false;

        var cut = Ctx.Render<CommandItem>(p => p
            .Add(x => x.OnHighlight, EventCallback.Factory.Create<bool>(this, h => highlighted = h))
        );

        var item = cut.Find("[data-slot='command-item']");
        await item.MouseEnterAsync(new MouseEventArgs());

        highlighted.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC06: CommandItem updates data-selected on mouse enter")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_CommandItemUpdatesDataSelected()
    {
        var cut = Ctx.Render<CommandItem>();

        var item = cut.Find("[data-slot='command-item']");
        item.HasAttribute("data-selected").ShouldBeFalse();

        await item.MouseEnterAsync(new MouseEventArgs());

        item = cut.Find("[data-slot='command-item']");
        item.GetAttribute("data-selected").ShouldBe("True");
    }

    [Fact(DisplayName = "UAC07: CommandDialog renders closed by default")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CommandDialogRendersClosedByDefault()
    {
        // Setup JSInterop to handle the eval call
        Ctx.JSInterop.SetupVoid("eval", _ => true);
        
        var cut = Ctx.Render<CommandDialog>();
        cut.FindAll(".fixed").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: CommandDialog renders open when IsOpen is true")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_CommandDialogRendersOpenWhenIsOpenTrue()
    {
        // Setup JSInterop to handle the eval call
        Ctx.JSInterop.SetupVoid("eval", _ => true);
        
        var cut = Ctx.Render<CommandDialog>(p => p.Add(x => x.IsOpen, true));
        cut.FindAll(".fixed").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC09: CommandDialog renders with Title and Description")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_CommandDialogRendersTitleAndDescription()
    {
        // Setup JSInterop to handle the eval call
        Ctx.JSInterop.SetupVoid("eval", _ => true);
        
        var cut = Ctx.Render<CommandDialog>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.Title, "Test Title")
            .Add(x => x.Description, "Test Description")
        );

        cut.Find(".sr-only").TextContent.ShouldContain("Test Title");
        cut.Find(".sr-only").TextContent.ShouldContain("Test Description");
    }

    [Fact(DisplayName = "UAC10: CommandDialog renders Trigger when provided")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_CommandDialogRendersTrigger()
    {
        // Setup JSInterop to handle the eval call
        Ctx.JSInterop.SetupVoid("eval", _ => true);
        
        var cut = Ctx.Render<CommandDialog>(p => p
            .Add(x => x.Trigger, builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "class", "trigger-btn");
                builder.AddContent(2, "Open");
                builder.CloseElement();
            })
        );

        cut.Find(".trigger-btn").ShouldNotBeNull();
        cut.Find(".trigger-btn").TextContent.ShouldBe("Open");
    }
}
