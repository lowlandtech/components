# Calendar Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1009 |
| **Namespace** | `LowlandTech.Components.Calendar` |
| **Root Element** | `<div>` |
| **data-slot** | `calendar` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/calendar |

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
SPEC1009.Calendar/
├── Calendar.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderCalendar.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using CalendarSut = LowlandTech.Components.Calendar.Calendar;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Calendar component")]
public class SC01_RenderCalendar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CalendarSut>();
        cut.Find("[data-slot='calendar']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Calendar.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
