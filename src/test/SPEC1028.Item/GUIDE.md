# Item Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1028 |
| **Namespace** | `LowlandTech.Components.Item` |
| **Root Element** | `<div>` |
| **data-slot** | `item` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/item |

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
SPEC1028.Item/
├── Item.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderItem.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ItemSut = LowlandTech.Components.Item.Item;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1028")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Item component")]
public class SC01_RenderItem : ViewScenario
{
    [Fact(DisplayName = "UAC01: Item renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ItemSut>();
        cut.Find("[data-slot='item']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Item.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
