# Sheet Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1042 |
| **Namespace** | `LowlandTech.Components.Sheet` |
| **Root Element** | `<div>` |
| **data-slot** | `sheet` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/sheet |

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
SPEC1042.Sheet/
├── Sheet.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSheet.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SheetSut = LowlandTech.Components.Sheet.Sheet;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1042")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Sheet component")]
public class SC01_RenderSheet : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sheet renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SheetSut>();
        cut.Find("[data-slot='sheet']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Sheet.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
