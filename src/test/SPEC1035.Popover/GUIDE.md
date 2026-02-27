# Popover Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1035 |
| **Namespace** | `LowlandTech.Components.Popover` |
| **Root Element** | `<div>` |
| **data-slot** | `popover` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/popover |

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
SPEC1035.Popover/
├── Popover.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderPopover.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using PopoverSut = LowlandTech.Components.Popover.Popover;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Popover component")]
public class SC01_RenderPopover : ViewScenario
{
    [Fact(DisplayName = "UAC01: Popover renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PopoverSut>();
        cut.Find("[data-slot='popover']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Popover.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
