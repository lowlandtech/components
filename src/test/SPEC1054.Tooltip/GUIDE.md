# Tooltip Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1054 |
| **Namespace** | `LowlandTech.Components.Tooltip` |
| **Root Element** | `<div>` |
| **data-slot** | `tooltip-provider` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/tooltip |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1054.Tooltip/
├── Tooltip.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderTooltip.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using TooltipSut = LowlandTech.Components.Tooltip.Tooltip;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1054")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Tooltip component")]
public class SC01_RenderTooltip : ViewScenario
{
    [Fact(DisplayName = "UAC01: Tooltip renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TooltipSut>();
        cut.Find("[data-slot='tooltip-provider']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Tooltip.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
