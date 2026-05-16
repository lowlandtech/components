# Empty Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1022 |
| **Namespace** | `LowlandTech.Components.Empty` |
| **Root Element** | `<div>` |
| **data-slot** | `empty` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/empty |

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
SPEC1022.Empty/
├── Empty.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderEmpty.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using EmptySut = LowlandTech.Components.Empty.Empty;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1022")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Empty component")]
public class SC01_RenderEmpty : ViewScenario
{
    [Fact(DisplayName = "UAC01: Empty renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<EmptySut>();
        cut.Find("[data-slot='empty']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Empty.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
