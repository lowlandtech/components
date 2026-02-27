# Alert Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1001 |
| **Namespace** | `LowlandTech.Components.Alert` |
| **Root Element** | `<div>` |
| **data-slot** | `alert` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/alert |

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
SPEC1001.Alert/
├── Alert.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderAlert.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using AlertSut = LowlandTech.Components.Alert.Alert;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1001")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Alert component")]
public class SC01_RenderAlert : ViewScenario
{
    [Fact(DisplayName = "UAC01: Alert renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AlertSut>();
        cut.Find("[data-slot='alert']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Alert.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
