# Slider Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1045 |
| **Namespace** | `LowlandTech.Components.Slider` |
| **Root Element** | `<span>` |
| **data-slot** | `slider` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/slider |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |
| `min` | `string?` | `null` | Component-specific |
| `max` | `string?` | `null` | Component-specific |
| `step` | `string?` | `null` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1045.Slider/
├── Slider.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSlider.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SliderSut = LowlandTech.Components.Slider.Slider;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1045")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Slider component")]
public class SC01_RenderSlider : ViewScenario
{
    [Fact(DisplayName = "UAC01: Slider renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SliderSut>();
        cut.Find("[data-slot='slider']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Slider.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
