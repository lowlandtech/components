# Input Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1025 |
| **Namespace** | `LowlandTech.Components.Input` |
| **Root Element** | `<input>` |
| **data-slot** | `input` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/input |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |
| `type` | `string?` | `null` | Component-specific |
| `placeholder` | `string?` | `null` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1025.Input/
├── Input.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderInput.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using InputSut = LowlandTech.Components.Input.Input;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1025")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Input component")]
public class SC01_RenderInput : ViewScenario
{
    [Fact(DisplayName = "UAC01: Input renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Input.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
