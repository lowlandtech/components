# DropdownMenu Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1021 |
| **Namespace** | `LowlandTech.Components.DropdownMenu` |
| **Root Element** | `<div>` |
| **data-slot** | `dropdown-menu` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/dropdown-menu |

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
SPEC1021.DropdownMenu/
├── DropdownMenu.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderDropdownMenu.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using DropdownMenuSut = LowlandTech.Components.DropdownMenu.DropdownMenu;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1021")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render DropdownMenu component")]
public class SC01_RenderDropdownMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: DropdownMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuSut>();
        cut.Find("[data-slot='dropdown-menu']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `DropdownMenu.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
