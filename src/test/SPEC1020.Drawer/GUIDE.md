# Drawer Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1020 |
| **Namespace** | `LowlandTech.Components.Drawer` |
| **Root Element** | `<div>` |
| **data-slot** | `drawer` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/drawer |

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
SPEC1020.Drawer/
├── Drawer.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderDrawer.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using DrawerSut = LowlandTech.Components.Drawer.Drawer;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Drawer component")]
public class SC01_RenderDrawer : ViewScenario
{
    [Fact(DisplayName = "UAC01: Drawer renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DrawerSut>();
        cut.Find("[data-slot='drawer']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Drawer.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
