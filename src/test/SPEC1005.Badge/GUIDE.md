# Badge Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1005 |
| **Namespace** | `LowlandTech.Components.Badge` |
| **Root Element** | `<div>` |
| **data-slot** | `badge` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/badge |

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
SPEC1005.Badge/
├── Badge.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderBadge.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using BadgeSut = LowlandTech.Components.Badge.Badge;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1005")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Badge component")]
public class SC01_RenderBadge : ViewScenario
{
    [Fact(DisplayName = "UAC01: Badge renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<BadgeSut>();
        cut.Find("[data-slot='badge']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Badge.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
