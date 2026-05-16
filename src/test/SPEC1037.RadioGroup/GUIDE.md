# RadioGroup Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1037 |
| **Namespace** | `LowlandTech.Components.RadioGroup` |
| **Root Element** | `<div>` |
| **data-slot** | `radio-group` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/radio-group |

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
SPEC1037.RadioGroup/
├── RadioGroup.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderRadioGroup.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using RadioGroupSut = LowlandTech.Components.RadioGroup.RadioGroup;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1037")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render RadioGroup component")]
public class SC01_RenderRadioGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: RadioGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<RadioGroupSut>();
        cut.Find("[data-slot='radio-group']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `RadioGroup.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
