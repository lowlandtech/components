# Combobox Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1015 |
| **Namespace** | `LowlandTech.Components.Combobox` |
| **Root Element** | `<div>` |
| **data-slot** | `combobox` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/combobox |

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
SPEC1015.Combobox/
├── Combobox.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderCombobox.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ComboboxSut = LowlandTech.Components.Combobox.Combobox;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1015")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Combobox component")]
public class SC01_RenderCombobox : ViewScenario
{
    [Fact(DisplayName = "UAC01: Combobox renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ComboboxSut>();
        cut.Find("[data-slot='combobox']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Combobox.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
