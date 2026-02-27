# Checkbox Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1013 |
| **Namespace** | `LowlandTech.Components.Checkbox` |
| **Root Element** | `<button>` |
| **data-slot** | `checkbox` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/checkbox |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `LtComponentBase` |
| `@checked` | `bool` | `false` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1013.Checkbox/
├── Checkbox.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderCheckbox.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using CheckboxSut = LowlandTech.Components.Checkbox.Checkbox;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1013")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Checkbox component")]
public class SC01_RenderCheckbox : ViewScenario
{
    [Fact(DisplayName = "UAC01: Checkbox renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CheckboxSut>();
        cut.Find("[data-slot='checkbox']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Checkbox.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
