# AlertDialog Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1002 |
| **Namespace** | `LowlandTech.Components.AlertDialog` |
| **Root Element** | `<div>` |
| **data-slot** | `alert-dialog` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/alert-dialog |

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
SPEC1002.AlertDialog/
├── AlertDialog.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderAlertDialog.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using AlertDialogSut = LowlandTech.Components.AlertDialog.AlertDialog;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1002")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render AlertDialog component")]
public class SC01_RenderAlertDialog : ViewScenario
{
    [Fact(DisplayName = "UAC01: AlertDialog renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogSut>();
        cut.Find("[data-slot='alert-dialog']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `AlertDialog.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
