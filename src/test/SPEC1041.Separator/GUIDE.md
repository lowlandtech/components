# Separator Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1041 |
| **Namespace** | `LowlandTech.Components.Separator` |
| **Root Element** | `<div>` |
| **data-slot** | `separator` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/separator |

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
SPEC1041.Separator/
├── Separator.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSeparator.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SeparatorSut = LowlandTech.Components.Separator.Separator;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1041")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Separator component")]
public class SC01_RenderSeparator : ViewScenario
{
    [Fact(DisplayName = "UAC01: Separator renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SeparatorSut>();
        cut.Find("[data-slot='separator']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Separator.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
