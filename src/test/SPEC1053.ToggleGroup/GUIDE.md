# ToggleGroup Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1053 |
| **Namespace** | `LowlandTech.Components.ToggleGroup` |
| **Root Element** | `<div>` |
| **data-slot** | `toggle-group` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/toggle-group |

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
SPEC1053.ToggleGroup/
├── ToggleGroup.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderToggleGroup.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ToggleGroupSut = LowlandTech.Components.ToggleGroup.ToggleGroup;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1053")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ToggleGroup component")]
public class SC01_RenderToggleGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: ToggleGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ToggleGroupSut>();
        cut.Find("[data-slot='toggle-group']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `ToggleGroup.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
