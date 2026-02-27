# Select Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1040 |
| **Namespace** | `LowlandTech.Components.Select` |
| **Root Element** | `<div>` |
| **data-slot** | `select` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/select |

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
SPEC1040.Select/
├── Select.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSelect.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SelectSut = LowlandTech.Components.Select.Select;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1040")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Select component")]
public class SC01_RenderSelect : ViewScenario
{
    [Fact(DisplayName = "UAC01: Select renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SelectSut>();
        cut.Find("[data-slot='select']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Select.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
