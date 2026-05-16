# Resizable Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1038 |
| **Namespace** | `LowlandTech.Components.Resizable` |
| **Root Element** | `<div>` |
| **data-slot** | `resizable-panel-group` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/resizable |

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
SPEC1038.Resizable/
├── Resizable.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderResizable.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Resizable component")]
public class SC01_RenderResizable : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resizable renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ResizableSut>();
        cut.Find("[data-slot='resizable-panel-group']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Resizable.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
