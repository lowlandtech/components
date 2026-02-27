# Field Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1023 |
| **Namespace** | `LowlandTech.Components.Field` |
| **Root Element** | `<div>` |
| **data-slot** | `field` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/field |

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
SPEC1023.Field/
├── Field.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderField.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using FieldSut = LowlandTech.Components.Field.Field;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1023")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Field component")]
public class SC01_RenderField : ViewScenario
{
    [Fact(DisplayName = "UAC01: Field renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<FieldSut>();
        cut.Find("[data-slot='field']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Field.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
