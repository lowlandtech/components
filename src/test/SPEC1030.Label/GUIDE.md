# Label Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1030 |
| **Namespace** | `LowlandTech.Components.Label` |
| **Root Element** | `<label>` |
| **data-slot** | `label` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/label |

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
SPEC1030.Label/
├── Label.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderLabel.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using LabelSut = LowlandTech.Components.Label.Label;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1030")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Label component")]
public class SC01_RenderLabel : ViewScenario
{
    [Fact(DisplayName = "UAC01: Label renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<LabelSut>();
        cut.Find("[data-slot='label']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Label.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
