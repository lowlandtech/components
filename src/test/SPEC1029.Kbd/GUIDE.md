# Kbd Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1029 |
| **Namespace** | `LowlandTech.Components.Kbd` |
| **Root Element** | `<kbd>` |
| **data-slot** | `kbd` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/kbd |

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
SPEC1029.Kbd/
├── Kbd.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderKbd.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using KbdSut = LowlandTech.Components.Kbd.Kbd;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1029")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Kbd component")]
public class SC01_RenderKbd : ViewScenario
{
    [Fact(DisplayName = "UAC01: Kbd renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<KbdSut>();
        cut.Find("[data-slot='kbd']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Kbd.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
