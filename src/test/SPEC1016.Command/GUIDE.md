# Command Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1016 |
| **Namespace** | `LowlandTech.Components.Command` |
| **Root Element** | `<div>` |
| **data-slot** | `command` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/command |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `LtComponentBase` |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1016.Command/
├── Command.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderCommand.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using CommandSut = LowlandTech.Components.Command.Command;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1016")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Command component")]
public class SC01_RenderCommand : ViewScenario
{
    [Fact(DisplayName = "UAC01: Command renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CommandSut>();
        cut.Find("[data-slot='command']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Command.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
