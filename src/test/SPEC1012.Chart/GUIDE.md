# Chart Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1012 |
| **Namespace** | `LowlandTech.Components.Chart` |
| **Root Element** | `<div>` |
| **data-slot** | `chart` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/chart |

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
SPEC1012.Chart/
├── Chart.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderChart.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ChartSut = LowlandTech.Components.Chart.Chart;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1012")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Chart component")]
public class SC01_RenderChart : ViewScenario
{
    [Fact(DisplayName = "UAC01: Chart renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ChartSut>();
        cut.Find("[data-slot='chart']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Chart.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
