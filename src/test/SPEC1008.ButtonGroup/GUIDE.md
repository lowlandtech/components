# ButtonGroup Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1008 |
| **Namespace** | `LowlandTech.Components.ButtonGroup` |
| **Root Element** | `<div>` |
| **data-slot** | `button-group` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/button-group |

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
SPEC1008.ButtonGroup/
├── ButtonGroup.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderButtonGroup.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ButtonGroupSut = LowlandTech.Components.ButtonGroup.ButtonGroup;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1008")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ButtonGroup component")]
public class SC01_RenderButtonGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: ButtonGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ButtonGroupSut>();
        cut.Find("[data-slot='button-group']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `ButtonGroup.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
