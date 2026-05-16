# Carousel Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1011 |
| **Namespace** | `LowlandTech.Components.Carousel` |
| **Root Element** | `<div>` |
| **data-slot** | `carousel` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/carousel |

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
SPEC1011.Carousel/
├── Carousel.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderCarousel.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using CarouselSut = LowlandTech.Components.Carousel.Carousel;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1011")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Carousel component")]
public class SC01_RenderCarousel : ViewScenario
{
    [Fact(DisplayName = "UAC01: Carousel renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CarouselSut>();
        cut.Find("[data-slot='carousel']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Carousel.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
