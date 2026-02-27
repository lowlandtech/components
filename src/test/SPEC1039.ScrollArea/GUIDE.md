# ScrollArea Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1039 |
| **Namespace** | `LowlandTech.Components.ScrollArea` |
| **Root Element** | `<div>` |
| **data-slot** | `scroll-area` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/scroll-area |

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
SPEC1039.ScrollArea/
├── ScrollArea.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderScrollArea.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ScrollAreaSut = LowlandTech.Components.ScrollArea.ScrollArea;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1039")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ScrollArea component")]
public class SC01_RenderScrollArea : ViewScenario
{
    [Fact(DisplayName = "UAC01: ScrollArea renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ScrollAreaSut>();
        cut.Find("[data-slot='scroll-area']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `ScrollArea.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
