# AspectRatio Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1003 |
| **Namespace** | `LowlandTech.Components.AspectRatio` |
| **Root Element** | `<div>` |
| **data-slot** | `aspect-ratio` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/aspect-ratio |

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
SPEC1003.AspectRatio/
├── AspectRatio.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderAspectRatio.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using AspectRatioSut = LowlandTech.Components.AspectRatio.AspectRatio;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1003")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render AspectRatio component")]
public class SC01_RenderAspectRatio : ViewScenario
{
    [Fact(DisplayName = "UAC01: AspectRatio renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AspectRatioSut>();
        cut.Find("[data-slot='aspect-ratio']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `AspectRatio.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
