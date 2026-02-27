# Skeleton Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1044 |
| **Namespace** | `LowlandTech.Components.Skeleton` |
| **Root Element** | `<div>` |
| **data-slot** | `skeleton` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/skeleton |

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
SPEC1044.Skeleton/
├── Skeleton.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSkeleton.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SkeletonSut = LowlandTech.Components.Skeleton.Skeleton;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1044")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Skeleton component")]
public class SC01_RenderSkeleton : ViewScenario
{
    [Fact(DisplayName = "UAC01: Skeleton renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SkeletonSut>();
        cut.Find("[data-slot='skeleton']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Skeleton.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
