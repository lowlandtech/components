# NavigationMenu Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1033 |
| **Namespace** | `LowlandTech.Components.NavigationMenu` |
| **Root Element** | `<nav>` |
| **data-slot** | `navigation-menu` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/navigation-menu |

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
SPEC1033.NavigationMenu/
├── NavigationMenu.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderNavigationMenu.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using NavigationMenuSut = LowlandTech.Components.NavigationMenu.NavigationMenu;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1033")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render NavigationMenu component")]
public class SC01_RenderNavigationMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: NavigationMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuSut>();
        cut.Find("[data-slot='navigation-menu']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `NavigationMenu.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
