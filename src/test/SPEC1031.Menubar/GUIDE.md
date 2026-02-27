# Menubar Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1031 |
| **Namespace** | `LowlandTech.Components.Menubar` |
| **Root Element** | `<div>` |
| **data-slot** | `menubar` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/menubar |

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
SPEC1031.Menubar/
├── Menubar.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderMenubar.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using MenubarSut = LowlandTech.Components.Menubar.Menubar;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1031")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Menubar component")]
public class SC01_RenderMenubar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Menubar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<MenubarSut>();
        cut.Find("[data-slot='menubar']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Menubar.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
