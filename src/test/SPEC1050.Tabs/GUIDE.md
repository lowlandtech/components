# Tabs Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1050 |
| **Namespace** | `LowlandTech.Components.Tabs` |
| **Root Element** | `<div>` |
| **data-slot** | `tabs` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/tabs |

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
SPEC1050.Tabs/
├── Tabs.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderTabs.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Tabs component")]
public class SC01_RenderTabs : ViewScenario
{
    [Fact(DisplayName = "UAC01: Tabs renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TabsSut>();
        cut.Find("[data-slot='tabs']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Tabs.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
