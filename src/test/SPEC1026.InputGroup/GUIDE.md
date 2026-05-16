# InputGroup Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1026 |
| **Namespace** | `LowlandTech.Components.InputGroup` |
| **Root Element** | `<div>` |
| **data-slot** | `input-group` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/input-group |

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
SPEC1026.InputGroup/
├── InputGroup.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderInputGroup.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using InputGroupSut = LowlandTech.Components.InputGroup.InputGroup;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1026")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render InputGroup component")]
public class SC01_RenderInputGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputGroupSut>();
        cut.Find("[data-slot='input-group']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `InputGroup.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
