# Pagination Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1034 |
| **Namespace** | `LowlandTech.Components.Pagination` |
| **Root Element** | `<nav>` |
| **data-slot** | `pagination` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/pagination |

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
SPEC1034.Pagination/
├── Pagination.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderPagination.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using PaginationSut = LowlandTech.Components.Pagination.Pagination;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1034")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Pagination component")]
public class SC01_RenderPagination : ViewScenario
{
    [Fact(DisplayName = "UAC01: Pagination renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PaginationSut>();
        cut.Find("[data-slot='pagination']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Pagination.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
