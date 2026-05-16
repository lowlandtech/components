# Table Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1049 |
| **Namespace** | `LowlandTech.Components.Table` |
| **Root Element** | `<div>` |
| **data-slot** | `table` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/table |

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
SPEC1049.Table/
├── Table.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderTable.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using TableSut = LowlandTech.Components.Table.Table;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1049")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Table component")]
public class SC01_RenderTable : ViewScenario
{
    [Fact(DisplayName = "UAC01: Table renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TableSut>();
        cut.Find("[data-slot='table']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Table.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
