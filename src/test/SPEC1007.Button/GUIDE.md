# Button Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1007 |
| **Namespace** | `LowlandTech.Components.Button` |
| **Root Element** | `<button>` |
| **data-slot** | `button` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/button |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `LtComponentBase` |
| `variant` | `string?` | `null` | Component-specific |
| `size` | `string?` | `null` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1007.Button/
├── Button.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderButton.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ButtonSut = LowlandTech.Components.Button.Button;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1007")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Button component")]
public class SC01_RenderButton : ViewScenario
{
    [Fact(DisplayName = "UAC01: Button renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ButtonSut>();
        cut.Find("[data-slot='button']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Button.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
