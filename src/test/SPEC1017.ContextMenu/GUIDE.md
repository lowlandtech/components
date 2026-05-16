# ContextMenu Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1017 |
| **Namespace** | `LowlandTech.Components.ContextMenu` |
| **Root Element** | `<div>` |
| **data-slot** | `context-menu` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/context-menu |

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
SPEC1017.ContextMenu/
├── ContextMenu.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderContextMenu.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ContextMenuSut = LowlandTech.Components.ContextMenu.ContextMenu;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ContextMenu component")]
public class SC01_RenderContextMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuSut>();
        cut.Find("[data-slot='context-menu']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `ContextMenu.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
