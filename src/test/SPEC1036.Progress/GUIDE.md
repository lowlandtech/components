# Progress Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1036 |
| **Namespace** | `LowlandTech.Components.Progress` |
| **Root Element** | `<div>` |
| **data-slot** | `progress` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/progress |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `LtComponentBase` |
| `value` | `string?` | `null` | Component-specific |
| `max` | `string?` | `null` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1036.Progress/
├── Progress.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderProgress.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ProgressSut = LowlandTech.Components.Progress.Progress;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1036")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Progress component")]
public class SC01_RenderProgress : ViewScenario
{
    [Fact(DisplayName = "UAC01: Progress renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-slot='progress']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Progress.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
