# Dialog Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1018 |
| **Namespace** | `LowlandTech.Components.Dialog` |
| **Root Element** | `<div>` |
| **data-slot** | `dialog` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/dialog |

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
SPEC1018.Dialog/
├── Dialog.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderDialog.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using DialogSut = LowlandTech.Components.Dialog.Dialog;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1018")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Dialog component")]
public class SC01_RenderDialog : ViewScenario
{
    [Fact(DisplayName = "UAC01: Dialog renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DialogSut>();
        cut.Find("[data-slot='dialog']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Dialog.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
