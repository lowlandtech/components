# Direction Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1019 |
| **Namespace** | `LowlandTech.Components.Direction` |
| **Root Element** | `<div>` |
| **data-slot** | `direction-provider` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/direction |

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
SPEC1019.Direction/
├── Direction.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderDirection.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using DirectionSut = LowlandTech.Components.Direction.Direction;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1019")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Direction component")]
public class SC01_RenderDirection : ViewScenario
{
    [Fact(DisplayName = "UAC01: Direction renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DirectionSut>();
        cut.Find("[data-slot='direction-provider']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Direction.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
