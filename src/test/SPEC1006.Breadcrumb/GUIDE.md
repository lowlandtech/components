# Breadcrumb Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1006 |
| **Namespace** | `LowlandTech.Components.Breadcrumb` |
| **Root Element** | `<nav>` |
| **data-slot** | `breadcrumb` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/breadcrumb |

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
SPEC1006.Breadcrumb/
├── Breadcrumb.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderBreadcrumb.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using BreadcrumbSut = LowlandTech.Components.Breadcrumb.Breadcrumb;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1006")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Breadcrumb component")]
public class SC01_RenderBreadcrumb : ViewScenario
{
    [Fact(DisplayName = "UAC01: Breadcrumb renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<BreadcrumbSut>();
        cut.Find("[data-slot='breadcrumb']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Breadcrumb.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
