# Toggle Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1052 |
| **Namespace** | `LowlandTech.Components.Toggle` |
| **Root Element** | `<button>` |
| **data-slot** | `toggle` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/toggle |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |
| `pressed` | `bool` | `false` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1052.Toggle/
├── Toggle.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderToggle.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ToggleSut = LowlandTech.Components.Toggle.Toggle;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1052")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Toggle component")]
public class SC01_RenderToggle : ViewScenario
{
    [Fact(DisplayName = "UAC01: Toggle renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ToggleSut>();
        cut.Find("[data-slot='toggle']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Toggle.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
