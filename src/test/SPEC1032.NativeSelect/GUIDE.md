# NativeSelect Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1032 |
| **Namespace** | `LowlandTech.Components.NativeSelect` |
| **Root Element** | `<div>` |
| **data-slot** | `native-select` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/native-select |

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
SPEC1032.NativeSelect/
├── NativeSelect.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderNativeSelect.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using NativeSelectSut = LowlandTech.Components.NativeSelect.NativeSelect;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1032")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render NativeSelect component")]
public class SC01_RenderNativeSelect : ViewScenario
{
    [Fact(DisplayName = "UAC01: NativeSelect renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<NativeSelectSut>();
        cut.Find("[data-slot='native-select']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `NativeSelect.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
