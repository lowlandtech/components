# Switch Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1048 |
| **Namespace** | `LowlandTech.Components.Switch` |
| **Root Element** | `<button>` |
| **data-slot** | `switch` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/switch |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |
| `@checked` | `bool` | `false` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1048.Switch/
├── Switch.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSwitch.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SwitchSut = LowlandTech.Components.Switch.Switch;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1048")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Switch component")]
public class SC01_RenderSwitch : ViewScenario
{
    [Fact(DisplayName = "UAC01: Switch renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SwitchSut>();
        cut.Find("[data-slot='switch']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Switch.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
