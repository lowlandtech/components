# HoverCard Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1024 |
| **Namespace** | `LowlandTech.Components.HoverCard` |
| **Root Element** | `<div>` |
| **data-slot** | `hover-card` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/hover-card |

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
SPEC1024.HoverCard/
├── HoverCard.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderHoverCard.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using HoverCardSut = LowlandTech.Components.HoverCard.HoverCard;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1024")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render HoverCard component")]
public class SC01_RenderHoverCard : ViewScenario
{
    [Fact(DisplayName = "UAC01: HoverCard renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<HoverCardSut>();
        cut.Find("[data-slot='hover-card']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `HoverCard.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
