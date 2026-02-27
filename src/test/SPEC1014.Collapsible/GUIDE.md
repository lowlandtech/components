# Collapsible Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1014 |
| **Namespace** | `LowlandTech.Components.Collapsible` |
| **Root Element** | `<div>` |
| **data-slot** | `collapsible` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/collapsible |

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
SPEC1014.Collapsible/
├── Collapsible.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderCollapsible.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using CollapsibleSut = LowlandTech.Components.Collapsible.Collapsible;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1014")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Collapsible component")]
public class SC01_RenderCollapsible : ViewScenario
{
    [Fact(DisplayName = "UAC01: Collapsible renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CollapsibleSut>();
        cut.Find("[data-slot='collapsible']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Collapsible.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
